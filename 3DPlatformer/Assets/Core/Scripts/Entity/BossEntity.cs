using Core.Scripts.Entity.Interfaces;
using Core.Scripts.Entity.Managers.Interfaces;

namespace Core.Scripts.Entity
{
    public abstract class BossEntity : BaseEntity, IPlayerConsumer
    {
        protected IPlayerService PlayerService;

        public void Initialize(IPlayerService playerService)
        {
            PlayerService = playerService;
        }

        public override void UpdateHp(float health, float maxHealth)
        {
            HealthBarManager.UpdateHp(health, maxHealth, this);
        }
    }
}