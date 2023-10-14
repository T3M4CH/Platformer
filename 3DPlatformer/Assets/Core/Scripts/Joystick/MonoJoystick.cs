using UnityEngine;

public class MonoJoystick : MonoBehaviour
{
    [SerializeField] private float maxStickOffset;
    [SerializeField] private RectTransform joystickRect;
    [SerializeField] private RectTransform handlerRect;

    private GameObject _joystick;
    private PlayerInput _playerInput;
    private PlayerInput.PlayerActions _input;

    private void ShowGUI()
    {
        _joystick.SetActive(true);
        
        joystickRect.position = _input.TouchPosition.ReadValue<Vector2>();
        handlerRect.localPosition = Vector3.zero;
    }

    private void UpdatePosition()
    {
        var position = (Vector3)_input.TouchPosition.ReadValue<Vector2>();

        var joystickRectPosition = joystickRect.position;
        handlerRect.anchoredPosition = Vector3.ClampMagnitude(position - joystickRectPosition, maxStickOffset);

        Direction = (handlerRect.position - joystickRectPosition) / maxStickOffset;
        print(Direction);
    }

    private void HideGUI()
    {
        _joystick.SetActive(false);
        
        Direction = Vector2.zero;
    }

    private void Update()
    {
        if (_input.Touch.WasPressedThisFrame())
        {
            ShowGUI();
        }

        if (_input.Touch.IsPressed())
        {
            UpdatePosition();    
        }

        if (_input.Touch.WasReleasedThisFrame())
        {
            HideGUI();
        }
    }

    private void Awake()
    {
        _playerInput = new PlayerInput();
        _playerInput.Enable();

        _input = _playerInput.Player;

        _joystick = joystickRect.gameObject;
    }

    private void OnDestroy()
    {
        _playerInput.Disable();
        _playerInput.Dispose();
    }
    
    public Vector2 Direction { get; private set; }
}
