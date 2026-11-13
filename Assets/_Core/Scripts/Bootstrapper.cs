using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
    [SerializeField] private InputController _inputController;
    [SerializeField] private PlayerShootController _playerShootController;
    [SerializeField] private PlayerSizeController _playerSizeController;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Transform _goalTransform;
    [SerializeField] private GameObject _pathSegmentPrefab;
    [SerializeField] private ObstacleFieldController _obstacleFieldController;
    [SerializeField] private Material _doorMaterial;
    [SerializeField] private Material _explosionMaterial;

    private void Awake()
    {
        Vector3 playerPosition = _playerTransform.position;
        playerPosition.y = _goalTransform.position.y;
        _playerTransform.position = playerPosition;

        ObstacleChainController obstacleChain = GetOrAdd<ObstacleChainController>();
        obstacleChain.SetExplosionMaterial(_explosionMaterial);
        CorridorController corridor = GetOrAdd<CorridorController>();
        corridor.SetSegmentPrefab(_pathSegmentPrefab);
        FinishDoorController finishDoor = GetOrAdd<FinishDoorController>();
        PlayerTraversalController traversal = GetOrAdd<PlayerTraversalController>();
        GameResultView resultView = GetOrAdd<GameResultView>();
        GameFlowController gameFlow = GetOrAdd<GameFlowController>();

        _obstacleFieldController.Generate(_playerTransform, _goalTransform, 90);
        corridor.Build(_playerTransform, _goalTransform);
        finishDoor.Construct(_playerTransform, _goalTransform, _doorMaterial);
        traversal.Construct(_playerTransform, _goalTransform, corridor, finishDoor);
        gameFlow.Construct(_obstacleFieldController, traversal, resultView, _playerShootController);
        _playerShootController.Construct(_playerSizeController, _playerTransform, _goalTransform,
            gameFlow, obstacleChain, _obstacleFieldController);
        _inputController.Construct(_playerShootController);
    }

    private T GetOrAdd<T>() where T : Component
    {
        T component = GetComponent<T>();
        return component != null ? component : gameObject.AddComponent<T>();
    }
}
