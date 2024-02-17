using System;

namespace Core.Scripts.Levels.Interfaces
{
    public interface ILevelService
    {
        void CompleteLevel();
        event Action<MonoLevelBase> OnLevelChanged;
    }
}