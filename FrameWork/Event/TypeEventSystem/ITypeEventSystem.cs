using System;

namespace FrameWork
{
    public interface ITypeEventSystem : IDisposable
    {
        IDisposable RegisterEvent<T>(Action<T> onReceive);
        void UnRegisterEvent<T>(Action<T> onReceive);

        void SendEvent<T>() where T : new();

        void SendEvent<T>(T e);
        
        void Clear();
    }
}