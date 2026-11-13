using System;
using UnityEngine;

public class PlayerSizeController:MonoBehaviour
{
    internal void Resize(float amount)
        => transform.localScale = Vector3.one * Mathf.Max(0f, amount);
}
