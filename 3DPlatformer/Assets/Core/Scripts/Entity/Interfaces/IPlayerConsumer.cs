using Core.Scripts.Entity.Managers.Interfaces;

namespace Core.Scripts.Entity.Interfaces
{
    public interface IPlayerConsumer
    {
        public void Initialize(IPlayerService playerService);
    }
}