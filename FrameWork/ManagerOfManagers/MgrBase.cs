namespace FrameWork
{
	using System;
	
	/// <summary>
	/// manager基类
	/// </summary>
	public abstract class MgrBase : MgrMonoBehaviour,IManager
	{
		private readonly EventSystem mEventSystem = NonPublicObjectPool<EventSystem>.Instance.Allocate();

		#region IManager
		public virtual void Init() {}
		#endregion

		public abstract int ManagerId { get ; }

		public override IManager Manager
		{
			get { return this; }
		}
		
		public void RegisterEvent<T>(T msgId,OnEvent process) where T:IConvertible
		{
			mEventSystem.Register (msgId, process);
		}

		public void UnRegisterEvent<T>(T msgEvent, OnEvent process) where T : IConvertible
		{
			mEventSystem.UnRegister(msgEvent, process);

		}

		public override void SendMsg(IMsg msg)
		{
            if (msg.ManagerID == ManagerId)
			{
                Process(msg.EventID, msg);
			}
			else 
			{
				MsgCenter.Instance.SendMsg (msg);
			}
		}

        public override void SendEvent<T>(T eventId)
	    {
			SendMsg(Msg.Allocate(eventId));
		}
        

        // 来了消息以后,通知整个消息链
		protected override void ProcessMsg(int eventId,Msg msg)
		{
			mEventSystem.Send(msg.EventID,msg);
		}
		
		protected override void OnBeforeDestroy()
		{
            mEventSystem?.OnRecycled();
		}
	}
}
