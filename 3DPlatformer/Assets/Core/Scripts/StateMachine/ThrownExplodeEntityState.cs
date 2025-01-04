using System;
using Core.Scripts.Effects.Interfaces;
using Core.Scripts.Entity;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Scripts.StatesMachine
{
    public class ThrownExplodeEntityState : ThrownEntityState
    {
        private int counter;
        private float currentTime;
        private const float StackTime = 5f;

        public ThrownExplodeEntityState(EntityStateMachine entityStateMachine, EntityState exitState, BaseEntity baseEntity, IEffectService effectService) : base(entityStateMachine, exitState, baseEntity, effectService)
        {
        }

        private void Explode()
        {
            var transform = BaseEntity.transform;
            EffectService.GetEffect(EVfxType.Hit, true).SetPosition(transform.position);

            var colliders = Physics.OverlapSphere(transform.position, 3f, BaseEntity.EntityLayerMask);
            foreach (var collider in colliders)
            {
                if (collider.isTrigger || collider.transform == transform) continue;
                collider.GetComponent<IDamageable>()?.TakeDamage(20, 5 * new Vector3(Mathf.Sign(collider.transform.position.x - transform.position.x), 1, 0));
            }

            UniTask.Void(async () =>
            {
                await UniTask.Yield(PlayerLoopTiming.Update);
                StateMachine.SetState(ExitState);
            });
        }

        public override void Enter()
        {
            counter += 1;

            if (counter >= 3)
            {
                counter = 0;
                Explode();
            }
            else
            {
                base.Enter();
            }
        }

        public override void Update()
        {
            base.Update();

            currentTime += Time.deltaTime;

            if (currentTime > StackTime)
            {
                currentTime = 0;
                counter -= 1;

                counter = Mathf.Max(counter, 0);
            }
        }
    }
}