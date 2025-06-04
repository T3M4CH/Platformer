namespace Core.Scripts.Entity.Interfaces
{
    public interface IAuraCharger
    {
        void Show(float targetY, float time, bool invincible);
        void Cancel();
        BaseEntity BaseEntity { get; }
    }
}