namespace Framework
{
	using UnityEngine;
	using System;
	using System.Collections.Generic;

	public abstract class MgrMonoBehaviour : MonoBehaviour
	{
		protected bool mReceiveMsgOnlyObjActive = true;
		
		public void Process (int eventId, params object[] param)  
		{
			if (mReceiveMsgOnlyObjActive && gameObject.activeInHierarchy || !mReceiveMsgOnlyObjActive)
			{
				var msg = param[0] as IMsg;
				ProcessMsg(eventId, msg as Msg);
				msg.Processed = true;
				
				if (msg.ReuseAble)
				{
					msg.Recycle2Cache();
				}
			}
		}

		protected virtual void ProcessMsg (int eventId,Msg msg) {}

		public abstract IManager Manager { get; }

		protected void RegisterEvents<T>(params T[] eventIDs) where T : IConvertible
		{
			foreach (var eventId in eventIDs)
			{
				RegisterEvent(eventId);
			}
		}

		protected void RegisterEvent<T>(T eventId) where T : IConvertible
		{
			mCachedEventIds.Add(eventId.ToUInt16(null));
			Manager.RegisterEvent(eventId, Process);
		}
		
		protected void UnRegisterEvent<T>(T eventId) where T : IConvertible
		{
			mCachedEventIds.Remove(eventId.ToUInt16(null));
			Manager.UnRegisterEvent(eventId.ToInt32(null), Process);
		}

		protected void UnRegisterAllEvent()
		{
			if (null != mPrivateEventIds)
			{
				mPrivateEventIds.ForEach(id => Manager.UnRegisterEvent(id,Process));
			}
		}

		public virtual void SendMsg(IMsg msg)
		{
			Manager.SendMsg(msg);
		}
		
        public virtual void SendEvent<T>(T eventId) where T : IConvertible
		{
			Manager.SendEvent(eventId);
		}
		
		private List<ushort> mPrivateEventIds = null;
		
		private List<ushort> mCachedEventIds
		{
			get { return mPrivateEventIds ?? (mPrivateEventIds = new List<ushort>()); }
		}

		protected virtual void OnDestroy()
		{			
			if (Application.isPlaying) 
			{
				OnBeforeDestroy();
				UnRegisterAllEvent();
			}
		}
		
	    protected virtual void OnBeforeDestroy(){}
	}
}
