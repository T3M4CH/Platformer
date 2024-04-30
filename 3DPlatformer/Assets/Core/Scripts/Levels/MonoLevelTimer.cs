namespace Core.Scripts.Levels
{
    public class MonoLevelTimer : MonoLevelBase
    {
        private TimerWindow _timerWindow;

        private void Start()
        {
            _timerWindow = WindowManager.GetWindow<TimerWindow>();
            _timerWindow.SetTimer(40f);
            _timerWindow.OnComplete += Complete;
        }

        private void Complete()
        {
            LevelService.CompleteLevel();
        }

        private void OnDestroy()
        {
            _timerWindow.OnComplete -= Complete;
        }
    }
}