using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonoHealthbar : MonoBehaviour
{
    [field: SerializeField] public Image FillImage { get; private set; }
    [field: SerializeField] public CanvasGroup CanvasGroup { get; private set; }
}
