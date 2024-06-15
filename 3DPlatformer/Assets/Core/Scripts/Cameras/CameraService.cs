using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Core.Scripts.Cameras;
using Cysharp.Threading.Tasks;
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
    private CancellationTokenSource _tokenSource;

    private readonly BaseVirtualCamera[] _cameras;

    public void SetFovInstantly(float value)
    {
        _tokenSource?.Cancel();
        _tokenSource?.Dispose();
        
        _tokenSource = null;

        _activeCamera.Camera.m_Lens.FieldOfView = value;

        FOV = value;
    }

    public void SetFov(float value, float duration)
    {
        _tokenSource?.Cancel();
        _tokenSource?.Dispose();
        
        _tokenSource = null;

        _tokenSource = new CancellationTokenSource();
        var token = _tokenSource.Token;
        
        PerformChangingFov(value, duration, token);
    }

    private async void PerformChangingFov(float value, float duration, CancellationToken token)
    {
        var delay = duration;
        var initialFov = FOV;

        while (delay > 0)
        {
            await UniTask.Yield(PlayerLoopTiming.LastUpdate);

            if (token.IsCancellationRequested)
            {
                break;
            }

            delay -= Time.deltaTime;

            var newFov = Mathf.Lerp(value, initialFov, delay / duration);

            _activeCamera.Camera.m_Lens.FieldOfView = newFov;

            FOV = newFov;
        }
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