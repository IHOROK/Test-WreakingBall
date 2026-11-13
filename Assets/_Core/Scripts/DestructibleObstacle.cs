using System.Collections;
using UnityEngine;

public class DestructibleObstacle : MonoBehaviour
{
    private static readonly Color InfectedGlow = new Color(0.35f, 1f, 0.12f);

    private Renderer _renderer;
    private Material _material;
    private Color _originalEmission;
    private bool _hasEmission;
    private bool _originalEmissionEnabled;

    public bool IsInfected { get; private set; }
    public bool IsDestroyed { get; private set; }

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer == null)
            return;

        _material = _renderer.material;
        _hasEmission = _material.HasProperty("_EmissionColor");
        if (_hasEmission)
        {
            _originalEmission = _material.GetColor("_EmissionColor");
            _originalEmissionEnabled = _material.IsKeywordEnabled("_EMISSION");
        }
    }

    public bool Infect(float blastRadius, float delay, ObstacleChainController chain,
        bool canInfectNeighbors)
    {
        if (IsInfected || IsDestroyed || chain == null)
            return false;

        IsInfected = true;
        StartCoroutine(InfectionSequence(blastRadius, Mathf.Max(0f, delay), chain,
            canInfectNeighbors));
        return true;
    }

    public void MarkExploded(float destroyDelay)
    {
        if (IsDestroyed)
            return;

        IsDestroyed = true;
        RestoreInfectionGlow();

        Collider obstacleCollider = GetComponent<Collider>();
        if (obstacleCollider != null)
            obstacleCollider.enabled = false;

        Destroy(gameObject, Mathf.Max(0f, destroyDelay));
    }

    private IEnumerator InfectionSequence(float blastRadius, float delay, ObstacleChainController chain,
        bool canInfectNeighbors)
    {
        float elapsed = 0f;
        while (elapsed < delay)
        {
            elapsed += Time.deltaTime;
            if (_material != null && _hasEmission)
            {
                float pulse = 0.5f + 0.5f * Mathf.Sin(elapsed * 22f);
                _material.EnableKeyword("_EMISSION");
                _material.SetColor("_EmissionColor", InfectedGlow * pulse);
            }
            yield return null;
        }

        RestoreInfectionGlow();
        if (!IsDestroyed && chain != null)
            chain.Detonate(this, blastRadius, canInfectNeighbors);
    }

    private void RestoreInfectionGlow()
    {
        if (_material == null || !_hasEmission)
            return;

        _material.SetColor("_EmissionColor", _originalEmission);
        if (!_originalEmissionEnabled)
            _material.DisableKeyword("_EMISSION");
    }
}
