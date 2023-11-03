using System;
using UnityEngine;

public class MonoDefaultEnemy : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private MonoInteractionSystem interactionSystem;

    private Vector3 _direction = Vector3.right;
    private static readonly int JoystickOffset = Animator.StringToHash("JoystickOffset");
    
    private void FixedUpdate()
    {
        if (!interactionSystem.IsGround.Under)
        {
            _direction *= -1; 
            rigidBody.MoveRotation(Quaternion.LookRotation(_direction));
        }

        rigidBody.MovePosition(rigidBody.position + _direction * (Time.fixedDeltaTime * speed));
    }

    private void Start()
    {
        animator.SetFloat(JoystickOffset, 0.5f);
    }
}