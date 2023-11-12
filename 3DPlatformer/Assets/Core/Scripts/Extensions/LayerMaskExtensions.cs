using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LayerMaskExtensions
{
    public static bool Includes(this int mask, int value)
    {
        return ((1 << value) & mask) != 0;
    }
}
