namespace Core.Scripts.Effects.Interfaces
{
    public interface IEffectService
    {
        MonoEffect GetEffect(EVfxType vfxType, bool returnActive = false);
    }
}