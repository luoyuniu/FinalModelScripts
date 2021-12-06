namespace Framework
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;
    using UnityEngine.Networking;

    public class ResManager : MonoSingleton<ResManager>
    {
        public override void OnSingletonInit()
        {
            if (m_Table == null)
            {
                m_Table = new ResTable();
            }
            //加载文件
            string fileContent = GetConfigFile();

            //解析文件 string--》Dictionary<string,string>
            BuildMap(fileContent);

            SafeObjectPool<ResLoader>.Instance.Init(30, 5);
        }

        //资源存储表
        private ResTable m_Table = new ResTable();

        //资源路径
        private static Dictionary<string, string> configMap;

        #region 加载方法
        #region Resources目录下资源加载
        /// <summary>
        /// 单个资源直接加载
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="resSearchKey">搜索器</param>
        /// <returns>资源</returns>
        public T AssetLoad<T>(ResSearchKey resSearchKey) where T : UnityEngine.Object
        {
            Res res = m_Table.GetResBySearchKey(resSearchKey);

            if (res == null)
            {
                ResLoader resLoader = ResLoader.Allocate();

                res = resLoader.AssetLoad(resSearchKey);

                m_Table.Add(res);

                resLoader.Recycle2Cache();
            }

            return res.Asset as T;
        }

        /// <summary>
        /// 单个资源异步加载
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="resSearchKey">搜索器</param>
        /// <returns>资源</returns>
        public void AssetLoadAsync<T>(ResSearchKey resSearchKey, Action<T> callback) where T : UnityEngine.Object
        { 
            Res res = m_Table.GetResBySearchKey(resSearchKey);

            if (res == null)
            {
                ResLoader resLoader = ResLoader.Allocate();

                StartCoroutine(resLoader.AssetLoadAsync(resSearchKey, (res) => {
                    m_Table.Add(res);
                    callback?.Invoke(res.Asset as T);
                    resLoader.Recycle2Cache(); }));
            }
            else
            {
                callback?.Invoke(res.Asset as T);
            }
        }

        #endregion

        #region 从内存加载AB资源
        /// <summary>
        /// 从内存直接加载单个AB资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="resSearchKey">搜索器</param>
        /// <returns>资源</returns>
        public T ABLoadFromMemory<T>(ResSearchKey resSearchKey) where T : UnityEngine.Object
        {
            Res res = m_Table.GetResBySearchKey(resSearchKey);

            if (res == null)
            {
                ResLoader resLoader = ResLoader.Allocate();
                ResSearchKey abrsk = ResSearchKey.Allocate(resSearchKey.OwnerBundle, typeof(AssetBundle));
                AssetBundle ab = resLoader.ABLoadFromMemory(abrsk);
                res = new Res(resSearchKey.AssetName, resSearchKey.OwnerBundle, resSearchKey.AssetType, ab.LoadAsset<T>(resSearchKey.AssetName));
                m_Table.Add(res);
                resLoader.Recycle2Cache();
            }

            return res.Asset as T;
        }

        /// <summary>
        /// 从内存异步加载单个AB资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resSearchKey"></param>
        /// <param name="callback"></param>
        public void ABLoadFromMemoryAsync<T>(ResSearchKey resSearchKey, Action<T> callback) where T : UnityEngine.Object
        {
            Res res = m_Table.GetResBySearchKey(resSearchKey);

            if (res == null)
            {
                ResLoader resLoader = ResLoader.Allocate();
                ResSearchKey abrsk = ResSearchKey.Allocate(resSearchKey.OwnerBundle, typeof(AssetBundle));
                StartCoroutine(resLoader.ABLoadFromMemoryAsync(abrsk, (ab) => {
                    var asset = ab.LoadAsset(resSearchKey.AssetName, resSearchKey.AssetType);
                    res = new Res(resSearchKey.AssetName, resSearchKey.OwnerBundle, resSearchKey.AssetType, asset);
                    m_Table.Add(res);
                    resLoader.Recycle2Cache();
                    callback?.Invoke(res.Asset as T);
                }));
            }
        }
        #endregion

        #region  从磁盘加载AB资源
        /// <summary>
        /// 从磁盘直接加载单个AB资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resSearchKey"></param>
        /// <returns></returns>
        public T ABLoadFromFile<T>(ResSearchKey resSearchKey) where T : UnityEngine.Object
        {
            Res res = m_Table.GetResBySearchKey(resSearchKey);

            if (res == null)
            {
                ResLoader resLoader = ResLoader.Allocate();
                ResSearchKey abrsk = ResSearchKey.Allocate(resSearchKey.OwnerBundle, typeof(AssetBundle));
                AssetBundle ab = resLoader.ABLoadFromFile(abrsk);
                res = new Res(resSearchKey.AssetName, resSearchKey.OwnerBundle, resSearchKey.AssetType, ab.LoadAsset<T>(resSearchKey.AssetName));
                m_Table.Add(res);
                resLoader.Recycle2Cache();
            }

            return res.Asset as T;
        }

        /// <summary>
        /// 从内存异步加载单个AB资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public void ABLoadFromFileAsync<T>(ResSearchKey resSearchKey, Action<T> callback) where T : UnityEngine.Object
        {
            Res res = m_Table.GetResBySearchKey(resSearchKey);

            if (res == null)
            {
                ResLoader resLoader = ResLoader.Allocate();
                ResSearchKey abrsk = ResSearchKey.Allocate(resSearchKey.OwnerBundle, typeof(AssetBundle));

                StartCoroutine(resLoader.ABLoadFromFileAsync(abrsk, (ab) => {
                    var asset = ab.LoadAsset(resSearchKey.AssetName, resSearchKey.AssetType);
                    res = new Res(resSearchKey.AssetName, resSearchKey.OwnerBundle, resSearchKey.AssetType, asset);
                    m_Table.Add(res);
                    resLoader.Recycle2Cache();
                    callback?.Invoke(res.Asset as T);
                }));
            }
        }
        #endregion

        #region 网络请求加载AB资源
        /// <summary>
        /// 网络请求加载单个AB资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="name">资源名</param>
        /// <returns>资源</returns>
        public void ABLoadWebRequest<T>(ResSearchKey resSearchKey, Action<T> callback) where T : UnityEngine.Object
        {
            Res res = m_Table.GetResBySearchKey(resSearchKey);

            if (res == null)
            {
                ResLoader resLoader = ResLoader.Allocate();
                ResSearchKey abrsk = ResSearchKey.Allocate(resSearchKey.OwnerBundle, typeof(AssetBundle));

                StartCoroutine(resLoader.ABLoadWebRequest(abrsk, (ab) => {
                    var asset = ab.LoadAsset(resSearchKey.AssetName, resSearchKey.AssetType);
                    res = new Res(resSearchKey.AssetName, resSearchKey.OwnerBundle, resSearchKey.AssetType, asset);
                    m_Table.Add(res);
                    resLoader.Recycle2Cache();
                    callback?.Invoke(res.Asset as T);
                }));
            }
        }
        #endregion
        #endregion

        #region 其他方法
        /// <summary>
        /// 读取本地文件  唯一的方式
        /// </summary>
        /// <returns></returns>
        public static string GetConfigFile()//加载文件
        {
            string url = "";
            //在编辑器下读取
#if UNITY_EDITOR|| UNITY_STANDALONE
            url = "file://" + Application.dataPath + "/StreamingAssets/ConfigMap.txt";
            //url =  Application.streamingAssetsPath + "/ConfigMap.txt";

#elif UNITY_IOS  //在iphone端读取
            url = "file://" + Application.dataPath + "/Raw/ConfigMap.txt";

#elif UNITY_ANDROID  //在android端读取
            url = "jar:file://" + Application.dataPath + "!/assets/ConfigMap.txt";
#endif

            UnityWebRequest www = new UnityWebRequest(url);

            while (true)
            {
                if (www.isDone)
                {
                    return www.downloadHandler.text;
                }
            }
        }

        private static void BuildMap(string fileContent)//解析文件 string--》Dictionary<string,string>
        {
            configMap = new Dictionary<string, string>();
            // 格式 ： 文件名=路径\r\n文件名=路径
            using (StringReader reader = new StringReader(fileContent))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] keyvalue = line.Split('=');
                    configMap.Add(keyvalue[0], keyvalue[1]);
                }
                //while (line != null)
                //{
                //    string[] keyValue = line.Split('=');
                //    //keyValue[0]文件名 keyValue[1]路径
                //    configMap.Add(keyValue[0], keyValue[1]);
                //    line = reader.ReadLine();
                //}
            }//当程序退出using代码块，将自动调用reader.Dispose()方法
        }
        #endregion
    }
}
