using System;
using UnityEngine;

public class MonoAnimatorHelper : MonoBehaviour
{
    public event Action OnLand = () => { };
    public event Action OnStand = () => { };
    public event Action OnStanding = () => { };
    public event Action OnAttacked = () => { };
    
    public void PerformAttackEvent()
    {
        OnAttacked.Invoke();
    }

    public void PerformLandEvent()
    {
        OnLand.Invoke();
    }

    public void PerformStandingEvent()
    {
        OnStanding.Invoke();
    }

    public void PerformStandEvent()
    {
        OnStand.Invoke();
    }
}
