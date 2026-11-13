using UnityEngine;

public class PlayerShootController : MonoBehaviour
{
    [SerializeField] private PlayerBullet bulletPrefab;
    [SerializeField] private LineRenderer aimLine;

    private PlayerSizeController _playerSizeController;
    private Transform _playerTransform;
    private Transform _goalTransform;
    private GameFlowController _gameFlow;
    private ObstacleChainController _obstacleChain;
    private ObstacleFieldController _obstacleField;
    private PlayerBullet _previewBullet;
    private float _remainingSize = 1f;
    private float _currentCharge;
    private bool _isCharging;
    private bool _aimHidden;

    public float RemainingSize => _remainingSize;

    public void Construct(PlayerSizeController playerSizeController, Transform playerTransform,
        Transform goalTransform, GameFlowController gameFlow, ObstacleChainController obstacleChain,
        ObstacleFieldController obstacleField)
    {
        _playerSizeController = playerSizeController;
        _playerTransform = playerTransform;
        _goalTransform = goalTransform;
        _gameFlow = gameFlow;
        _obstacleChain = obstacleChain;
        _obstacleField = obstacleField;
        if (aimLine != null)
        {
            aimLine.gameObject.SetActive(true);
            aimLine.useWorldSpace = true;
            aimLine.positionCount = 2;
            aimLine.startColor = Color.red;
            aimLine.endColor = Color.red;
            aimLine.startWidth = 0.035f;
            aimLine.endWidth = 0.035f;
        }
        _remainingSize = Mathf.Clamp01(_playerTransform.localScale.x);
        _playerSizeController.Resize(_remainingSize);
    }

    private void Update()
    {
        UpdateAimLine();
    }

    public void BeginCharge()
    {
        if (_isCharging || _aimHidden || _gameFlow.IsGameOver || _gameFlow.IsTraversing)
            return;

        _isCharging = true;
        _currentCharge = 0f;
        _previewBullet = Instantiate(bulletPrefab, _playerTransform.position, Quaternion.identity);
        _previewBullet.BeginPreview();
        UpdatePreview();
    }

    public void Charge()
    {
        if (!_isCharging)
            return;

        float minSize = CoreConstants.MinLosePlayerSizePercent / 100f;
        float available = Mathf.Max(0f, _remainingSize - minSize);
        float amount = Mathf.Min(Time.deltaTime * CoreConstants.DeltaPlayerChargePercent / 100f, available);
        _currentCharge += amount;
        _remainingSize -= amount;
        _playerSizeController.Resize(_remainingSize);
        UpdatePreview();

        if (_remainingSize <= minSize + 0.0001f)
        {
            CancelCharge();
            _gameFlow.Lose("Куля-гравець стала замалою.");
        }
    }

    public void Discharge()
    {
        if (!_isCharging)
            return;

        _isCharging = false;
        if (_previewBullet != null)
        {
            if (_currentCharge > 0.005f)
                _previewBullet.Launch(GetDirection(), _currentCharge, _obstacleChain,
                    _gameFlow, _goalTransform);
            else
                Destroy(_previewBullet.gameObject);
        }

        _previewBullet = null;
        _currentCharge = 0f;
    }

    private void UpdatePreview()
    {
        if (_previewBullet == null)
            return;

        Vector3 direction = GetDirection();
        float playerRadius = 0.5f * _playerTransform.localScale.x;
        float shotRadius = 0.5f * Mathf.Max(_currentCharge, 0.035f);
        _previewBullet.transform.position = _playerTransform.position + direction * (playerRadius + shotRadius);
        _previewBullet.SetPreviewSize(Mathf.Max(_currentCharge, 0.035f));
    }

    private Vector3 GetDirection()
    {
        Vector3 target = GetAimTarget();
        Vector3 direction = target - _playerTransform.position;
        return direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector3.forward;
    }

    private Vector3 GetAimTarget()
    {
        DestructibleObstacle nearestObstacle = _obstacleField.FindNearestBlockingObstacle();
        return nearestObstacle != null ? nearestObstacle.transform.position : _goalTransform.position;
    }

    private void UpdateAimLine()
    {
        if (_aimHidden || aimLine == null || _playerTransform == null || _goalTransform == null
            || _gameFlow == null || _gameFlow.IsGameOver)
        {
            if (aimLine != null)
                aimLine.enabled = false;
            return;
        }

        aimLine.enabled = true;
        Vector3 target = GetAimTarget();
        Vector3 offset = target - _playerTransform.position;
        Vector3 direction = offset.sqrMagnitude > 0.0001f ? offset.normalized : Vector3.forward;
        float playerRadius = _playerTransform.lossyScale.x * 0.5f;
        aimLine.SetPosition(0, _playerTransform.position + direction * playerRadius);
        aimLine.SetPosition(1, target);
    }

    public void HideAim()
    {
        _aimHidden = true;
        if (aimLine != null)
            aimLine.enabled = false;
    }

    private void CancelCharge()
    {
        _isCharging = false;
        _currentCharge = 0f;
        if (_previewBullet != null)
            Destroy(_previewBullet.gameObject);
        _previewBullet = null;
    }
}
