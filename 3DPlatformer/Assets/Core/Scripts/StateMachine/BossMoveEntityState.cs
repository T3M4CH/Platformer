using Core.Scripts.Entity.Managers.Interfaces;
using Core.Scripts.Entity;
using UnityEngine;

namespace Core.Scripts.StatesMachine
{
    public class BossMoveEntityState : MoveEntityState
    {
        private float currentJumpAttackTime = 0f;
        private float currentBowAttackTime = 0f;

        private MonoPlayerController _playerController;
        private readonly IPlayerService _playerService;
        private readonly float _jumpAttackCooldown = 5f;
        private readonly float _bowAttackCooldown = 10f;

        public BossMoveEntityState(EntityStateMachine entityStateMachine, BossEntity baseEntity, IPlayerService playerService, MonoInteractionSystem interactionSystem) : base(entityStateMachine, baseEntity, interactionSystem)
        {
            _playerService = playerService;
            currentBowAttackTime = _bowAttackCooldown;
            currentJumpAttackTime = _jumpAttackCooldown;
        }

        public override void Enter()
        {
            base.Enter();

            _playerController = _playerService.PlayerInstance;
        }

        public override void Update()
        {
            base.Update();

            //TODO: Скиллы + добавить индикаторы + КД

            if (_playerController.StateMachine.CurrentEntityState is ThrownEntityState)
            {
                StateMachine.SetState<ExplodeEntityState>();
                Direction.x = 0;
                return;
            }

            var direction = (_playerController.transform.position - BaseEntity.transform.position).normalized;

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
    }
}