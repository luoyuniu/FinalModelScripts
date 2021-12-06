using UnityEngine;
using Framework;

namespace Framework {	/// <summary>
	/// 启动脚本
	/// </summary>
	public class StartUp : MonoBehaviour
	{
	    void Awake()
	    {
	        // 热更新之前初始化一些模块
	        InitBeforeHotUpdate();
	        // 热更新
	        // 热更新后初始化一些模块
	        InitAfterHotUpdate();
	        // 启动游戏
	        StartGame();
	    }
	
	    private void StartGame()
	    {
	        //启动lua框架//
	    }
	
	    /// <summary>
	    /// 热更新之前初始化一些模块
	    /// </summary>
	    private void InitBeforeHotUpdate()
	    {
	        //new一个网络消息监听器
	        // 限制游戏帧数
	        // 手机常亮//
	        // 后台运行//
	
	        // 日志
	        GameLogger.Init();
	        //LogCat.Init();
	        // 网络消息注册
	        // 界面管理器(UIManager)
	        UIManager.Instance.Init();
	
	        // 版本号(VersionMgr)
	
	        // 预加载AssetBundle(AssetBundleMgr)
	        // TODO 加载必要的资源AssetBundle
	
	        //计时器线程（TimerThread）
	        //客户端网络接口（ClientNet）
	        //ScreenCapture(截屏管理)
	    }
	
	    /// <summary>
	    /// 热更新后初始化一些模块
	    /// </summary>
	    private void InitAfterHotUpdate()
	    {
	        // 资源管理器(ResourceManager)
	        // 音效管理器(AudioMgr)
	        // 多语言(LanguageMgr)
	        // 图集管理器(SpriteManager)
	    }
	
	    //门面模式
	    private void FixedUpdate()
	    {
	        //物理更新
	        //AppFacade.Instance.FixedUpdateEx();
	    }
	
	    private void Update()
	    {
	        //逻辑更新
	        //AppFacade.Instance.UpdateEx();
	    }
	
	    private void LateUpdate()
	    {
	        //相机更新
	        //AppFacade.Instance.LateUpdateEx();
	    }
	}
}
