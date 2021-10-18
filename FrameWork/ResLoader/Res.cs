using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
	using UnityEngine;

	public enum ResType
	{
		ResSingle,//Res单个资源
		ResGroup,//Res组资源
		AssetBundle,//AB包资源
	}

	/// <summary>
	/// 资源容器成员属性和方法
	/// </summary>
	public interface IRes
	{
		ResType AssetType { get; set; }//资源类型标记
		string AssetKey { get; set; }//资源标记
		Object Asset { get; set; }//单体资源
		Object[] Assets { get; set; }//资源组
		int ReferenceCount { get; set; }//引用计数
		void Reference();//引用执行
		void Release();//释放执行
		void Destroy();//摧毁执行
	}


	/// <summary>
	/// 资源容器
	/// </summary>
	public class Res : IRes
	{

		/// <summary>
		/// 资源类型标记
		/// </summary>
		public ResType AssetType { get; set; }
		/// <summary>
		/// 资源名标记
		/// </summary>
		public string AssetKey { get; set; }
		/// <summary>
		/// 单体资源
		/// </summary>
		public Object Asset { get; set; }
		/// <summary>
		/// 资源组
		/// </summary>
		public Object[] Assets { get; set; }
		/// <summary>
		/// 资源的引用次数
		/// </summary>
		public int ReferenceCount { get; set; }

		/// <summary>
		/// 单体资源构造函数
		/// </summary>
		/// <param name="asset"></param>
		/// <param name="path"></param>
		/// <param name="type"></param>
		public Res(Object asset, string path, ResType type)
		{
			Asset = asset;
			AssetKey = path;
			AssetType = type;
			ReferenceCount = 0;
		}
		/// <summary>
		/// 资源组构造函数
		/// </summary>
		/// <param name="assets"></param>
		/// <param name="path"></param>
		/// <param name="type"></param>
		public Res(Object[] assets, string path, ResType type)
		{
			Assets = assets;
			AssetKey = path;
			AssetType = type;
			ReferenceCount = 0;
		}

		/// <summary>
		/// 引用
		/// </summary>
		public void Reference()
		{
			ReferenceCount++;
		}

		/// <summary>
		/// 释放
		/// </summary>
		public void Release()
		{
			//引用数--
			ReferenceCount--;

			//引用数小于等于0 则将资源从内存中释放
			if (ReferenceCount <= 0)
			{
				switch (AssetType)
				{
					case ResType.ResSingle:
						Resources.UnloadAsset(Asset);
						Asset = null;
						break;
					case ResType.ResGroup:
						foreach (var asset in Assets)
						{
							Resources.UnloadAsset(asset);
						}
						Assets = null;
						break;
					case ResType.AssetBundle:
						var ab = Asset as AssetBundle;
						if (ab != null) ab.Unload(false);
						Asset = null;
						break;
					default:
						Debug.LogError("This ResType is Error or Null");
						break;
				}

				if (ResLoader.PublicLoadingResList.Contains(this))
				{
					ResLoader.PublicLoadingResList.Remove(this);
				}
			}
		}

		/// <summary>
		/// 强制释放，不考虑引用
		/// </summary>
		public void Destroy()
		{
			switch (AssetType)
			{
				case ResType.ResSingle:
					Resources.UnloadAsset(Asset);
					break;
				case ResType.ResGroup:
					foreach (var asset in Assets)
					{
						Resources.UnloadAsset(asset);
					}
					break;
				case ResType.AssetBundle:
					var ab = Asset as AssetBundle;
					if (ab != null) ab.Unload(true);
					break;
				default:
					Debug.LogError("This ResType is Error or Null");
					break;
			}
		}
	}
}