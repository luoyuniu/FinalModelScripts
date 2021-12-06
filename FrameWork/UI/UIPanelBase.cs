namespace Framework
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

	public interface IUIData
    {

    }

	public class UIPanelData : IUIData
    {
		protected UIPanelBase mPanelBase;
    }

    public class UIPanelBase : MonoBehaviour,IPanel
	{
        public Transform Transform { get; set; }
		public UILevel Level { get; set; }
		public PanelState State { get ; set ; }
		public IUIData mUIData { get; set; }

		//通过里式转换原则 来存储所有的控
		private Dictionary<string, List<UIBehaviour>> controlDic = new Dictionary<string, List<UIBehaviour>>();


		private void GetAllChildrensControl()
        {
			FindChildrenControl<Button>();
			FindChildrenControl<Image>();
			FindChildrenControl<Toggle>();
			FindChildrenControl<Text>();
			FindChildrenControl<Slider>();
			FindChildrenControl<ScrollRect>();
		}

		/// <summary>
		/// 找到子对象的对应控件
		/// </summary>
		/// <typeparam name="T"></typeparam>
		private void FindChildrenControl<T>() where T : UIBehaviour
		{
			T[] controls = GetComponentsInChildren<T>();
			string gobjName;
			for (int i = 0; i < controls.Length; i++)
			{
				gobjName = controls[i].gameObject.name;

				if (controlDic.ContainsKey(gobjName))
				{
					controlDic[gobjName].Add(controls[i]);
				}
				else
				{
					controlDic.Add(gobjName, new List<UIBehaviour>() { controls[i] });
				}
			}
		}

		/// <summary>
		/// 得到对应名字的对应控件脚本
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="controlName"></param>
		/// <returns></returns>
		protected T GetControl<T>(string controlName) where T : UIBehaviour
		{
			if (controlDic.ContainsKey(controlName))
			{
				for (int i = 0; i < controlDic[controlName].Count; i++)
				{
					if (controlDic[controlName][i] is T)
					{
						return controlDic[controlName][i] as T;
					}
				}
			}
			return null;
		}

        public void Init(IUIData uidata)
        {
			mUIData = uidata;
			GetAllChildrensControl();
			Debug.Log("Init");
        }

        public void Open()
		{
			State = PanelState.Opening;
			Debug.Log("Open");
		}

        public void Show()
		{
			Debug.Log("Show");
		}

        public void Pause()
		{
			State = PanelState.Pause;
			Debug.Log("Pause");
		}


		public void Resume()
        {
			
        }

        public void Hide()
		{
			State = PanelState.Hide;
			Debug.Log("Hide");
		}

        public void Close(bool destroy = true)
		{
			State = PanelState.Closed;
			Debug.Log("Close");
            if (destroy)
            {
                gameObject.DestroySelf();
            }
		}
    }
}
