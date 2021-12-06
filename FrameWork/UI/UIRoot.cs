using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
    [MonoSingletonPath("UIRoot")]
    public class UIRoot : MonoSingleton<UIRoot>
    {
        public Camera UICamera;
        public Canvas Canvas;
        public CanvasScaler CanvasScaler;
        public GraphicRaycaster GraphicRaycaster;

        public RectTransform Bg;
        public RectTransform Common;
        public RectTransform PopUI;
        public RectTransform CanvasPanel;
        public RectTransform Manager;

        private new static UIRoot mInstance;

        public new static UIRoot Instance
        {
            get
            {
                if (!mInstance)
                {
                    mInstance = FindObjectOfType<UIRoot>();
                }

                if (!mInstance)
                {
                    Instantiate(Resources.Load<GameObject>("Prefabs/UIRoot"));
                    mInstance = MonoSingletonProperty<UIRoot>.Instance;
                    mInstance.name = "UIRoot";
                    DontDestroyOnLoad(mInstance);
                }

                return mInstance;
            }
        }


        public Camera Camera
        {
            get { return UICamera; }
        }

        public void SetResolution(int width, int height, float matchOnWidthOrHeight)
        {
            CanvasScaler.referenceResolution = new Vector2(width, height);
            CanvasScaler.matchWidthOrHeight = matchOnWidthOrHeight;
        }

        public Vector2 GetResolution()
        {
            return CanvasScaler.referenceResolution;
        }

        public float GetMatchOrWidthOrHeight()
        {
            return CanvasScaler.matchWidthOrHeight;
        }

        public void SetLevelOfPanel(UILevel level, IPanel panel)
        {
            var canvas = panel.Transform.GetComponent<Canvas>();

            if (canvas)
            {
                panel.Transform.SetParent(CanvasPanel);
                SetDefaultSizeOfPanel(panel);
            }
            else
            {
                switch (level)
                {
                    case UILevel.Bg:
                        panel.Transform.SetParent(Bg);
                        SetDefaultSizeOfPanel(panel);
                        break;
                    case UILevel.Common:
                        panel.Transform.SetParent(Common);
                        SetDefaultSizeOfPanel(panel);
                        break;
                    case UILevel.PopUI:
                        panel.Transform.SetParent(PopUI);
                        SetDefaultSizeOfPanel(panel);
                        break;
                }
            }
        }

        public virtual void SetDefaultSizeOfPanel(IPanel panel)
        {
            var panelRectTrans = panel.Transform as RectTransform;

            panelRectTrans.offsetMin = Vector2.zero;
            panelRectTrans.offsetMax = Vector2.zero;
            panelRectTrans.anchoredPosition3D = Vector3.zero;
            panelRectTrans.anchorMin = Vector2.zero;
            panelRectTrans.anchorMax = Vector2.one;
            panelRectTrans.transform.localScale = Vector3.one;
        }
    }
}
