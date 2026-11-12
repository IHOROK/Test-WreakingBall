using System;
using UnityEngine;

internal class PlayerBullet : MonoBehaviour
{
    Vector3 _direction = Vector3.zero;
    internal void Set(float currentCharge, Vector3 direction)
    {
        transform.localScale = Vector3.one * currentCharge / 100f;
        _direction = direction;
    }

    private void Update()
    {
        if (_direction != Vector3.zero)
        {
            transform.Translate(_direction.normalized*Time.deltaTime);
        }
    }
}