using Core.Scripts.Cameras;

public interface ICameraService
{
    void SetFovInstantly(float value);
    void SetFov(float value, float duration);
    BaseVirtualCamera GetActiveCamera();
    T GetCamera<T>() where T : BaseVirtualCamera;
    T ChangeActiveCamera<T>() where T : BaseVirtualCamera;
    BaseVirtualCamera ChangeActiveCamera(int id);
}
