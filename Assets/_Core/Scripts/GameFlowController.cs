using UnityEngine;

public class GameFlowController : MonoBehaviour
{
    private enum GameState { Clearing, Traversing, Won, Lost }

    private GameState _state;
    private ObstacleFieldController _obstacleField;
    private PlayerTraversalController _playerTraversal;
    private GameResultView _resultView;
    private PlayerShootController _playerShootController;

    public bool IsGameOver => _state == GameState.Won || _state == GameState.Lost;
    public bool IsTraversing => _state == GameState.Traversing;

    public void Construct(ObstacleFieldController obstacleField,
        PlayerTraversalController playerTraversal, GameResultView resultView,
        PlayerShootController playerShootController)
    {
        _obstacleField = obstacleField;
        _playerTraversal = playerTraversal;
        _resultView = resultView;
        _playerShootController = playerShootController;
        _playerTraversal.Arrived += HandlePlayerArrived;
        _state = GameState.Clearing;
        _resultView.ShowStatus("Утримуйте, щоб зарядити постріл");
    }

    private void Update()
    {
        if (IsGameOver)
            return;

        if (_state != GameState.Clearing)
            return;

        if (_obstacleField.HasBlockingObstacles)
            return;

        _state = GameState.Traversing;
        _playerShootController.HideAim();
        _resultView.ShowStatus("Шлях розчищено");
        _playerTraversal.Begin();
    }

    public void Lose(string reason)
    {
        if (IsGameOver)
            return;

        _state = GameState.Lost;
        _playerTraversal.Stop();
        _resultView.ShowEnd(reason);
    }

    public void NotifyBombReachedGoal()
    {
        if (IsGameOver || _state == GameState.Traversing)
            return;

        _state = GameState.Traversing;
        _playerShootController.HideAim();
        _resultView.ShowStatus("Бомба дісталась фінішу — рухаємось до нього");
        _playerTraversal.Begin();
    }

    private void HandlePlayerArrived()
    {
        if (_state != GameState.Traversing)
            return;

        Win();
    }

    private void Win()
    {
        if (IsGameOver)
            return;

        _state = GameState.Won;
        _playerTraversal.Stop();
        _resultView.ShowEnd("Рівень пройдено!");
    }

    private void OnDestroy()
    {
        if (_playerTraversal != null)
            _playerTraversal.Arrived -= HandlePlayerArrived;
    }
}
