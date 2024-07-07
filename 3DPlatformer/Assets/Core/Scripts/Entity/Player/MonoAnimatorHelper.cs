using System;
using UnityEngine;

public class MonoAnimatorHelper : MonoBehaviour
{
    public event Action OnLand = () => { };
    public event Action OnStand = () => { };
    public event Action OnAttacked = () => { };
    public event Action OnDamageExit = () => { };
    public event Action<bool> OnAbleAttack = _ => { };
    public event Action OnAttackExitEvent = () => { };
    
    public void PerformAttackEvent()
    {
        OnAttacked.Invoke();
    }

    public void AbleAttackEvent()
    {
        OnAbleAttack.Invoke(true);    
    }
    
    public void DisableAttackEvent()
    {
        OnAbleAttack.Invoke(false);    
    }

    public void AttackExitEvent()
    {
        OnAttackExitEvent.Invoke();
    }

    public void PerformLandEvent()
    {
        OnLand.Invoke();
    }

    public void DamageExitEvent()
    {
        OnDamageExit.Invoke();
    }

    public void PerformStandEvent()
    {
        OnStand.Invoke();
    }
}
