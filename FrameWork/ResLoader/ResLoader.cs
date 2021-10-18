using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace FrameWork
{
	/// <summary>
	/// 资源的加载容器属性和方法
	/// </summary>
	public interface IResLoader : IPoolable, IPoolType
	{
		List<Res> SelfLoadingResList { get; set; } //储存当前容器加载的资源
		T AssetLoad<T>(string assetPath) where T : Object; //Resources单个资源的同步加载
		T[] AssetsLoad<T>(string folderPaht) where T : Object; //Resources目录下全部资源加载
		IEnumerator AssetLoadAsync<T>(string assetPath, Action<T> callback) where T : Object; //异步加载Resources下的单个资源
		AssetBundle ABLoadFromMemory(string assetPath); //直接加载AB包
		IEnumerator ABLoadFromMemoryAsync(string assetPath, Action<AssetBundle> callback); //LoadFromMemoryAsync 异步加载AB包
		AssetBundle ABLoadFromFile(string assetPath); //LoadFromFile直接加载本地AB包
		IEnumerator ABLoadFromFileAsync(string assetPath, Action<AssetBundle> callback); //从本地异步加载AB资源
		IEnumerator ABLoadWebRequest(string assetPath, Action<AssetBundle> callback); //WebRequest请求AB包
		void UnloadAll();//卸载当前加载器资源
	}



/// <summary>
/// 资源的加载器
/// </summary>
	public class ResLoader : IResLoader
	{

		/// <summary>
		/// 公共容器 储存所有动态资源
		/// </summary>
		public static List<Res> PublicLoadingResList = new List<Res>();

        /// <summary>
        /// 自身容器 
        /// </summary>
        public List<Res> SelfLoadingResList { get; set; }

		public bool IsRecycled { get; set; }

		public ResLoader()
		{
			if (SelfLoadingResList == null)
			{
				SelfLoadingResList = new List<Res>();
			}
		}

		public static ResLoader Allocate()
		{
			return SafeObjectPool<ResLoader>.Instance.Allocate();
		}

		public void OnRecycled()
		{
			PublicLoadingResList.Clear();
			SelfLoadingResList.Clear();
		}

		public void Recycle2Cache()
		{
			SafeObjectPool<ResLoader>.Instance.Recycle(this);
		}


		/// <summary>
		/// Resources单个资源的同步加载
		/// </summary>
		/// <param name="assetPath"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T AssetLoad<T>(string assetPath) where T : Object
		{
            //先从当前容器中查找
            var loadAsset = SelfLoadingResList.Find(asset => asset.AssetKey == assetPath);
			if (loadAsset != null)
			{
				return loadAsset.Asset as T;
			}
			//在从公共容器中查找
			loadAsset = PublicLoadingResList.Find(asset => asset.AssetKey == assetPath);
			if (loadAsset != null)
			{
				return loadAsset.Asset as T;
			}
			//资源列表中不存在则重写加载
			var assetIns = Resources.Load<T>(assetPath);
			if (assetIns == null)
			{
				Debug.LogError(assetPath + " is Null !");
				return null;
			}
			//加载后添加到资源列表
			loadAsset = new Res(assetIns, assetPath, ResType.ResSingle);
			PublicLoadingResList.Add(loadAsset);
			SelfLoadingResList.Add(loadAsset);
			//增添该资源的引用次数
			loadAsset.Reference();
			//返回目标
			return assetIns;
		}

		/// <summary>
		/// Resources目录下全部资源加载
		/// </summary>
		/// <param name="folderPaht"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T[] AssetsLoad<T>(string folderPaht) where T : Object
		{
			//先从当前容器中查找
			var loadAsset = SelfLoadingResList.Find(asset => asset.AssetKey == folderPaht);
			if (loadAsset != null)
			{
				return loadAsset.Asset as T[];
			}
			//在从公共容器中查找
			loadAsset = PublicLoadingResList.Find(asset => asset.AssetKey == folderPaht);
			if (loadAsset != null)
			{
				return loadAsset.Asset as T[];
			}
			//资源列表中不存在则重写加载
			T[] assetIns = Resources.LoadAll<T>(folderPaht);
			if (assetIns == null)
			{
				Debug.LogError(folderPaht + " is Null !");
				return null;
			}

			//加载后添加到资源列表
			loadAsset = new Res(assetIns, folderPaht, ResType.ResGroup);
			PublicLoadingResList.Add(loadAsset);
			SelfLoadingResList.Add(loadAsset);
			//增添该资源的引用次数
			loadAsset.Reference();
			return assetIns;
		}

		/// <summary>
		/// 异步加载Resources下的单个资源
		/// </summary>
		/// <param name="assetPath"></param>
		/// <param name="callback"></param> 异步加载的回调
		/// <returns></returns>
		public IEnumerator AssetLoadAsync<T>(string assetPath, Action<T> callback) where T : Object
		{
			//先从当前容器中查找
			var loadAsset = SelfLoadingResList.Find(asset => asset.AssetKey == assetPath);
			if (loadAsset != null)
			{
				yield return loadAsset;
				callback(loadAsset.Asset as T);
				yield break;
			}

			//在从公共容器中查找
			loadAsset = PublicLoadingResList.Find(asset => asset.AssetKey == assetPath);
			if (loadAsset != null)
			{
				yield return loadAsset;
				callback(loadAsset.Asset as T);
				yield break;
			}

			ResourceRequest request = Resources.LoadAsync(assetPath);
			yield return request;
			//加载后添加到资源列表
			loadAsset = new Res(request.asset, assetPath, ResType.ResSingle);
			PublicLoadingResList.Add(loadAsset);
			SelfLoadingResList.Add(loadAsset);
			//增添该资源的引用次数
			loadAsset.Reference();
			callback(request.asset as T);
		}

		/// <summary>
		/// 直接加载AB包
		/// </summary>
		/// <param name="assetPath"></param>
		/// <returns></returns>
		public AssetBundle ABLoadFromMemory(string assetPath)
		{
			//先从自身容器查找
			var loadAsset = SelfLoadingResList.Find(asset => asset.AssetKey == assetPath);
			if (loadAsset != null)
			{
				return loadAsset.Asset as AssetBundle;
			}
			//再从公共容器查找		
			loadAsset = PublicLoadingResList.Find(asset => asset.AssetKey == assetPath);
			if (loadAsset != null)
			{
				return loadAsset.Asset as AssetBundle;
			}

			AssetBundle ab = AssetBundle.LoadFromMemory(File.ReadAllBytes(assetPath));
			loadAsset = new Res(ab, assetPath, ResType.AssetBundle);
			PublicLoadingResList.Add(loadAsset);
			SelfLoadingResList.Add(loadAsset);
			//增添该资源的引用次数
			loadAsset.Reference();
			return ab;
		}


		/// <summary>
		///  LoadFromMemoryAsync 异步加载AB包
		/// </summary>
		/// <param name="assetPath"></param>
		/// <param name="callback"></param>
		/// <returns></returns>
		public IEnumerator ABLoadFromMemoryAsync(string assetPath, Action<AssetBundle> callback)
		{
			//先从自身容器查找
			var loadAsset = SelfLoadingResList.Find(asset => asset.AssetKey == assetPath);
			if (loadAsset != null)
			{
				yield return loadAsset;
				callback(loadAsset.Asset as AssetBundle);
				yield break;
			}
			//再从公共容器查找
			loadAsset = PublicLoadingResList.Find(asset => asset.AssetKey == assetPath);
			if (loadAsset != null)
			{
				yield return loadAsset;
				callback(loadAsset.Asset as AssetBundle);
				yield break;
			}

			//获取AB包资源
			AssetBundleCreateRequest request = AssetBundle.LoadFromMemoryAsync(File.ReadAllBytes(assetPath));
			yield return request;
			loadAsset = new Res(request.assetBundle, assetPath, ResType.AssetBundle);
			PublicLoadingResList.Add(loadAsset);
			SelfLoadingResList.Add(loadAsset);
			//增添该资源的引用次数
			loadAsset.Reference();
			callback(request.assetBundle);
		}

		/// <summary>
		/// LoadFromFile直接加载本地AB包
		/// </summary>
		/// <param name="assetPath"></param>
		/// <returns></returns>
		public AssetBundle ABLoadFromFile(string assetPath)
		{
			//先从自身容器查找
			var loadAsset = PublicLoadingResList.Find(asset => asset.AssetKey == assetPath);
			if (loadAsset != null)
			{
				return loadAsset.Asset as AssetBundle;
			}
			//再从公共容器查找
			loadAsset = PublicLoadingResList.Find(asset => asset.AssetKey == assetPath);
			if (loadAsset != null)
			{
				return loadAsset.Asset as AssetBundle;
			}

			AssetBundle ab = AssetBundle.LoadFromFile(assetPath);
			loadAsset = new Res(ab, assetPath, ResType.AssetBundle);
			PublicLoadingResList.Add(loadAsset);
			SelfLoadingResList.Add(loadAsset);
			//增添该资源的引用次数
			loadAsset.Reference();
			return ab;
		}

		/// <summary>
		/// 从本地异步加载AB资源
		/// </summary>
		/// <param name="assetPath"></param>
		/// <param name="callback"></param>
		/// <returns></returns>
		public IEnumerator ABLoadFromFileAsync(string assetPath, Action<AssetBundle> callback)
		{
			//先从自身容器查找
			var loadAsset = SelfLoadingResList.Find(asset => asset.AssetKey == assetPath);
			if (loadAsset != null)
			{
				yield return loadAsset;
				callback(loadAsset.Asset as AssetBundle);
				yield break;
			}
			//再从公共容器查找
			loadAsset = PublicLoadingResList.Find(asset => asset.AssetKey == assetPath);
			if (loadAsset != null)
			{
				yield return loadAsset;
				callback(loadAsset.Asset as AssetBundle);
				yield break;
			}

			//获取AB包资源
			AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(assetPath);
			yield return request;
			loadAsset = new Res(request.assetBundle, assetPath, ResType.AssetBundle);
			PublicLoadingResList.Add(loadAsset);
			SelfLoadingResList.Add(loadAsset);
			//增添该资源的引用次数
			loadAsset.Reference();
			callback(request.assetBundle);
		}


		/// <summary>
		/// webrequest加载AB包
		/// </summary>
		/// <param name="assetPath"></param>
		/// <param name="callback"></param>
		/// <returns></returns>
		public IEnumerator ABLoadWebRequest(string assetPath, Action<AssetBundle> callback)
		{
			//先从自身容器查找
			var loadAsset = SelfLoadingResList.Find(asset => asset.AssetKey == assetPath);
			if (loadAsset != null)
			{
				yield return loadAsset;
				callback(loadAsset.Asset as AssetBundle);
				yield break;
			}
			//再从公共容器查找
			loadAsset = PublicLoadingResList.Find(asset => asset.AssetKey == assetPath);
			if (loadAsset != null)
			{
				yield return loadAsset;
				callback(loadAsset.Asset as AssetBundle);
				yield break;
			}

			//获取AB包资源
			UnityWebRequest webReuest = UnityWebRequestAssetBundle.GetAssetBundle(assetPath, 1);
			yield return webReuest.SendWebRequest();
			AssetBundle ab = DownloadHandlerAssetBundle.GetContent(webReuest);
			loadAsset = new Res(ab, assetPath, ResType.AssetBundle);
			PublicLoadingResList.Add(loadAsset);
			SelfLoadingResList.Add(loadAsset);
			//增添该资源的引用次数
			loadAsset.Reference();
			callback(ab);
		}

		/// <summary>
		/// 卸载当前容器资源
		/// </summary>
		public void UnloadAll()
		{
			foreach (var asset in SelfLoadingResList)
			{
				asset.Release();
			}

			SelfLoadingResList.Clear();
			SelfLoadingResList = null;
		}
	}

}
