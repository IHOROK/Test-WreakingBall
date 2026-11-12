using System;
using System.Reflection.Emit;
using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
    [SerializeField] private InputController _inputController;
    [SerializeField] private PlayerShootController _playerShootController;
    [SerializeField] private PlayerSizeController _playerSizeController;
    [SerializeField] private Transform _playerTransform, _goalTransform;

    private void Awake()
    {
        ConstructAll();
    }

    private void ConstructAll()
    {
        _playerShootController.Construct(_playerSizeController, _playerTransform,_goalTransform);
        _inputController.Construct(_playerShootController);
    }
}
