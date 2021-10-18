
namespace FrameWork
{
    public class ResManager : Singleton<ResManager>
    {
        /// <summary>
        /// 获取一个资源加载器
        /// </summary>
        /// <returns></returns>
        public static ResLoader Allocate()
		{
			return SafeObjectPool<ResLoader>.Instance.Allocate();
        }
	}
}
