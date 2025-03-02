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

        public void PerformMoveBowAlongBody(float ratio, Vector3 target)
        {
            var relationPosition = (target - BowTransform.position).normalized;
            var lookRotation = Quaternion.LookRotation(relationPosition);
            var angleY = Mathf.Atan2(relationPosition.y, relationPosition.x) * Mathf.Rad2Deg;

            var radius = Mathf.Lerp(0.3f, targetRadius, ratio);
            //var _center = Vector3.Lerp(new Vector3(0, -0.5f, 0), center, ratio);

            //var startRotation = transform.TransformPoint(new Vector3(0, -1, 1) * 3);

            BowTransform.rotation = lookRotation;
            BowTransform.localPosition = Vector3.Lerp(handPosition, center + new Vector3(0f, Mathf.Sin(angleY * Mathf.Deg2Rad) * radius, -Mathf.Cos(angleY * Mathf.Deg2Rad) * radius), ratio);
        }

        public void Shot()
        {
            var target = BowTransform.TransformPoint(Vector3.forward * 3f);

            var relativePosition = target - BowTransform.position;
            relativePosition.z = 0;

            Instantiate(bowArrow, BowTransform.position, Quaternion.LookRotation(relativePosition)).ArrowOwnwer = transform;
        }

        private void Start()
        {
            handPosition = BowTransform.localPosition;

            BowInHands = false;
        }

        public bool BowInHands
        {
            get => _bowInHands;
            set
            {
                _bowInHands = value;

                BowTransform.DOScale(value ? 1 : 0, duration / 2).From(value ? 0 : 1).SetEase(ease);

                DOTween.To(t => bowRig.weight = t, value ? 0 : 1, value ? 1 : 0, duration).SetEase(ease);

                if (value)
                {
                    exclamationMark.SetActive(true);
                }
            }
        }

        [field: SerializeField] public Transform BowTransform { get; private set; }
    }
}