using Core.Scripts.Windows;
using UnityEngine;
using UnityEngine.UI;

public class ControlsWindow : UIWindow
{
    public override void Hide()
    {
        base.Hide();

        Joystick.enabled = false;
    }

    public override void Show()
    {
        base.Show();

        Joystick.enabled = true;
    }

    [field: SerializeField] public Button JumpButton { get; private set; }
    [field: SerializeField] public Button AttackButton { get; private set; }
    [field: SerializeField] public MonoJoystick Joystick { get; private set; }
}
