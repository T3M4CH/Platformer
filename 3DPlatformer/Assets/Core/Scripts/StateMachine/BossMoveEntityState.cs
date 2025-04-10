using System.Collections;
using Core.Scripts.Entity.Managers.Interfaces;
using Core.Scripts.Entity;
using UnityEngine;

namespace Core.Scripts.StatesMachine
{
    public class BossMoveEntityState : MoveEntityState
    {
        public int DamagedCount;

        private float currentJumpAttackTime = 0f;
        private float currentBowAttackTime = 0f;

        private MonoPlayerController _playerController;
        private readonly MoveBehaviour _moveBehaviour;
        private readonly IPlayerService _playerService;
        private readonly float _jumpAttackCooldown = 5f;
        private readonly float _bowAttackCooldown = 10f;

        private Coroutine _bowJumpCoroutine;

        public BossMoveEntityState(EntityStateMachine entityStateMachine, BossEntity baseEntity, IPlayerService playerService, MonoInteractionSystem interactionSystem) : base(entityStateMachine, baseEntity, interactionSystem)
        {
            _playerService = playerService;
            currentBowAttackTime = _bowAttackCooldown;
            currentJumpAttackTime = _jumpAttackCooldown;
            _moveBehaviour = baseEntity.Animator.GetBehaviour<MoveBehaviour>();
        }

        public override void Enter()
        {
            base.Enter();

            DamagedCount = 0;
            _playerController = _playerService.PlayerInstance;
        }

        public override void Update()
        {
            base.Update();

            //TODO: Скиллы + добавить индикаторы + КД

            if (!_moveBehaviour.isMoving || _bowJumpCoroutine != null) return;

            if (_playerController.StateMachine.CurrentEntityState is ThrownEntityState)
            {
                if (DamagedCount > 0)
                {
                    _bowJumpCoroutine = CoroutineRunner.Instance.StartCoroutine(BowJumpCoroutine());
                    return;
                }

                Direction.x = 0;
                _animator.SetFloat(JoystickOffset, Mathf.Abs(Direction.x));
                StateMachine.SetState<ExplodeEntityState>();
                return;
            }

            var direction = Vector3.ClampMagnitude(_playerController.transform.position - BaseEntity.transform.position, 0.6f);

            Direction.x = direction.x;

            currentJumpAttackTime -= Time.deltaTime;

            if (currentJumpAttackTime < 0)
            {
                currentJumpAttackTime = _jumpAttackCooldown;
                StateMachine.SetState<BossJumpEntityState>().SetAimTarget(_playerController.transform);
            }

            if (IsCloseToEnemy)
            {
                StateMachine.SetState<MeleeAttackEntityState>().SetAimTarget(_playerController);
            }
        }

        private IEnumerator BowJumpCoroutine()
        {
            yield return new WaitForSeconds(2);

            StateMachine.SetState<BossJumpEntityState>().SetAimTarget(_playerController.transform, true);
        }

        public override void Exit()
        {
            base.Exit();

            if (_bowJumpCoroutine != null)
            {
                CoroutineRunner.Instance.StopConcreteCoroutine(_bowJumpCoroutine);
            }

            _bowJumpCoroutine = null;
        }
    }
}