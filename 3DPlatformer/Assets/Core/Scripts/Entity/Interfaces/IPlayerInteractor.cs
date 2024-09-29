using Core.Scripts.StatesMachine;

namespace Core.Scripts.Entity.Interfaces
{
    public interface IPlayerInteractor
    {
        void ExecuteExtraJump();
        EntityState CurrentEntityState { get; }
    }
}