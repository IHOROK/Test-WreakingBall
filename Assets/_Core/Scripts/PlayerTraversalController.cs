using System;
using UnityEngine;

public class PlayerTraversalController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float hopHeight = 0.24f;

    private Transform _player;
    private Vector3 _startPosition;
    private Vector3 _goalPosition;
    private float _distance;
    private float _travelled;
    private bool _isMoving;
    private CorridorController _corridor;
    private FinishDoorController _finishDoor;
    private BallTrailController _playerTrail;

    public event Action Arrived;

    public void Construct(Transform player, Transform goal,
        CorridorController corridor, FinishDoorController finishDoor)
    {
        _player = player;
        _startPosition = player.position;
        _goalPosition = goal.position;
        _distance = Vector3.Distance(_startPosition, _goalPosition);
        _corridor = corridor;
        _finishDoor = finishDoor;
        _playerTrail = player.GetComponent<BallTrailController>();
        if (_playerTrail == null)
            _playerTrail = player.gameObject.AddComponent<BallTrailController>();
    }

    public void Begin()
    {
        _travelled = 0f;
        _isMoving = true;
        _playerTrail?.SetTrailActive(true);
    }

    public void Stop()
    {
        _isMoving = false;
        _playerTrail?.SetTrailActive(false);
    }

    private void Update()
    {
        if (!_isMoving)
            return;

        _travelled = Mathf.MoveTowards(_travelled, _distance, moveSpeed * Time.deltaTime);
        float progress = _distance > 0.001f ? _travelled / _distance : 1f;
        Vector3 position = Vector3.Lerp(_startPosition, _goalPosition, progress);
        position.y += Mathf.Abs(Mathf.Sin(_travelled * 5f)) * hopHeight;
        _player.position = position;
        _playerTrail?.RefreshWidth();
        _corridor.UpdateWidth(_player.localScale.x);
        _finishDoor.UpdateForPlayer();

        if (progress < 1f)
            return;

        _player.position = _goalPosition;
        _isMoving = false;
        Arrived?.Invoke();
    }
}
