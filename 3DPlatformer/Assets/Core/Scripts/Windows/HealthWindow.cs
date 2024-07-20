using System.Collections.Generic;
using Core.Scripts.Entity;
using UnityEngine;

namespace Core.Scripts.Windows
{
    public class HealthWindow : UIWindow
    {
        [SerializeField] private Transform parent;
        [SerializeField] private BossHealthBar healthbarPrefab;

        private readonly Dictionary<BossEntity, BossHealthBar> _healthBars = new();

        public void UpdateHealth(float health, float maxHealth, BossEntity entity)
        {
            if (!_healthBars.ContainsKey(entity))
            {
                _healthBars.Add(entity, Instantiate(healthbarPrefab, parent));
            }

            var healthBar = _healthBars[entity];

            if (health < 0)
            {
                healthBar.gameObject.SetActive(false);
                return;
            }

            healthBar.FillImage.fillAmount = health / maxHealth;
            healthBar.gameObject.SetActive(true);
        }
    }
}