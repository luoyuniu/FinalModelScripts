using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using FrameWork;
using System;

public class SceneMgr : MgrBase,ISingleton
{
    private AsyncOperation _op;

    public float transitionTime = 1f;
    public  AsyncOperation Op { get { return _op; } }
    public override int ManagerId { get { return MgrID.Game; } }

    private LoadingPanel _loadingPanel;

    public void OnSingletonInit()
    {
        
    }

    public static SceneMgr Instance
    {
        get { return MonoSingletonProperty<SceneMgr>.Instance; }
    }

    #region 异步加载界面
    //异步加载场景
    public void LoadSceneAysnc(string nextScene, Func<LoadingPanel> customLoadingStyle = null,Action customOnLoadedEnd = null)
    {
        if (customLoadingStyle == null)
        {
            customLoadingStyle = DefaultLoadingStyle;
        }

        if (customOnLoadedEnd == null)
        {
            customOnLoadedEnd = DefaultLoadedEnd;
        }

        StartCoroutine(ReallLoadScene(nextScene, customLoadingStyle));
        StartCoroutine(LoadEnd(_op, customOnLoadedEnd));
    }

    private IEnumerator ReallLoadScene(string nextScene, Func<LoadingPanel> customLoadingStyle)
    {
        _loadingPanel = customLoadingStyle();
        //异步加载
        _op = SceneManager.LoadSceneAsync(nextScene);
        //不允许显示界面
        _op.allowSceneActivation = false;

        while (!_loadingPanel.Loaded)
        {
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("地图加载100%");
        _op.allowSceneActivation = true;
    }

    private IEnumerator LoadEnd(AsyncOperation Op, Action OnLoadedEnd = null)
    {
        while (!Op.isDone)
        {
            yield return new WaitForEndOfFrame();
        }

        OnLoadedEnd?.Invoke();
    }
    
    private LoadingPanel DefaultLoadingStyle()
    {
        UIManager.Instance.OpenPanel(PanelType.DEFAULTLOADINGPANEL);
        return UIManager.Instance.GetPanel(PanelType.DEFAULTLOADINGPANEL) as LoadingPanel;
    }

    public void DefaultLoadedEnd()
    {
        UIManager.Instance.ClosePanel(true);
        UIManager.Instance.Reset();
    }
    #endregion
}


