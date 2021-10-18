namespace FrameWork
{
    /// <summary>
    /// 管理模块编号
    /// </summary>
    public abstract class MgrID
    {
        public const int Framework = 0;
        public const int UI = Framework + MsgSpan.Count; 
        public const int Audio = UI + MsgSpan.Count; 
        public const int Network = Audio + MsgSpan.Count;
        public const int UIFilter = Network + MsgSpan.Count;
        public const int Game = UIFilter + MsgSpan.Count;
        public const int PCConnectMobile = Game + MsgSpan.Count;
        public const int FrameworkEnded = PCConnectMobile + MsgSpan.Count;
        public const int FrameworkMsgModuleCount = 7;
    }
}
