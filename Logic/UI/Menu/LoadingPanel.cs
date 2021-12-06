using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;
using UnityEngine.UI;

namespace Framework {	
	public class LoadingPanel : UIPanelBase
	{
	    private Text _progressText;
	    private Slider _progressSlider;
	    private bool _loaded = false;
	
	    public bool Loaded { get => _loaded; }
	
	    /// <summary>
	    /// 加载组件
	    /// </summary>
	    private void Start()
	    {
	        _progressText = GetControl<Text>("LoadText");
	        _progressSlider = GetControl<Slider>("LoadingSlider");
	    }
	
	    /// <summary>
	    /// 进度条更新
	    /// </summary>
	    private void Update()
	    {
	        if (_progressSlider.value < SceneMgr.Instance.Op.progress || BasicDataTypeExtension.FloatEqual(SceneMgr.Instance.Op.progress, 0.9f))
	        {
	            SetLoadingPercentage();
	        }
	    }
	
	    //进度条显示
	    private void SetLoadingPercentage()
	    {
	        _progressSlider.value += Time.deltaTime;
	        if (_progressSlider.value > 0.5)
	        {
	            _progressText.color = Color.black;
	        }
	        _progressText.text = (_progressSlider.value * 100).ToString("f0") + "%";
	
	        Debug.Log(_progressText.text);
	        if (_progressSlider.value > 1 || BasicDataTypeExtension.FloatEqual(_progressSlider.value, 1f))
	        {
	            _loaded = true;
	        }
	    }
	}
	
	
}
