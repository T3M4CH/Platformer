using System;
using Core.Scripts.Entity;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core.Scripts.StatesMachine
{
    public class BossIdleEntityState : EntityState
    {
        private readonly MonoPlayerController _playerController;

        public BossIdleEntityState(EntityStateMachine entityStateMachine, MonoPlayerController playerController, BossEntity baseEntity) : base(entityStateMachine, baseEntity)
        {
            _playerController = playerController;
        }

        public override void Enter()
        {
            base.Enter();

            // UniTask.Void(async () =>
            // {
            //     await UniTask.Delay(TimeSpan.FromSeconds(2));
            //
            //     var condition = Random.Range(0, 100f) > 50;
            //
            //     if (condition)
            //     {
            //         StateMachine.SetState<MeleeAttackEntityState>();
            //     }
            //     else
            //     {
            //         StateMachine.SetState<BossJumpEntityState>();
            //     }
            // });
        }

        public override void Update()
        {
            base.Update();

            if (Vector3.Distance(BaseEntity.transform.position, _playerController.transform.position) < 5)
            {
                StateMachine.SetState<BossMoveEntityState>();
            }
        }
    }
}