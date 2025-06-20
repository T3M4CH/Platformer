﻿using UnityEngine.InputSystem;
using Core.Scripts.Entity;
using UnityEngine.UI;
using UnityEngine;
using System;

namespace Core.Scripts.StatesMachine
{
    public class MoveEntityState : EntityState, IDisposable
    {
        public MoveEntityState(EntityStateMachine entityStateMachine, BaseEntity baseEntity, MonoInteractionSystem interactionSystem) : base(entityStateMachine, baseEntity)
        {
            _rigidBody = baseEntity.RigidBody;
            _speed = baseEntity.Speed;
            _animator = baseEntity.Animator;
            _collision = baseEntity.EntityCollision;
            EntityLayerMask = baseEntity.EntityLayerMask;
            _idleBehaviour = baseEntity.Animator.GetBehaviour<MoveBehaviour>();
            InteractionSystem = interactionSystem;
        }

        private bool _isJumping;
        protected Vector3 Direction;
        protected bool IsCloseToEnemy;

        private readonly float _speed;
        private readonly float _jumpForce;
        protected readonly Animator _animator;
        private readonly Rigidbody _rigidBody;
        protected readonly LayerMask EntityLayerMask;
        private readonly EntityCollision _collision;
        private readonly MoveBehaviour _idleBehaviour;
        protected readonly MonoInteractionSystem InteractionSystem;
        protected static readonly int JoystickOffset = Animator.StringToHash("JoystickOffset");

        public override void Enter()
        {
            base.Enter();

            IsCloseToEnemy = false;
            //_rigidBody.linearVelocity = Vector3.zero;

            _collision.CollisionStay += OnCollisionStay;
            _collision.CollisionExit += OnCollisionExit;
        }

        private void OnCollisionExit(Collision collision)
        {
            if (EntityLayerMask.value.Includes(collision.gameObject.layer))
            {
                IsCloseToEnemy = false;
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            if (EntityLayerMask.value.Includes(collision.gameObject.layer))
            {
                IsCloseToEnemy = true;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            Move();
        }

        protected void Attack()
        {
            if (!InteractionSystem.IsGround.Under || Direction.x == 0) return;

            StateMachine.SetState<MeleeAttackEntityState>();
        }

        private void Move()
        {
            if (_idleBehaviour.isMoving)
            {
                //_rigidBody.MovePosition(_rigidBody.position + Direction * (_speed * Time.deltaTime));
            }

            var angle = Math.Sign(Direction.x);
            angle = angle == 0 ? 180 : angle * 90;
            _rigidBody.MoveRotation(Quaternion.Euler(0, angle, 0));

            _animator.SetFloat(JoystickOffset, Mathf.Abs(Direction.x));
        }

        public override void Exit()
        {
            base.Exit();

            _collision.CollisionStay -= OnCollisionStay;
            _collision.CollisionExit -= OnCollisionExit;
        }

        public virtual void Dispose()
        {
            _collision.CollisionStay -= OnCollisionStay;
            _collision.CollisionExit -= OnCollisionExit;
        }
    }
}