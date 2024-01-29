using UnityEngine.UI;
using UnityEngine;

namespace Core.Scripts.Windows
{
    public abstract class UIWindow : MonoBehaviour
    {
        protected WindowManager WindowManager { get; private set; }

        public void Initialize(WindowManager windowManager)
        {
            WindowManager = windowManager;
        }

        public virtual void Show()
        {
            if (BackGround)
            {
                BackGround.enabled = true;
            }

            if (Content)
            {
                Content.localScale = Vector3.one;
            }
        }

        public virtual void Hide()
        {
            if (BackGround)
            {
                BackGround.enabled = false;
            }

            if (Content)
            {
                Content.localScale = Vector3.zero;
            }
        }
        
        
        [field: SerializeField] public Image BackGround { get; private set; }
        [field: SerializeField] public RectTransform Content { get; private set; }
    }
}