/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: ResourceManager
 * Date    : 2018/01/25
 * Version : v1.0
 * Describe: 
 */

using System;
using System.Collections.Generic;
using GEngine.Patterns;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GEngine.Managers
{
    /// <summary>
    /// 资源加载管理
    /// </summary>
    public class ResourceManager : Manager
    {
        public new const string NAME = "ResourceManager";
        public static ResourceManager Instance;

        private Dictionary<string, AssetLoader> m_assetDic;
        private List<AssetLoader> m_loadingList;
        private List<AssetLoader> m_waitList;
        private List<AssetLoader> m_deleteList; 

        private StartCoroutineHandler m_startCoroutine;

        private const int syncCount = 1;
        private bool alreadyLoad;
        private bool deleteInLoad;
        private string bundlePath;

        public override void OnRegister()
        {
            Instance = this;
            m_assetDic = new Dictionary<string, AssetLoader>();

            m_loadingList = new List<AssetLoader>();
            m_waitList = new List<AssetLoader>();
            m_deleteList = new List<AssetLoader>();

            MonoManager mono = GameObject.FindObjectOfType<MonoManager>();
            if (mono == null)
            {
                GameObject obj = new GameObject("GManager");
                mono = obj.AddComponent<MonoManager>();
            }
            m_startCoroutine = mono.StartCoroutine;
            //LoadManifest();由加载本地配置时统一调用，可填充进度条
            GarbageManager.Instance.AddCollector(UnloadUnusedBundles);
        }

        public Object LoadAsset(string assetPath)
        {
            AssetLoader loader = Load(assetPath, false);
            if (loader != null)
            {
                return loader.LoadAsset(assetPath);
            }
            return null;
        }

        public void LoadAssetSync(string assetPath, Callback_1<Object> callback, int priority = 0)
        {
            AssetLoader loader = Load(assetPath, true, priority);
            if (loader != null && callback != null)
                loader.LoadAsset(assetPath, callback);
        }

        public byte[] LoadStringByte(string assetPath)
        {
            TextAsset textAsset = LoadAsset(assetPath) as TextAsset;
            if(textAsset != null)
                return textAsset.bytes;
            return new byte[0];
        }

        public string LoadString(string assetPath)
        {
            TextAsset textAsset = LoadAsset(assetPath) as TextAsset;
            if(textAsset != null)
                return textAsset.text;
            return string.Empty;
        }

        public AssetLoader Load(string assetPath, bool isSync, int priority = 0)
        {
            AssetLoader loader = null;
            deleteInLoad = false;
            bundlePath = String.Empty;

            if (Config.BundleMode)
            {
                #region Bundle加载
                if (!FileManager.GetBundlePath(assetPath, ref bundlePath))//不存在该bundle
                    return null;

                if (m_assetDic.TryGetValue(bundlePath, out loader))
                {
                    if (!isSync && !loader.isDone)
                    {
                        //当前已有未完成的异步加载且当前换成同步加载
                        if (m_waitList.Contains(loader))
                            m_waitList.Remove(loader);
                        m_assetDic.Remove(bundlePath);
                        m_deleteList.Add(loader);
                        deleteInLoad = true;
                    }
                }

                AssetLoader asset;
                if (loader == null || deleteInLoad)
                {
                    if(!isSync)
                        asset = new BundleLoader(bundlePath);
                    else
                        asset = new BundleSyncLoader(bundlePath, priority);
                    asset.SetManager(this);

                    if(deleteInLoad)
                        asset.CopyCompleteEvents(loader);

                    loader = asset;
                    m_assetDic.Add(bundlePath, loader);

                    if (isSync)
                    {
                        m_waitList.Add(loader);
                        m_waitList.Sort();
                        LoadNext();
                    }
                    else
                    {
                        asset.Load();
                    }
                }
                #endregion
            }
            else
            {
                #region Resources加载或Editor加载
                if (m_assetDic.TryGetValue(assetPath, out loader))
                {
                    if (!isSync && !loader.isDone)
                    {
                        //当前已有未完成的异步加载且当前换成同步加载
                        if (m_waitList.Contains(loader))
                        {
                            m_waitList.Remove(loader);
                        }
                        m_assetDic.Remove(assetPath);
                        m_deleteList.Add(loader);
                        deleteInLoad = true;
                    }
                }

                AssetLoader asset;
                if (loader == null || deleteInLoad)
                {
                    if (isSync)
                        asset = new AssetSyncLoader(assetPath, priority);
                    else
                        asset = new AssetLoader(assetPath);
                    asset.SetManager(this);

                    if (deleteInLoad)//转移完成回调
                        asset.CopyCompleteEvents(loader);

                    loader = asset;
                    m_assetDic.Add(assetPath, loader);

                    if (isSync)
                    {
                        m_waitList.Add(loader);
                        m_waitList.Sort();
                        LoadNext();
                    }
                    else
                    {
                        asset.Load();
                    }
                }
                #endregion
            }
            return loader;
        }

        public void Unload(string assetPath, bool unloadFromMenery)
        {
            AssetLoader loader;
            if (!m_assetDic.TryGetValue(assetPath, out loader))
            {
                string bp = "";
                if (FileManager.GetBundlePath(assetPath, ref bp))
                {
                    m_assetDic.TryGetValue(bp, out loader);
                }
            }

            if (loader != null)
            {
                loader.UnLoad(unloadFromMenery);
                if (unloadFromMenery)
                {
                    m_assetDic.Remove(assetPath);
                    if (!loader.isDone)
                    {
                        if (m_waitList.Contains(loader))
                            m_waitList.Remove(loader);
                        m_deleteList.Add(loader);
                    }
                }
            }
        }

        public void UnloadUnusedBundles()
        {
            List<string> willUnload = new List<string>();
            foreach (var assetLoader in m_assetDic)
            {
                if(assetLoader.Value.isDone && assetLoader.Value.refCount <= 0)
                    willUnload.Add(assetLoader.Key);
            }
            foreach (var unload in willUnload)
            {
                Unload(unload, true);
            }
        }

        private void LoadNext()
        {
            if(m_loadingList.Count >= syncCount || m_waitList.Count == 0)
                return;

            AssetLoader loader = m_waitList[0];
            m_waitList.RemoveAt(0);
            m_loadingList.Add(loader);
            m_startCoroutine(loader.SyncLoad());
        }

        //仅供AssetLoader加载完成自行调用
        internal void OnLoadComplete(AssetLoader loader)
        {
            if (m_loadingList.Contains(loader))//从加载列表中移除
            {
                m_loadingList.Remove(loader);
                LoadNext();
            }
            if (m_deleteList.Contains(loader)) //存在已删除列表中
            {
                m_deleteList.Remove(loader);
                loader.UnLoad(true);
            }
            else
                loader.InvokeComplete();
        }

        //仅供Bundle加载器调用加载所需依赖
        internal void LoadAllDependencies(string assetPath)
        {
            List<JsonNode> deps = FileManager.GetDependencies(assetPath);
            foreach (var dep in deps)
            {
                BundleSyncLoader loader = Load(dep.ToString(), true) as BundleSyncLoader;
                if (loader != null)
                    loader.RefSet(1);
            }
        }
        //取消依赖的加载
        internal void UnLoadAllDependencies(string assetPath)
        {
            List<JsonNode> deps = FileManager.GetDependencies(assetPath);
            foreach (var dep in deps)
            {
                AssetLoader loader;
                if (m_assetDic.TryGetValue(dep.ToString(), out loader))
                {
                    loader.UnLoad(false);
                    if(loader.refCount <= 0)
                        Unload(dep.ToString(), true);
                }
            }
        }

        //加载Bundle数据文件
        void LoadManifest()
        {
            if(!Config.BundleMode)
                return;

            string path = FileManager.GetBundleRealPath(Config.BundleManifest);
            FileLoadAgent agent = new FileLoadAgent();
            m_startCoroutine(agent.LoadSync(path, bytes =>
            {
                FileManager.LoadMenifest(bytes);
            }, error =>
            {
                GLog.E(error);
            }));
        }

    }
}
