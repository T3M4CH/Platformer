using Core.Scripts.StatesMachine;
using UnityEngine;

namespace Core.Scripts.Entity
{
    public abstract class DefaultEntity : BaseEntity
    {
        public override void UpdateHp(float health, float maxHealth)
        {
            HealthBarManager.UpdateHp(health, maxHealth, transform, Vector3.up);
        }
    }
}