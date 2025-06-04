using Core.Scripts.Effects.Interfaces;
using Core.Scripts.Entity;
using Core.Scripts.Entity.Interfaces;
using UnityEngine;

namespace Core.Scripts.StatesMachine
{
    public class ExplodeEntityState : EntityState
    {
        private float _targetTime = 4f;
        private float _currentTime = 0f;

        private MonoEffect _prepareEffect;

        private readonly IAuraCharger _auraCharger;
        private readonly IEffectService _effectService;

        public ExplodeEntityState(EntityStateMachine entityStateMachine, BaseEntity baseEntity, IEffectService effectService) : base(entityStateMachine, baseEntity)
        {
            _effectService = effectService;
            _auraCharger = baseEntity.GetComponent<IAuraCharger>();
        }

        public override void Enter()
        {
            base.Enter();

            _currentTime = 0;

            _auraCharger.Show(BaseEntity.transform.position.y + 1, 3, false);
            _prepareEffect = _effectService.GetEffect(EVfxType.ExplosionPrepare, true);
            _prepareEffect.SetPosition(BaseEntity.transform.position + new Vector3(0, 1, 0), scale: Vector3.zero);
            _prepareEffect.transform.SetParent(BaseEntity.transform);
        }

        public override void Update()
        {
            base.Update();

            _currentTime += Time.deltaTime;

            _prepareEffect.SetPosition(scale: Mathf.Lerp(0, 5, _currentTime / _targetTime) * Vector3.one);

            if (_currentTime > _targetTime)
            {
                _currentTime = 0;

                Explode();
            }
        }

        private void Explode()
        {
            var transform = BaseEntity.transform;
            var position = transform.position + new Vector3(0, 1, 0);
            _effectService.GetEffect(EVfxType.Hit, true).SetPosition(position);

            var damagedCount = 0;

            var colliders = Physics.OverlapSphere(position, 3f, BaseEntity.EntityLayerMask);
            foreach (var collider in colliders)
            {
                if (collider.isTrigger || collider.transform == transform) continue;
                damagedCount += 1;
                collider.GetComponent<IDamageable>()?.TakeDamage(20, 5 * new Vector3(Mathf.Sign(collider.transform.position.x - position.x), 1, 0));
            }

            StateMachine.GetState<BossMoveEntityState>().DamagedCount = damagedCount;
            StateMachine.SetState<FallEntityState>();
        }

        public override void Exit()
        {
            base.Exit();

            _auraCharger.Cancel();

            BaseEntity.Animator.speed = 1;
            BaseEntity.RigidBody.isKinematic = false;

            if (BaseEntity.StateMachine.CurrentEntityState is ThrownEntityState)
            {
                Debug.LogWarning("Knock!");
                BaseEntity.Animator.CrossFade("Knocked", 0, 0);
            }

            Debug.Break();

            if (_prepareEffect)
            {
                _prepareEffect.Disable();
            }

            _prepareEffect = null;
        }
    }
}