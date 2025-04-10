using System;
using System.Collections;
using System.Linq;
using Core.Scripts.Cameras;
using UnityEngine;
using Object = UnityEngine.Object;

public class CameraService : ICameraService
{
    public CameraService(SerializableCameraSettings cameraSettings)
    {
        var cameras = cameraSettings.Cameras;
        var parent = new GameObject("CameraController").transform;

        Object.Instantiate(cameraSettings.CinemachineBrain, parent);
        _cameras = Enumerable.Range(0, cameras.Length).Select(id =>
        {
            var cameraInstance = cameras[id];
            var camera = Object.Instantiate(cameraInstance, parent);
            camera.name = cameraInstance.name;
            return camera;
        }).ToArray();

        _activeCamera = _cameras[0];
    }

    private BaseVirtualCamera _activeCamera;
    private Coroutine _fovCoroutine;
    private readonly BaseVirtualCamera[] _cameras;

    public void SetFovInstantly(float value)
    {
        if (_fovCoroutine != null)
        {
            CoroutineRunner.Instance.StopConcreteCoroutine(_fovCoroutine);
            _fovCoroutine = null;
        }

        _activeCamera.Camera.m_Lens.FieldOfView = value;
        FOV = value;
    }

    public void SetFov(float value, float duration)
    {
        if (_fovCoroutine != null)
        {
            CoroutineRunner.Instance.StopConcreteCoroutine(_fovCoroutine);
        }

        _fovCoroutine = CoroutineRunner.Instance.RunCoroutine(PerformChangingFov(value, duration));
    }

    private IEnumerator PerformChangingFov(float value, float duration)
    {
        var elapsed = 0f;
        var initialFov = FOV;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float newFov = Mathf.Lerp(initialFov, value, elapsed / duration);
            _activeCamera.Camera.m_Lens.FieldOfView = newFov;
            FOV = newFov;
            yield return null;
        }

        _activeCamera.Camera.m_Lens.FieldOfView = value;
        FOV = value;
    }

    public T GetCamera<T>() where T : BaseVirtualCamera
    {
        var camera = _cameras.FirstOrDefault(camera => camera is T);
        if (camera == null)
        {
            throw new Exception("Camera was not presented");
        }

        return camera as T;
    }

    public T ChangeActiveCamera<T>() where T : BaseVirtualCamera
    {
        var camera = _cameras.FirstOrDefault(camera => camera is T);
        if (camera == null)
        {
            throw new Exception("Camera was not presented");
        }

        _activeCamera.Camera.Priority = 0;
        _activeCamera = camera;
        _activeCamera.Camera.Priority = 1;
        FOV = _activeCamera.Camera.m_Lens.FieldOfView;

        return camera as T;
    }

    public BaseVirtualCamera ChangeActiveCamera(int id)
    {
        _activeCamera.Camera.Priority = 0;
        _activeCamera = _cameras[id];
        _activeCamera.Camera.Priority = 1;
        FOV = _activeCamera.Camera.m_Lens.FieldOfView;
        return _activeCamera;
    }

    public BaseVirtualCamera GetActiveCamera() => _activeCamera;
    public float FOV { get; private set; }
}