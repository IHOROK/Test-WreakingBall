using System.Collections;
using UnityEngine;

public class ObstacleChainController : MonoBehaviour
{
    [SerializeField] private float initialInfectionDelay = 0.2f;
    [SerializeField] private Vector2 chainInfectionDelay = new Vector2(0.18f, 0.34f);
    [SerializeField] private float explosionDuration = 0.85f;
    [SerializeField, Range(0f, 1f)] private float obstacleRemovalFraction = 0.6f;
    private Material _explosionMaterial;

    public void SetExplosionMaterial(Material material)
    {
        _explosionMaterial = material;
    }

    public void StartChain(DestructibleObstacle firstObstacle, float blastRadius)
    {
        if (firstObstacle == null)
            return;

        firstObstacle.Infect(blastRadius, initialInfectionDelay, this, true);
    }

    public void Detonate(DestructibleObstacle obstacle, float blastRadius, bool canInfectNeighbors)
    {
        if (obstacle == null || obstacle.IsDestroyed)
            return;

        Vector3 explosionPosition = obstacle.transform.position;
        if (canInfectNeighbors)
        {
            Collider[] colliders = Physics.OverlapSphere(explosionPosition, blastRadius,
                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);

            foreach (Collider hit in colliders)
            {
                DestructibleObstacle neighbor = hit.GetComponentInParent<DestructibleObstacle>();
                if (neighbor == null || neighbor == obstacle)
                    continue;

                float delay = Random.Range(chainInfectionDelay.x, chainInfectionDelay.y);
                neighbor.Infect(blastRadius, delay, this, false);
            }
        }

        obstacle.MarkExploded(explosionDuration * obstacleRemovalFraction);
        StartCoroutine(PlayExplosion(explosionPosition, blastRadius));
    }

    private IEnumerator PlayExplosion(Vector3 position, float radius)
    {
        GameObject effect = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        effect.name = "Obstacle Chain Explosion";
        effect.transform.position = position;
        effect.transform.localScale = Vector3.one * 0.05f;

        Collider effectCollider = effect.GetComponent<Collider>();
        if (effectCollider != null)
            effectCollider.enabled = false;

        Renderer effectRenderer = effect.GetComponent<Renderer>();
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        if (effectRenderer != null && _explosionMaterial != null)
            effectRenderer.sharedMaterial = _explosionMaterial;

        float duration = Mathf.Max(0.2f, explosionDuration);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            float easedProgress = 1f - Mathf.Pow(1f - progress, 3f);
            effect.transform.localScale = Vector3.one * Mathf.Lerp(0.05f, radius * 2f, easedProgress);
            if (effectRenderer != null)
            {
                Color color = Color.Lerp(new Color(1f, 0.75f, 0.12f),
                    new Color(0.9f, 0.12f, 0.02f), progress);
                propertyBlock.SetColor("_BaseColor", color);
                propertyBlock.SetColor("_Color", color);
                effectRenderer.SetPropertyBlock(propertyBlock);
            }
            yield return null;
        }

        Destroy(effect);
    }
}
