using Core.Scripts.Entity.Interfaces;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace Core.Scripts.Entity
{
    public class MonoAuraCharge : MonoBehaviour, IAuraCharger
    {
        [SerializeField] private AnimationCurve animationCurve;

        private BaseEntity _baseEntity;
        private TweenerCore<Vector3, Vector3, VectorOptions> _tween;

        public void Show(float targetY, float time, bool invincible)
        {
            //if (_baseEntity is BossEntity) Debug.Break();
            if (invincible)
            {
                BaseEntity.enabled = false;
                BaseEntity.RigidBody.isKinematic = true;
            }
            else
            {
                BaseEntity.RigidBody.isKinematic = true;
            }

            BaseEntity.transform.DORotate(new Vector3(0, 180, 0), 1);
            BaseEntity.Animator.CrossFade("AuraJump", 0.1f, 0);

            _tween.Kill();
            _tween = BaseEntity.transform.DOMoveY(targetY, time).SetLink(gameObject);
            _tween.OnUpdate(() =>
            {
                BaseEntity.Animator.speed = animationCurve.Evaluate(_tween.position / _tween.Duration());
                //BaseEntity.RigidBody.linearVelocity = Vector3.zero;
                BaseEntity.RigidBody.AddForce(-Physics.gravity * BaseEntity.RigidBody.mass, ForceMode.Force);
                Debug.LogWarning(BaseEntity.RigidBody.linearVelocity);
            });
        }

        public void Cancel()
        {
            _tween.Kill();
            Debug.LogWarning("EXIT SHOW!");
        }

        public BaseEntity BaseEntity
        {
            get
            {
                if (_baseEntity) return _baseEntity;
                _baseEntity = GetComponent<BaseEntity>();
                return _baseEntity;
            }
        }
    }
}