using System;
using System.Linq;
using Core.Scripts.Entity;
using Core.Scripts.Entity.Interfaces;
using Core.Scripts.StatesMachine;
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

    private Transform _transform;
    private Material _dissolveMaterial;
    private IAuraCharger _currentAuraChanger;
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

    public void TeleportEntity(IAuraCharger auraCharger, Vector3 position)
    {
        _currentAuraChanger = auraCharger;
        var entity = auraCharger.BaseEntity;

        foreach (var portalParticle in _portalParticles)
        {
            portalParticle.Play();
        }

        entity.enabled = false;
        entity.RigidBody.isKinematic = true;
        entity.transform.SetParent(transform);
        entity.transform.localPosition = Vector3.zero;
        entity.transform.localScale = Vector3.one / 3f;

        _transform.localScale = Vector3.zero;
        _transform.position = position;

        portalTeleportSound.Play(Random.Range(0.9f, 1.1f));

        _transform.DOScale(targetScale, 2).SetEase(Ease.OutBack).SetLink(gameObject).OnComplete(() =>
        {
            entity.transform.SetParent(null);
            entity.RigidBody.isKinematic = false;
            entity.enabled = true;
            entity.Animator.speed = 1;

            DOTween.To(t => meshRenderer.material.SetFloat(DissolveAmount, t), 0, 1, 1).SetLink(gameObject);
        });

        _transform.DOMoveY(_transform.position.y + 2, 1.5f).SetLink(gameObject);
    }

    public void AppearPortalAtPlayer()
    {
        if (!_currentAuraChanger?.BaseEntity) throw new Exception("Player not saved");

        foreach (var portalParticle in _portalParticles)
        {
            portalParticle.Play();
        }

        portalAppearSound.Play(Random.Range(0.9f, 1.1f));
        _transform.localScale = Vector3.zero;
        _transform.position = _currentAuraChanger.BaseEntity.transform.position;

        _currentAuraChanger.Show(_transform.position.y + 2, 2, true);

        _transform.DOScale(targetScale, 1).SetEase(Ease.OutBack).SetLink(gameObject);
        _transform.DOMoveY(_transform.position.y + 2, 1.5f).SetLink(gameObject);

        DOTween.To(t => meshRenderer.material.SetFloat(DissolveAmount, t), 1, 0, 4).SetLink(gameObject);
    }
}