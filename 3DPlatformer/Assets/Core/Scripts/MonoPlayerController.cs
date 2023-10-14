using UnityEngine.InputSystem;
using UnityEngine;

public class MonoPlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private MonoJoystick joystick;

    private Vector3 _direction;
    
    private void Move()
    {
        rigidBody.MovePosition(rigidBody.position + _direction * (speed * Time.deltaTime));
    }

    private void PerformJump()
    {
        rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
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
}
