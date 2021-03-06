using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace Framework
{
    public class TypeEventSystem : ITypeEventSystem
    {
        public class TypeEventUnregister<T> : IDisposable
        {
            public Action<T> OnReceive;
            public ITypeEventSystem TypeEventSystem;

            public void Dispose()
            {
                TypeEventSystem.UnRegisterEvent<T>(OnReceive);
            }
        }

        /// <summary>
        /// 接口 只负责存储在字典中
        /// </summary>
        interface IRegisterations : IDisposable
        {
        }


        /// <summary>
        /// 多个注册
        /// </summary>
        class Registerations<T> : IRegisterations
        {
            /// <summary>
            /// 因为委托本身就可以一对多注册
            /// </summary>
            public Action<T> OnReceives = obj => { };

            public void Dispose()
            {
                OnReceives = null;
            }
        }

        /// <summary>
        /// 全局注册事件
        /// </summary>
        private static readonly ITypeEventSystem mGlobalEventSystem = new TypeEventSystem();

        /// <summary>
        /// 
        /// </summary>
        private Dictionary<Type, IRegisterations> mTypeEventDict = DictionaryPool<Type, IRegisterations>.Get();

        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="onReceive"></param>
        /// <typeparam name="T"></typeparam>
        public static IDisposable Register<T>(System.Action<T> onReceive)
        {
            return mGlobalEventSystem.RegisterEvent<T>(onReceive);
        }

        /// <summary>
        /// 注销事件
        /// </summary>
        /// <param name="onReceive"></param>
        /// <typeparam name="T"></typeparam>
        public static void UnRegister<T>(System.Action<T> onReceive)
        {
            mGlobalEventSystem.UnRegisterEvent<T>(onReceive);
        }

        /// <summary>
        /// 发送事件
        /// </summary>
        /// <param name="t"></param>
        /// <typeparam name="T"></typeparam>
        public static void Send<T>(T t)
        {
            mGlobalEventSystem.SendEvent<T>(t);
        }

        /// <summary>
        /// 发送事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void Send<T>() where T : new()
        {
            mGlobalEventSystem.SendEvent<T>();
        }


        public IDisposable RegisterEvent<T>(Action<T> onReceive)
        {
            var type = typeof(T);

            IRegisterations registerations = null;

            if (mTypeEventDict.TryGetValue(type, out registerations))
            {
                var reg = registerations as Registerations<T>;
                reg.OnReceives += onReceive;
            }
            else
            {
                var reg = new Registerations<T>();
                reg.OnReceives += onReceive;
                mTypeEventDict.Add(type, reg);
            }

            return new TypeEventUnregister<T> {OnReceive = onReceive, TypeEventSystem = this};
        }


        public void UnRegisterEvent<T>(Action<T> onReceive)
        {
            var type = typeof(T);

            IRegisterations registerations = null;

            if (mTypeEventDict.TryGetValue(type, out registerations))
            {
                var reg = registerations as Registerations<T>;
                reg.OnReceives -= onReceive;
            }
        }

        public void SendEvent<T>() where T : new()
        {
            var type = typeof(T);

            IRegisterations registrations = null;

            if (mTypeEventDict.TryGetValue(type, out registrations))
            {
                var reg = registrations as Registerations<T>;
                reg.OnReceives(new T());
            }
        }

        public void SendEvent<T>(T e)
        {
            var type = typeof(T);

            IRegisterations registrations = null;

            if (mTypeEventDict.TryGetValue(type, out registrations))
            {
                var reg = registrations as Registerations<T>;
                reg.OnReceives(e);
            }
        }

        public void Clear()
        {
            foreach (var keyValuePair in mTypeEventDict)
            {
                keyValuePair.Value.Dispose();
            }

            mTypeEventDict.Clear();
        }

        public void Dispose()
        {
        }
    }
}
