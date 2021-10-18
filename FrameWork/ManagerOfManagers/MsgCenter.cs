using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace FrameWork
{
	[MonoSingletonPath("[Event]/MsgCenter")]
	public partial class MsgCenter : UnityEngine.MonoBehaviour, ISingleton
	{
		public static MsgCenter Instance
		{
			get { return MonoSingletonProperty<MsgCenter>.Instance; }
		}

		public void OnSingletonInit()
		{

		}

		public void Dispose()
		{
			mRegisteredManagers.Clear();
			
			MonoSingletonProperty<MsgCenter>.Dispose();
		}

		void Awake()
		{
			DontDestroyOnLoad(this);
		}


		public void SendMsg(IMsg tmpMsg)
		{

			foreach (var manager in mRegisteredManagers)
			{
				if (manager.Key == tmpMsg.ManagerID)
				{
					manager.Value().SendMsg(tmpMsg);
					return;
				}
			}

			ForwardMsg(tmpMsg as Msg);
		}

		private static Dictionary<int, Func<MgrBase>> mRegisteredManagers =
			new Dictionary<int, Func<MgrBase>>();
		
		public static void RegisterManagerFactory(int mgrId, Func<MgrBase> managerFactory)
		{
			if (mRegisteredManagers.ContainsKey(mgrId))
			{
				mRegisteredManagers[mgrId] = managerFactory;
			}
			else
			{
				mRegisteredManagers.Add(mgrId, managerFactory);
			}
		}

		partial void ForwardMsg(Msg tmpMsg);
	}
}
