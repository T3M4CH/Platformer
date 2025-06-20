using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorExtensions
{
    public static Vector2 WorldToScreenPosition(this Vector3 vectorPosition, Camera camera, RectTransform area)
    {
        var position = camera.WorldToScreenPoint(vectorPosition);
        position.z = 0;

        return RectTransformUtility.ScreenPointToLocalPointInRectangle
            (area, position, null, out var screenPosition) ? screenPosition : Vector2.zero;
    }
}