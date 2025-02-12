using System;
using Core.Scripts.Entity;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Scripts.StatesMachine
{
    public class BossIdleEntityState : EntityState
    {
        private readonly BossEntity _baseEntity;
        private readonly MonoPlayerController _playerController;

        public BossIdleEntityState(EntityStateMachine entityStateMachine, MonoPlayerController playerController, BossEntity baseEntity) : base(entityStateMachine, baseEntity)
        {
            _playerController = playerController;
            _baseEntity = baseEntity;
        }

        public override void Enter()
        {
            base.Enter();

            // UniTask.Void(async () =>
            // {
            //     await UniTask.Delay(TimeSpan.FromSeconds(3.5f));
            //
            //     //StateMachine.SetState<BossJumpEntityState>().SetAimTarget(_playerController.transform);;
            //     StateMachine.SetState<ExplodeEntityState>();
            // });
        }

        public override void Update()
        {
            base.Update();

            //StateMachine.SetState<BossMoveEntityState>();
            if (Vector3.Distance(BaseEntity.transform.position, _playerController.transform.position) < 5)
            {
                _baseEntity.UpdateHp(1, 1);
                StateMachine.SetState<BossMoveEntityState>();
            }
        }
    }
}