using UnityEngine;

public class InputController : MonoBehaviour
{
    PlayerShootController _playerShootController;

    public void Construct(PlayerShootController playerShootController)
    { 
        _playerShootController = playerShootController;
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
            _playerShootController.Charge();

        if (Input.GetMouseButtonUp(0))
            _playerShootController.Discharge();
    }
}
