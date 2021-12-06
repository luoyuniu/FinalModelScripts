using System;
using System.Reflection;

namespace Framework {
	public static class SingletonCreator
	{
	    public static T CreateSingleton<T>() where T : class, ISingleton
	    {
	        // 获取私有构造函数
	        var ctors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
	
	        // 获取无参构造函数
	        var ctor = Array.Find(ctors, c => c.GetParameters().Length == 0);
	
	        //无构造函数报错
	        if (ctor == null)
	        {
	            throw new Exception("Non-Public Constructor() not found! in " + typeof(T));
	        }
	
	        // 通过构造函数，创建实例
	        T retInstance = ctor.Invoke(null) as T;
	
	        retInstance.OnSingletonInit();
	
	        return retInstance;
	    }
	}
}
