using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [SerializeField] private float explosionRadiusMultiplier = 3.5f;

    private Vector3 _direction;
    private float _charge;
    private bool _isPreview;
    private bool _isLaunched;
    private ObstacleChainController _obstacleChain;
    private GameFlowController _gameFlow;
    private Transform _goal;
    private BallTrailController _trail;

    private void Awake()
    {
        _trail = GetComponent<BallTrailController>();
        if (_trail == null)
            _trail = gameObject.AddComponent<BallTrailController>();
    }

    public void BeginPreview()
    {
        _isPreview = true;
        Collider projectileCollider = GetComponent<Collider>();
        if (projectileCollider != null)
            projectileCollider.enabled = false;
        _trail?.SetTrailActive(false);
    }

    public void SetPreviewSize(float charge)
    {
        _charge = Mathf.Clamp01(charge);
        transform.localScale = Vector3.one * Mathf.Max(_charge, 0.035f);
        _trail?.RefreshWidth();
    }

    public void Launch(Vector3 direction, float charge, ObstacleChainController obstacleChain,
        GameFlowController gameFlow, Transform goal)
    {
        _isPreview = false;
        _isLaunched = true;
        _direction = direction.normalized;
        _charge = Mathf.Clamp01(charge);
        _obstacleChain = obstacleChain;
        _gameFlow = gameFlow;
        _goal = goal;
        _startPosition = transform.position;
        transform.localScale = Vector3.one * _charge;

        Collider projectileCollider = GetComponent<Collider>();
        if (projectileCollider != null)
            projectileCollider.enabled = true;
        _trail?.SetTrailActive(true);
    }

    private void Update()
    {
        if (_isPreview || !_isLaunched)
            return;

        float radius = Mathf.Max(0.01f, transform.lossyScale.x * 0.5f);
        float distance = CoreConstants.DefaultPlayerBulletSpeed * Time.deltaTime;
        if (Physics.SphereCast(transform.position, radius, _direction, out RaycastHit hit,
                distance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            if (IsGoalCollider(hit.collider))
            {
                _gameFlow?.NotifyBombReachedGoal();
            }
            else
            {
                DestructibleObstacle obstacle = hit.collider.GetComponentInParent<DestructibleObstacle>();
                if (obstacle != null && _obstacleChain != null)
                {
                    float blastRadius = Mathf.Max(0.15f, _charge * explosionRadiusMultiplier);
                    _obstacleChain.StartChain(obstacle, blastRadius);
                }
            }

            Destroy(gameObject);
            return;
        }

        transform.position += _direction * distance;
        if (Vector3.Distance(transform.position, _startPosition) > 100f)
            Destroy(gameObject);
    }

    private Vector3 _startPosition;

    private void Start()
    {
        _startPosition = transform.position;
    }

    private bool IsGoalCollider(Collider collider)
    {
        if (_goal == null || collider == null)
            return false;

        Transform hitTransform = collider.transform;
        return hitTransform == _goal || hitTransform.IsChildOf(_goal) || _goal.IsChildOf(hitTransform);
    }
}
