using System;
using DG.Tweening;
using UnityEngine;

namespace Core.Scripts.Bow
{
    public class BowController : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private MonoBowArrow bowArrow;
        [SerializeField] private Transform leftHandPosition;
        [SerializeField] private Transform rightHandPosition;
        [SerializeField] private GameObject exclamationMark;
        [SerializeField] private Vector3 endBodyPosition;
        [SerializeField] private Vector3 startPosition;
        [SerializeField] private Vector3 handPosition;
        [SerializeField] private float duration;
        [SerializeField] private Ease ease;

        private bool _bowInHands = true;

        public void PerformBowAnim()
        {
            BowTransform.DOScale(1, duration).From(0).SetEase(ease);
            BowTransform.DOLocalMove(handPosition, duration).From(startPosition).SetEase(ease);

            exclamationMark.SetActive(true);
        }

        public void PerformMoveBowAlongBody(float ratio)
        {
            BowTransform.localPosition = Vector3.Lerp(handPosition, endBodyPosition, ratio);
        }

        public void Shot()
        {
            var target = BowTransform.TransformPoint(Vector3.forward * 3f);
            Debug.Break();
            var relativePosition = target - BowTransform.position;
            relativePosition.z = 0;

            Instantiate(bowArrow, BowTransform.position, Quaternion.LookRotation(relativePosition)).ArrowOwnwer = transform;
        }

        private void OnAnimatorIK(int layerIndex)
        {
            Debug.LogWarning(transform.name);
            if (transform.name == "Boss(Clone)")
            {
                Debug.LogWarning("Всё ок?" + layerIndex);
            }

            if (animator == null) return;

            if (!BowInHands || transform.name != "Boss(Clone)") return;
            Debug.LogWarning("!!" + layerIndex);

            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandPosition.position);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandPosition.position);

            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
            animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandPosition.rotation);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandPosition.rotation);
        }

        private void Start()
        {
            handPosition = BowTransform.localPosition;
            BowTransform.gameObject.SetActive(false);
        }

        public bool BowInHands
        {
            get => _bowInHands;
            set
            {
                _bowInHands = value;
                BowTransform.gameObject.SetActive(value);
            }
        }

        [field: SerializeField] public Transform BowTransform { get; private set; }
    }
}