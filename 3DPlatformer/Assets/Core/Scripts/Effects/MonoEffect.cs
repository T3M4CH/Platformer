using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoEffect : MonoBehaviour
{
    private Transform _transform;

    public void SetPosition(Vector3? position = null, Vector3? eulerRotation = null, Vector3? scale = null)
    {
        _transform ??= transform;

        if (position.HasValue)
        {
            _transform.position = position.Value;
        }

        if (eulerRotation.HasValue)
        {
            _transform.eulerAngles = eulerRotation.Value;
        }


        if (scale.HasValue)
        {
            _transform.localScale = scale.Value;
        }
    }

    public void Disable()
    {
        _transform.SetParent(null);
        gameObject.SetActive(false);
    }
}