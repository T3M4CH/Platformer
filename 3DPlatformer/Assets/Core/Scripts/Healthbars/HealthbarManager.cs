using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.Scripts.Healthbars
{
    public class HealthbarManager
    {
        private readonly MonoHealthbar _healthBarPrefab;
        private readonly RectTransform _rectParent;
        private readonly Camera _camera;
        private readonly Dictionary<Transform, MonoHealthbar> _healthbars = new();

        public HealthbarManager(HealthbarManagerSettings healthbarManagerSettings)
        {
            _rectParent = healthbarManagerSettings.Canvas;
            _healthBarPrefab = healthbarManagerSettings.HealthBarPrefab;

            _camera = Camera.main;
        }

        public void UpdateHp(float currentHp, float maxHealth, Transform transform, Vector3 offset)
        {
            if (!_healthbars.ContainsKey(transform)) _healthbars.Add(transform, CreateHealthBar());

            var healthBar = _healthbars[transform];

            healthBar.FillImage.fillAmount = currentHp / maxHealth;

            if (currentHp < 0)
            {
                healthBar.gameObject.SetActive(false);
                return;
            }

            var token = transform.GetCancellationTokenOnDestroy();

            UniTask.Void(async () =>
            {
                var currentTime = 0f;
                while (currentTime < 2f)
                {
                    currentTime += Time.deltaTime;
                    await UniTask.Yield(PlayerLoopTiming.Update, token).SuppressCancellationThrow();

                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    healthBar.CanvasGroup.alpha = Mathf.InverseLerp(0, 0.3f, currentTime);
                    healthBar.transform.localPosition = (transform.position + offset).WorldToScreenPosition(_camera, _rectParent);
                }

                if (healthBar.CanvasGroup)
                {
                    healthBar.CanvasGroup.alpha = 0;
                }
            });
        }

        private MonoHealthbar CreateHealthBar()
        {
            var instance = Object.Instantiate(_healthBarPrefab, _rectParent);

            return instance;
        }
    }
}