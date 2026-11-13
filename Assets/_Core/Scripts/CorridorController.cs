using System.Collections.Generic;
using UnityEngine;

public class CorridorController : MonoBehaviour
{
    [SerializeField] private GameObject segmentPrefab;
    private const int SegmentCount = 16;
    private readonly List<Transform> _segments = new List<Transform>();
    private float _length;

    public void SetSegmentPrefab(GameObject prefab)
    {
        segmentPrefab = prefab;
    }

    public void Build(Transform player, Transform goal)
    {
        Vector3 start = player.position;
        Vector3 end = goal.position;
        Vector3 direction = (end - start).normalized;
        _length = Vector3.Distance(start, end);
        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
        float segmentLength = Mathf.Max(0.1f, _length / SegmentCount);

        if (segmentPrefab == null)
        {
            Debug.LogError("Corridor segment prefab is not assigned.", this);
            return;
        }

        for (int i = 0; i < SegmentCount; i++)
        {
            float t = (i + 0.5f) / SegmentCount;
            Vector3 position = Vector3.Lerp(start, end, t) - Vector3.up * 0.55f;
            GameObject segment = Instantiate(segmentPrefab, position, rotation, transform);
            segment.name = "Path Segment";
            segment.transform.localScale = new Vector3(GetWidth(player.localScale.x, t), 0.1f,
                segmentLength + 0.025f);

            _segments.Add(segment.transform);
        }
    }

    public void UpdateWidth(float playerScale)
    {
        float segmentLength = Mathf.Max(0.1f, _length / SegmentCount);
        for (int i = 0; i < _segments.Count; i++)
        {
            float t = (i + 0.5f) / _segments.Count;
            _segments[i].localScale = new Vector3(GetWidth(playerScale, t), 0.1f,
                segmentLength + 0.025f);
        }
    }

    private static float GetWidth(float playerScale, float progress)
    {
        float startWidth = Mathf.Max(0.8f, playerScale + 0.45f);
        float endWidth = Mathf.Max(0.65f, playerScale + 0.15f);
        return Mathf.Lerp(startWidth, endWidth, progress);
    }
}
