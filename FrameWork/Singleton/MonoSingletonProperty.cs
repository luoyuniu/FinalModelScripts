using UnityEngine;

namespace FrameWork {	
	public static class MonoSingletonProperty<T> where T : UnityEngine.MonoBehaviour, ISingleton
	{
		private static T mInstance = null;
	
		public static T Instance
		{
			get
			{
				if (null == mInstance)
				{
					mInstance = MonoSingletonCreator.CreateMonoSingleton<T>();
				}
	
				return mInstance;
			}
		}
	
		public static void Dispose()
		{
			if (MonoSingletonCreator.IsUnitTestMode)
			{
				Object.DestroyImmediate(mInstance.gameObject);
			}
			else
			{
				Object.Destroy(mInstance.gameObject);
			}
	
			mInstance = null;
		}
	}
}
