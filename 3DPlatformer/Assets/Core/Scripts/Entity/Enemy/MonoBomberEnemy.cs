using Core.Scripts.Entity.Managers.Interfaces;
using Core.Scripts.StatesMachine;
using Core.Scripts.Entity;
using UnityEngine;

public class MonoBomberEnemy : DefaultEntity
{
    private IPlayerService _playerService;

    public void Initialize(IPlayerService playerService)
    {
        _playerService = playerService;
    }

    private void Start()
    {
        StateMachine = new EntityStateMachine();
        StateMachine.AddState(new ThrowBombsEntityState(StateMachine, this, _playerService.PlayerInstance));
        StateMachine.SetState<ThrowBombsEntityState>();
    }

    private void Update()
    {
        StateMachine.Update();
    }

    public override EntityStateMachine StateMachine { get; protected set; }
    [field: SerializeField] public float Delay { get; private set; }
    [field: SerializeField] public Rigidbody Bomb { get; private set; }
}
