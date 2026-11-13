using UnityEngine;

public class InputController : MonoBehaviour
{
    private PlayerShootController _playerShootController;

    public void Construct(PlayerShootController playerShootController)
    {
        _playerShootController = playerShootController;
    }

    private void Update()
    {
        if (_playerShootController == null)
            return;

        if (Input.GetMouseButtonDown(0))
            _playerShootController.BeginCharge();

        if (Input.GetMouseButton(0))
            _playerShootController.Charge();

        if (Input.GetMouseButtonUp(0))
            _playerShootController.Discharge();
    }
}
