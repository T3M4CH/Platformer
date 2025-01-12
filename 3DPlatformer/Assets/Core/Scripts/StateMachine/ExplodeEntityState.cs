using Core.Scripts.Effects.Interfaces;
using Core.Scripts.Entity;
using UnityEngine;

namespace Core.Scripts.StatesMachine
{
    public class ExplodeEntityState : EntityState
    {
        private float _targetTime = 4f;
        private float _currentTime = 0f;

        private MonoEffect _prepareEffect;

        private readonly IEffectService _effectService;

        public ExplodeEntityState(EntityStateMachine entityStateMachine, BaseEntity baseEntity, IEffectService effectService) : base(entityStateMachine, baseEntity)
        {
            _effectService = effectService;
        }

        public override void Enter()
        {
            base.Enter();

            _currentTime = 0;

            _prepareEffect = _effectService.GetEffect(EVfxType.ExplosionPrepare, true);
            _prepareEffect.SetPosition(BaseEntity.transform.position, scale: Vector3.zero);
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
            _effectService.GetEffect(EVfxType.Hit, true).SetPosition(transform.position);

            var damagedCount = 0;

            var colliders = Physics.OverlapSphere(transform.position, 3f, BaseEntity.EntityLayerMask);
            foreach (var collider in colliders)
            {
                if (collider.isTrigger || collider.transform == transform) continue;
                damagedCount += 1;
                collider.GetComponent<IDamageable>()?.TakeDamage(20, 5 * new Vector3(Mathf.Sign(collider.transform.position.x - transform.position.x), 1, 0));
            }

            //TODO : Change to bow attack
            StateMachine.SetState<BossMoveEntityState>().DamagedCount = damagedCount;
        }

        public override void Exit()
        {
            base.Exit();

            if (_prepareEffect)
            {
                _prepareEffect.Disable();
            }

            _prepareEffect = null;
        }
    }
}