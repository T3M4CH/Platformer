using UnityEngine.InputSystem;
using UnityEngine;

public class MonoPlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private Rigidbody rigidBody;

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
        if (Keyboard.current.aKey.isPressed)
        {
            _direction.x = -1;
        }
        else if (Keyboard.current.dKey.isPressed)
        {
            _direction.x = 1;
        }
        else
        {
            _direction.x = 0;
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            PerformJump();    
        }
    }
}
