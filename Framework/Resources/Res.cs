using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
	using System;
	using UnityEngine;

	/// <summary>
	/// 资源容器成员属性和方法
	/// </summary>
	public interface IRes
	{
		int ReferenceCount { get; set; }//引用计数
		string AssetName { get; set; }//资源名标记
		string OwnerBundleName { get; set; }//所属AB包
		Type AssetType { get; set; }//资源类型标记

        UnityEngine.Object Asset { get; set; }//资源

		void Reference();//引用执行
		void Release();//释放执行
		void Destroy();//摧毁执行，慎用
	}


	/// <summary>
	/// 资源容器
	/// </summary>
	public class Res : IRes
	{
		/// <summary>
		/// 资源名标记
		/// </summary>
		public string AssetName { get; set; }

		/// <summary>
		/// 所属AB包
		/// </summary>
		public string OwnerBundleName { get; set; }

		/// <summary>
		/// 资源类型标记
		/// </summary>
		public Type AssetType { get; set; }

		/// <summary>
		/// 资源的引用次数
		/// </summary>
		public int ReferenceCount { get; set; }

		/// <summary>
		/// 资源
		/// </summary>
		public UnityEngine.Object Asset { get; set; }

		/// <summary>
		/// 资源构造函数
		/// </summary>
		/// <param name="name">资源名</param>
		/// <param name="bundle">所属ab包</param>
		/// <param name="type">资源类型</param>
		public Res(string name, string bundle, Type type, UnityEngine.Object asset)
		{
			AssetName = name;
			OwnerBundleName = bundle;
			AssetType = type;
			Asset = asset;
			ReferenceCount = 0;
		}

		/// <summary>
		/// 引用次数
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
				if (AssetType.GetType() == typeof(AssetBundle))
				{
					var ab = Asset as AssetBundle;
					ab.Unload(false);
					Asset = null;
                }
                else
                {
					Resources.UnloadAsset(Asset);
					Asset = null;
				}
			}
		}

		/// <summary>
		/// 强制释放，不考虑引用(慎用)
		/// </summary>
		public void Destroy()
		{
			if (AssetType.GetType() == typeof(AssetBundle))
			{
				var ab = Asset as AssetBundle;
				ab.Unload(false);
				Asset = null;
			}
			else
			{
				Resources.UnloadAsset(Asset);
				Asset = null;
			}
		}
	}
}
