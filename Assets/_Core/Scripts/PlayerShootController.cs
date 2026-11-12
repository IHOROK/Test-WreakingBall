using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerShootController : MonoBehaviour
{
    [SerializeField] private PlayerBullet bulletPrefab;

    private PlayerSizeController _playerSizeController;
    private Transform _playerTransform;
    private Transform _goalTransform;
    private float remainingCharge = 100f;
    private float currentCharge = 0;
    private float deltaCharge = 0;

    public void Construct(PlayerSizeController playerSizeController, 
        Transform playerTransform,
        Transform goalTransform
        )
    { 
        _playerSizeController = playerSizeController;
        _playerTransform = playerTransform;
        _goalTransform = goalTransform;
    }

    public void Charge()
    {
        deltaCharge = Time.deltaTime * CoreConstants.DeltaPlayerChargePercent;
        currentCharge += deltaCharge;
        remainingCharge -= deltaCharge;
        _playerSizeController.Resize(remainingCharge/100f);
    }

    public void Discharge()
    {
        Shoot(currentCharge);
        currentCharge = 0;
    }

    private void Shoot(float currentCharge)
    {
        PlayerBullet newBullet = Instantiate(bulletPrefab, _playerTransform.position,Quaternion.identity);
        newBullet.Set(currentCharge, GetDirection());
    }

    private Vector3 GetDirection() 
        => _goalTransform.position - _playerTransform.position;
}