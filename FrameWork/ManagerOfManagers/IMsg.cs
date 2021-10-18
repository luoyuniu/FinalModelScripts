using System;

namespace FrameWork
{
    public interface IMsg : IPoolable, IPoolType
    {
        /// <summary>
        /// 事件ID
        /// </summary>
        int EventID { get; set; }
		
        /// <summary>
        /// 是否已经处理
        /// </summary>
        bool Processed { get; set; }
		
        /// <summary>
        /// 能否复用 
        /// </summary>
        bool ReuseAble { get; set; }
		
        int ManagerID { get; }
    }
    
    /// <summary>
    /// 消息主体
    /// </summary>
    public class Msg : IMsg, IPoolable, IPoolType
    {
        /// <summary>
        /// EventID
        /// </summary>
        public virtual int EventID { get; set; }

        /// <summary>
        /// Processed or not
        /// </summary>
        public bool Processed { get; set; }

        /// <summary>
        /// reusable or not 
        /// </summary>
        public bool ReuseAble { get; set; }

        public int ManagerID
        {
            get { return EventID / MsgSpan.Count * MsgSpan.Count; }
        }

        public Msg()
        {
        }

        #region Object Pool

        public static Msg Allocate<T>(T eventId) where T : IConvertible
        {
            Msg msg = SafeObjectPool<Msg>.Instance.Allocate();
            msg.EventID = eventId.ToInt32(null);
            msg.ReuseAble = true;
            return msg;
        }

        public virtual void Recycle2Cache()
        {
            SafeObjectPool<Msg>.Instance.Recycle(this);
        }

        void IPoolable.OnRecycled()
        {
            Processed = false;
        }

        bool IPoolable.IsRecycled { get; set; }

        #endregion
    }
}
