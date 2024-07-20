using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [field: SerializeField] public Image FillImage { get; private set; }
    [field: SerializeField] public TMP_Text BossName { get; private set; }
}
