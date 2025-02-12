using System;
using UnityEngine;

public class AnimatorRootMotion : MonoBehaviour
{
    public bool ApplyRootMotion;
    [SerializeField] private Rigidbody rigidbody;
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnAnimatorMove()
    {
        if (ApplyRootMotion)
        {
            MoveRootMotion();
        }
    }

    private void MoveRootMotion()
    {
        var position = transform.position + _animator.deltaPosition;
        rigidbody.MovePosition(position);
        var rotation = transform.rotation * _animator.deltaRotation;
        //rigidbody.MoveRotation(rotation);
    }
}