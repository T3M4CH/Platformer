using System;

namespace Core.Scripts.Windows.Interfaces
{
    public interface IGameSettings
    {
        event Action OnChangeMusic;
        event Action OnChangeSounds;
        event Action OnChangeHaptic;
        
        bool Haptic { get; }
        bool Sound { get; }
        bool Music { get; }
    }
}