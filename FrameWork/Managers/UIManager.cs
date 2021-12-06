using System;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using UnityEngine.SceneManagement;

namespace Framework
{
    public partial class UIManager : MgrBase, ISingleton
    {
        private static UIManager mInstance;
        private Transform canvasTrans;
        public UIPanelBase CurrentPanel;

　　     public Transform CanvasTrans
        {
            get
            {
                if (canvasTrans == null)
                {
                    canvasTrans = GameObject.Find("Canvas").transform;
                }
                return canvasTrans;
            }
        }

        private Dictionary<PanelType, UIPanelInfo> allPanelInfoDict = new Dictionary<PanelType, UIPanelInfo>();
        private Dictionary<PanelType, UIPanelBase> localPanelDict = new Dictionary<PanelType, UIPanelBase>();
        private Stack<UIPanelBase> panelDisplayedStack = new Stack<UIPanelBase>();

        void ISingleton.OnSingletonInit()
        {
            ParsePanelTypeJson();
        }

        public static UIManager Instance
        {
            get
            {
                if (!mInstance)
                {
                    var uiRoot = UIRoot.Instance;
                    Debug.Log("currentUIRoot:" + uiRoot);
                    mInstance = MonoSingletonProperty<UIManager>.Instance;
                    mInstance.transform.SetParent(UIRoot.Instance.Manager);
                }
                return mInstance;
            }
        }

        private void ParsePanelTypeJson()
        {
            // 用Resources.Load()从Json文件中读取text数据
            string jeson = Resources.Load<TextAsset>("Data/PanelType").text;
            Debug.Log("textAsset:" + jeson);
            //将Json格式的text数据转换成List<> 数据
            UIPanelInfos panelInfos = JsonUtility.FromJson<UIPanelInfos>(jeson);

            // 把List的存储到Dictionary中
            foreach (var element in panelInfos.panelInfoList)
            {
                allPanelInfoDict.Add(element.panelType, element);
                Debug.Log("panelType:" + element.panelType + "info:" + element);
            }
        }

        public override int ManagerId
        {
			get { return MgrID.UI; }
        }

        /// <summary>
        /// 获取面板脚本
        /// </summary>
        /// <returns></returns>
        public UIPanelBase GetPanel(PanelType panelType)
        {
            UIPanelBase panel;
            localPanelDict.TryGetValue(panelType, out panel);

            if (panel == null)
            {
                UIPanelInfo panelInfo;
                allPanelInfoDict.TryGetValue(panelType, out panelInfo);

                if (panelInfo == null)
                {
                    Debug.LogError("无此面板信息:" + panelType.ToString());
                    return null;
                }
                else
                {
                    GameObject panelObj = InstantcePanel(panelInfo.path);
                    UIPanelBase panelBase = panelObj.GetComponent<UIPanelBase>();
                    panelBase.Transform = panelObj.transform;

                    localPanelDict.Add(panelType, panelBase);
                    return panelBase;
                }
            }
            else
            {
                return panel;
            }
        }

        public void OpenPanel(PanelType panelType, UILevel level = UILevel.Common)
        {
            UIPanelBase panelBase = GetPanel(panelType);
            PushPanel(panelBase);
            UIRoot.Instance.SetLevelOfPanel(level, panelBase);

            panelBase.Open();
        }

        public void ClosePanel(bool IsRemove = false)
        {
            UIPanelBase panelBase = PopPanel();

            if (IsRemove)
            {
                RemovePanel(panelBase);
            }
            panelBase.Close();
        }

        private void RemovePanel(UIPanelBase panelBase)
        {
            //删除面板
            panelBase.transform.SetParent(null);
            SceneManager.MoveGameObjectToScene(panelBase.gameObject, SceneManager.GetActiveScene());
        }

        private void PushPanel(UIPanelBase panelBase)
        {
            // 进行生命周期的调用
            panelBase.Init(panelBase.mUIData);
            // 如果之前的栈不为空，则之前的栈顶panel需要调用Pause()
            if (panelDisplayedStack.Count > 0)
            {
                // Debug.Log("peek on pause: " + panelDisplayedStack.Peek());
                panelDisplayedStack.Peek().Pause();
            }

            panelDisplayedStack.Push(panelBase);
            // Debug.Log("Push " + panel + " 进入Stack");
            Debug.Log("PushPanel:" + panelBase);
        }

        private UIPanelBase PopPanel()
        {
            if (panelDisplayedStack.Count > 0)
            {
                UIPanelBase panel = panelDisplayedStack.Pop();

                // Resume新的栈顶panel
                if (panelDisplayedStack.Count > 0)
                {
                    panelDisplayedStack.Peek().Resume();
                }

                return panel;
            }
            else
            {
                Debug.LogError("面板栈为空!");
                return null;
            }
        }

        /// <summary>
        /// 实例化面板
        /// </summary>
        /// <param name="panelType"></param>
        /// <returns></returns>
        private GameObject InstantcePanel(string path)
        {
           return PanelLoader.Instance.LoadPanelPrefab(path);
        }

        public void Reset()
        {
            localPanelDict.Clear();
            panelDisplayedStack.Clear();
         }
    }
}
