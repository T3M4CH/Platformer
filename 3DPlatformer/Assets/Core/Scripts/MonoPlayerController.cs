using System;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;

public class MonoPlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private Animator animator;
    [SerializeField] private Button jumpButton;
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private MonoJoystick joystick;
    [SerializeField] private MonoInteractionSystem interactionSystem;

    private Vector3 _direction;
    
    private static readonly int Jump = Animator.StringToHash("Jump");
    private static readonly int JoystickOffset = Animator.StringToHash("JoystickOffset");

    private void Move()
    {
        rigidBody.MovePosition(rigidBody.position + _direction * (speed * Time.deltaTime));
        rigidBody.MoveRotation(Quaternion.Euler(0, Math.Sign(_direction.x) * -90, 0));
        
        animator.SetFloat(JoystickOffset, Mathf.Abs(_direction.x));
    }

    private void PerformJump()
    {
        if (!interactionSystem.IsGround) return;
        
        rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        animator.SetTrigger(Jump);
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Update()
    {
        _direction.x = joystick.Direction.x;
        
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            PerformJump();    
        }
    }

    private void Start()
    {
        jumpButton.onClick.AddListener(PerformJump);
    }

    private void OnDestroy()
    {
        jumpButton.onClick.RemoveAllListeners();
    }
}
