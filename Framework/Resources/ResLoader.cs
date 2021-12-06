using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace Framework
{
	/// <summary>
	/// 资源的加载容器属性和方法
	/// </summary>
	public interface IResLoader : IPoolable, IPoolType
	{
        List<Res> LoaderResList { get; set; }

		Res AssetLoad(ResSearchKey resSearchKey); //Resources加载单个资源
		IEnumerator AssetLoadAsync(ResSearchKey resSearchKey, Action<Res> action); //Resources异步加载单个资源

		AssetBundle ABLoadFromMemory(ResSearchKey resSearchKey); //从内存加载AB包
		IEnumerator ABLoadFromMemoryAsync(ResSearchKey resSearchKey, Action<AssetBundle> action); //从内存异步加载AB包

		AssetBundle ABLoadFromFile(ResSearchKey resSearchKey); //从磁盘加载AB包（最快）
		IEnumerator ABLoadFromFileAsync(ResSearchKey resSearchKey, Action<AssetBundle> action); //从磁盘异步加载AB包（与同步无异）

		IEnumerator ABLoadWebRequest(ResSearchKey resSearchKey, Action<AssetBundle> action); //WebRequest请求AB包
	}

	/// <summary>
	/// 资源的加载器
	/// </summary>
	public class ResLoader : IResLoader
	{
		/// <summary>
		/// 加载器可否回收
		/// </summary>
		public bool IsRecycled { get; set; }

		public List<Res> LoaderResList { get; set; }

		/// <summary>
		/// 分配加载器
		/// </summary>
		/// <returns>加载器</returns>
		public static ResLoader Allocate()
		{
			ResLoader resLoader = SafeObjectPool<ResLoader>.Instance.Allocate();

            if (resLoader.LoaderResList == null)
            {
				resLoader.LoaderResList = new List<Res>();
			}

			return resLoader;
		}

		/// <summary>
		/// 准备回收加载器
		/// </summary>
		public void OnRecycled()
		{
			LoaderResList = null;
		}

		/// <summary>
		/// 回收加载器进入对象池
		/// </summary>
		public void Recycle2Cache()
		{
			OnRecycled();

			SafeObjectPool<ResLoader>.Instance.Recycle(this);
		}

		/// <summary>
		/// Resources加载单个资源
		/// </summary>
		/// <param name="resSearchKey">资源信息</param>
		/// <returns>资源</returns>
		public Res AssetLoad(ResSearchKey resSearchKey)
		{
			var assetIns = Resources.Load(resSearchKey.GetPath(), resSearchKey.AssetType);

			Res loadAsset = Add2Res(resSearchKey, assetIns);

			return loadAsset;
		}

		/// <summary>
		/// Resources异步加载单个资源
		/// </summary>
		/// <param name="resSearchKey">资源信息</param>
		/// <returns>迭代器</returns>
		public IEnumerator AssetLoadAsync(ResSearchKey resSearchKey, Action<Res> callback)
		{
			ResourceRequest request = Resources.LoadAsync(resSearchKey.GetPath(), resSearchKey.AssetType);

			yield return request;

			Res loadAsset = Add2Res(resSearchKey, request.asset);

			callback?.Invoke(loadAsset);
		}

		/// <summary>
		/// 从内存加载AB包
		/// </summary>
		/// <param name="resSearchKey">资源信息</param>
		/// <returns>AB包</returns>
		public AssetBundle ABLoadFromMemory(ResSearchKey resSearchKey)
		{
			AssetBundle ab = AssetBundle.LoadFromMemory(File.ReadAllBytes(resSearchKey.GetPath()));

			return ab;
		}

		/// <summary>
		/// 从内存异步加载AB包
		/// </summary>
		/// <param name="resSearchKey">资源信息</param>
		/// <returns>迭代器</returns>
		public IEnumerator ABLoadFromMemoryAsync(ResSearchKey resSearchKey, Action<AssetBundle> action)
		{
			//获取AB包资源
			AssetBundleCreateRequest request = AssetBundle.LoadFromMemoryAsync(File.ReadAllBytes(resSearchKey.GetPath()));

			yield return request;

			action?.Invoke(request.assetBundle);
		}

		/// <summary>
		/// 从磁盘加载AB包（最快）
		/// </summary>
		/// <param name="resSearchKey">资源信息</param>
		/// <returns>AB包</returns>
		public AssetBundle ABLoadFromFile(ResSearchKey resSearchKey)
		{
			AssetBundle ab = AssetBundle.LoadFromFile(resSearchKey.GetPath());

			return ab;
		}

		/// <summary>
		/// 从磁盘异步加载AB包（与同步无异）
		/// </summary>
		/// <param name="resSearchKey">资源信息</param>
		/// <returns>迭代器</returns>
		public IEnumerator ABLoadFromFileAsync(ResSearchKey resSearchKey, Action<AssetBundle> action)
		{
			//获取AB包资源
			AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(resSearchKey.GetPath());

			yield return request;

			action?.Invoke(request.assetBundle);
		}

		/// <summary>
		/// 网络请求方式加载AB包
		/// </summary>
		/// <param name="resSearchKey">资源信息</param>
		/// <returns>迭代器</returns>
		public IEnumerator ABLoadWebRequest(ResSearchKey resSearchKey, Action<AssetBundle> action)
		{
			//获取AB包资源
			UnityWebRequest webReuest = UnityWebRequestAssetBundle.GetAssetBundle(resSearchKey.GetPath());

			yield return webReuest.SendWebRequest();

			if (webReuest.result != UnityWebRequest.Result.Success)
			{
				yield break;
			}
			else
			{
				AssetBundle ab = DownloadHandlerAssetBundle.GetContent(webReuest);

				action?.Invoke(ab);
			}
		}

		private Res Add2Res(ResSearchKey resSearchKey, Object assetIns)
		{
			Res loadAsset = new Res(resSearchKey.AssetName, resSearchKey.OwnerBundle, resSearchKey.AssetType, assetIns);

			//增添该资源的引用次数
			loadAsset.Reference();

			return loadAsset;
		}
	}
}
