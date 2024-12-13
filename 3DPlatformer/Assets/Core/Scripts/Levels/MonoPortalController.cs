using System;
using System.Linq;
using Core.Scripts.Entity;
using Core.Sounds.Scripts;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class MonoPortalController : MonoBehaviour
{
    [SerializeField] private float targetScale;
    [SerializeField] private Material dissolvePrefab;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private SoundAsset portalAppearSound;
    [SerializeField] private SoundAsset portalTeleportSound;
    [SerializeField] private AnimationCurve animationCurve;

    private BaseEntity _entity;
    private Transform _transform;
    private Material _dissolveMaterial;
    private ParticleSystem[] _portalParticles;
    private static readonly int DissolveAmount = Shader.PropertyToID("_DissolveAmount");

    private void Awake()
    {
        _transform = transform;

        _dissolveMaterial = new Material(dissolvePrefab);

        var texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.black);
        texture.Apply();

        _dissolveMaterial.SetTexture("_MainTex", texture);
        meshRenderer.materials = new[] { _dissolveMaterial };

        _portalParticles = GetComponentsInChildren<ParticleSystem>();
    }

    public void TeleportEntity(BaseEntity entity, Vector3 position)
    {
        Debug.LogWarning("Teleport");

        _entity = entity;

        foreach (var portalParticle in _portalParticles)
        {
            portalParticle.Play();
        }

        _entity.enabled = false;
        _entity.RigidBody.isKinematic = true;
        _entity.transform.SetParent(transform);
        _entity.transform.localPosition = Vector3.zero;
        _entity.transform.localScale = Vector3.one / 3f;

        _transform.localScale = Vector3.zero;
        _transform.position = position;

        portalTeleportSound.Play(Random.Range(0.9f, 1.1f));

        _transform.DOScale(targetScale, 2).SetEase(Ease.OutBack).SetLink(gameObject).OnComplete(() =>
        {
            _entity.transform.SetParent(null);
            _entity.RigidBody.isKinematic = false;
            _entity.enabled = true;
            _entity.Animator.speed = 1;
            _entity.Animator.CrossFade("Jump", 0, 0, 0);

            DOTween.To(t => meshRenderer.material.SetFloat(DissolveAmount, t), 0, 1, 1).SetLink(gameObject);
        });

        _transform.DOMoveY(_transform.position.y + 2, 1.5f).SetLink(gameObject);
    }

    public void AppearPortalAtPlayer()
    {
        if (!_entity) throw new Exception("Player not saved");

        Debug.LogWarning("Appear");

        foreach (var portalParticle in _portalParticles)
        {
            portalParticle.Play();
        }

        portalAppearSound.Play(Random.Range(0.9f, 1.1f));
        _transform.localScale = Vector3.zero;
        _transform.position = _entity.transform.position;

        _entity.enabled = false;
        _entity.RigidBody.isKinematic = true;
        //_entity.transform.SetParent(transform);
        //_entity.transform.localPosition = Vector3.zero;
        _entity.transform.DORotate(new Vector3(0, 180, 0), 1f);
        _entity.Animator.CrossFade("Jump", 0, 0);

        _transform.DOScale(targetScale, 1).SetEase(Ease.OutBack).SetLink(gameObject);
        _transform.DOMoveY(_transform.position.y + 2, 1.5f).SetLink(gameObject);


        var tween = _entity.transform.DOMoveY(_transform.position.y + 2, 2).SetLink(gameObject);
        tween.OnUpdate(() =>
        {
            _entity.Animator.speed = animationCurve.Evaluate(tween.position / tween.Duration());
            //Debug.LogWarning(tween.position / tween.Duration());
        });

        DOTween.To(t => meshRenderer.material.SetFloat(DissolveAmount, t), 1, 0, 4).SetLink(gameObject);
    }
}