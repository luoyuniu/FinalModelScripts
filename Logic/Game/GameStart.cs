using UnityEngine;
using System.Collections;
using System;
using Framework;

namespace Framework {	
	public class GameStart : MonoBehaviour
	{
	    void Awake()
	    {
	        Debug.Log("游戏完整性检查资源下载等前期工作");
	    }
	
	    // Use this for initialization
	    void Start()
	    {
	        SceneMgr.Instance.LoadSceneAysnc("main");
	    }
	
	    // Update is called once per frame
	    void Update()
	    {
	
	    }
	}
}
