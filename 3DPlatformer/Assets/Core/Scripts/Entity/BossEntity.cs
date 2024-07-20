namespace Core.Scripts.Entity
{
    public abstract class BossEntity : BaseEntity
    {
        protected virtual void Start()
        {
            UpdateHp(1, 1);
        }

        public override void UpdateHp(float health, float maxHealth)
        {
            HealthBarManager.UpdateHp(health, maxHealth, this);
        }
    }
}