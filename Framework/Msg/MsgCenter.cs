using System;
using System.Collections.Generic;

// 消息中心，统一发送消息
namespace Framework
{
	using UnityEngine;

	[MonoSingletonPath("[Event]/MsgCenter")]
	public partial class MsgCenter : MonoBehaviour, ISingleton
	{
		//已注册的管理模块
		private static Dictionary<int, Func<MgrBase>> mRegisteredManagers =
			new Dictionary<int, Func<MgrBase>>();

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

		//模块发送消息
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
		}

		//模块注册
		public static void RegisterManagerFactory(int mgrId, Func<MgrBase> managerFactory)
		{
            Debug.Log("MsgCenter模块注册:" + mgrId);
			if (mRegisteredManagers.ContainsKey(mgrId))
			{
				mRegisteredManagers[mgrId] = managerFactory;
			}
			else
			{
				mRegisteredManagers.Add(mgrId, managerFactory);
			}
		}
	}
}
