using UnityEngine;

public class BallTrailController : MonoBehaviour
{
    [SerializeField] private float trailDuration = 0.45f;

    private TrailRenderer _trail;

    private void Awake()
    {
        _trail = GetComponent<TrailRenderer>();
        if (_trail == null)
            _trail = gameObject.AddComponent<TrailRenderer>();

        Renderer ballRenderer = GetComponent<Renderer>();
        if (ballRenderer != null && ballRenderer.sharedMaterial != null)
        {
            _trail.sharedMaterial = ballRenderer.sharedMaterial;
            _trail.colorGradient = CreateGradient(GetBallColor(ballRenderer.sharedMaterial));
        }

        _trail.time = trailDuration;
        _trail.minVertexDistance = 0.025f;
        _trail.widthCurve = AnimationCurve.Linear(0f, 1f, 1f, 0.04f);
        _trail.widthMultiplier = transform.lossyScale.x;
        _trail.emitting = false;
        _trail.enabled = false;
    }

    public void SetTrailActive(bool active)
    {
        if (_trail == null)
            return;

        _trail.Clear();
        RefreshWidth();
        _trail.enabled = active;
        _trail.emitting = active;
    }

    public void RefreshWidth()
    {
        if (_trail != null)
            _trail.widthMultiplier = transform.lossyScale.x;
    }

    private static Color GetBallColor(Material material)
    {
        if (material.HasProperty("_BaseColor"))
            return material.GetColor("_BaseColor");
        if (material.HasProperty("_Color"))
            return material.GetColor("_Color");
        return Color.white;
    }

    private static Gradient CreateGradient(Color color)
    {
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new[]
            {
                new GradientColorKey(color, 0f),
                new GradientColorKey(color, 1f)
            },
            new[]
            {
                new GradientAlphaKey(color.a, 0f),
                new GradientAlphaKey(0f, 1f)
            });
        return gradient;
    }
}
