using System;
using System.Numerics;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Core.Scripts.Bow
{
    public class BowController : MonoBehaviour
    {
        [SerializeField] private Rig bowRig;
        [SerializeField] private MonoBowArrow bowArrow;
        [SerializeField] private GameObject exclamationMark;
        [SerializeField] private Vector3 startPosition;
        [SerializeField] private Vector3 handPosition;
        [SerializeField] private float duration;
        [SerializeField] private Ease ease;

        [SerializeField] private Transform testBowTarget;
        [SerializeField] private Vector3 center;
        [SerializeField] private float targetRadius;
        [SerializeField] private Vector3 startEuler;

        private bool _bowInHands = true;

        public void PerformBowAnim()
        {
            //BowTransform.DOScale(1, duration).From(0).SetEase(ease);
            //BowTransform.DOLocalMove(handPosition, duration).From(startPosition).SetEase(ease);
            DOTween.To(t => bowRig.weight = t, 0, 1, duration).SetEase(ease);

            exclamationMark.SetActive(true);
        }

        public void PerformMoveBowAlongBody(float ratio, Vector3 target)
        {
            //TODO: Тут в направлении чела надо sin cos
            //Анимка мб
            //BowTransform.localPosition = Vector3.Lerp(handPosition, GetEndPosition(), ratio);


            var relationPosition = (testBowTarget.position - BowTransform.position).normalized;
            var lookRotation = Quaternion.LookRotation(relationPosition);
            var angleY = Mathf.Atan2(relationPosition.y, relationPosition.x) * Mathf.Rad2Deg;

            var radius = Mathf.Lerp(0.3f, targetRadius, ratio);
            var _center = Vector3.Lerp(new Vector3(0, -0.5f, 0), center, ratio);

            BowTransform.rotation = Quaternion.Lerp(Quaternion.Euler(startEuler), lookRotation * Quaternion.Euler(-90, 0, 0), ratio);
            Debug.LogWarning(angleY);
            Debug.LogWarning(new Vector3(0, Mathf.Sin(angleY * Mathf.Deg2Rad), Mathf.Cos(angleY * Mathf.Deg2Rad)));
            BowTransform.localPosition = Vector3.Lerp(handPosition, center + new Vector3(0.2f, Mathf.Sin(angleY * Mathf.Deg2Rad) * radius, -Mathf.Cos(angleY * Mathf.Deg2Rad) * radius), ratio);
        }

        public void Shot()
        {
            var target = BowTransform.TransformPoint(Vector3.down * 3f);

            var relativePosition = target - BowTransform.position;
            relativePosition.z = 0;

            Instantiate(bowArrow, BowTransform.position, Quaternion.LookRotation(relativePosition)).ArrowOwnwer = transform;
        }

        // private void OnAnimatorIK(int layerIndex)
        // {
        //     Debug.LogWarning(transform.name);
        //     if (transform.name == "Boss(Clone)")
        //     {
        //         Debug.LogWarning("Всё ок?" + layerIndex);
        //     }
        //
        //     if (animator == null) return;
        //
        //     if (!BowInHands || transform.name != "Boss(Clone)") return;
        //     Debug.LogWarning("!!" + layerIndex);
        //
        //     animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        //     animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
        //     animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandPosition.position);
        //     animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandPosition.position);
        //     
        //     animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
        //     animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
        //     animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandPosition.rotation);
        //     animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandPosition.rotation);
        // }

        private void Start()
        {
            handPosition = BowTransform.localPosition;
        }

        public bool BowInHands
        {
            get => _bowInHands;
            set
            {
                _bowInHands = value;

                BowTransform.DOScale(value ? 1 : 0, duration / 2).From(value ? 0 : 1).SetEase(ease);
            }
        }

        [field: SerializeField] public Transform BowTransform { get; private set; }
    }
}