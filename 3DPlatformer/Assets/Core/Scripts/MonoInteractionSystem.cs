using UnityEngine;

public class MonoInteractionSystem : MonoBehaviour
{
    [SerializeField] private float rayLenght;
    [SerializeField] private LayerMask groundMask;

    private Transform _transform;

    private void Update()
    {
        var ray = new Ray(_transform.position, -_transform.up);

        IsGround = Physics.Raycast(ray, out var hitInfo, rayLenght, groundMask);
    }

    private void Start()
    {
        _transform = transform;
    }
    
    public bool IsGround { get; private set; }
}