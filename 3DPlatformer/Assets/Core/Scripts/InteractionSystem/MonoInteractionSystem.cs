using Core.Scripts.InteractionSystem.Structs;
using UnityEngine;

public class MonoInteractionSystem : MonoBehaviour
{
    [SerializeField] private float rayLenght;
    [SerializeField] private LayerMask groundMask;

    private Transform _transform;

    private GroundStruct GetGroundStruct()
    {
        var position = _transform.position;
        
        var ray = new Ray(position, -_transform.up);

        var under = Physics.Raycast(ray, rayLenght, groundMask);

        ray = new Ray(position - _transform.forward, -transform.up);
        var front = Physics.Raycast(ray, rayLenght, groundMask);
        
        ray = new Ray(position + _transform.forward, -transform.up);
        var back = Physics.Raycast(ray, rayLenght, groundMask);
        
        return new GroundStruct(under, front,back);
    }

    private void Awake()
    {
        _transform = transform;
    }

    public GroundStruct IsGround => GetGroundStruct();
}