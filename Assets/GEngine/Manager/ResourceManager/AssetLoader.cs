/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: AssetLoader
 * Date    : 2018/02/27
 * Version : v1.0
 * Describe: 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GEngine.Managers
{
    /// <summary>
    /// 资源加载
    /// </summary>
    public class AssetLoader : IComparable<AssetLoader>
    {
        public enum LoadState
        {
            None = 0,
            Loading,
            Error,
            Complete
        }

        private int priority;

        protected ResourceManager m_manager;
        protected Dictionary<string, List<Callback_1<Object>>> m_onCompletes;
        protected string m_assetPath;
        protected LoadState m_state;
        protected int m_refCount;

        protected Object m_asset;

        public virtual bool isDone { get { return true; } }
        public virtual float progress { get { return 1; } }
        public int refCount { get { return m_refCount; } }
        public LoadState state { get { return m_state; } }


        public AssetLoader(string path, int priority = 0)
        {
            m_assetPath = path;
            this.priority = priority;
            m_state = LoadState.None;
            m_refCount = 0;
            m_onCompletes = new Dictionary<string, List<Callback_1<Object>>>();
        }

        public virtual void Load()
        {
#if UNITY_EDITOR
            m_asset = UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(m_assetPath);
#else
            string resPath = string.Empty;
            if (FileManager.GetResourcePath(m_assetPath, ref resPath))
            {
                try
                {
                    m_asset = Resources.Load(resPath);
                    m_state = LoadState.Complete;
                }
                catch (Exception e)
                {
                    Debug.Log(e.ToString());
                    m_state = LoadState.Error;
                }
            }
#endif
            InvokeComplete();
        }

        public virtual IEnumerator SyncLoad()
        {
            yield return null;
        }

        protected virtual void LoadDepencies()
        {
            
        }


        public int CompareTo(AssetLoader other)
        {
            return priority <= other.priority ? 1 : -1;
        }

        public void SetManager(ResourceManager manager)
        {
            m_manager = manager;
        }


        protected void AddCompleteEvent(string assetPath, Callback_1<Object> callback)
        {
            if(callback == null)
                return;

            List<Callback_1<Object>> list;
            if (!m_onCompletes.TryGetValue(assetPath, out list))
            {
                list = new List<Callback_1<Object>>();
                m_onCompletes.Add(assetPath, list);
            }
            list.Add(callback);
        }

        public void CopyCompleteEvents(AssetLoader loader)
        {
            if (loader != null)
            {
                var completes = loader.m_onCompletes;
                foreach (var coms in completes)
                {
                    List<Callback_1<Object>> list;
                    if (!m_onCompletes.TryGetValue(coms.Key, out list))
                    {
                        list = new List<Callback_1<Object>>();
                        m_onCompletes.Add(coms.Key, list);
                    }
                    list.AddRange(coms.Value);
                }
                loader.m_onCompletes.Clear();
            }
        }
        public virtual void InvokeComplete()
        {
            foreach (var complete in m_onCompletes)
            {
                foreach (var callback1 in complete.Value)
                {
                    callback1(LoadAsset());
                }
            }
            m_onCompletes.Clear();
        }

        public virtual void UnLoad(bool unloadFromMenery)
        {
            if (unloadFromMenery && m_asset)
                GameObject.Destroy(m_asset);
        }

        public virtual Object LoadAsset(string assetPath = "")
        {
            if (m_asset != null)
                m_refCount++;
            return m_asset;
        }

        public virtual void LoadAsset(string assetPath, Callback_1<Object> callback)
        {
            
        }
    }

    public class AssetSyncLoader : AssetLoader
    {
        public AssetSyncLoader(string path, int priority) : base(path, priority) { }
        private ResourceRequest request;
        private bool m_isDone = false;

        public override bool isDone
        {
            get { return m_isDone; }
        }

        public override float progress
        {
            get
            {
                if (request == null)
                    return 1;
                else
                    return request.progress;
            }
        }


        public override IEnumerator SyncLoad()
        {
            string resPath = String.Empty;
            if (FileManager.GetResourcePath(m_assetPath, ref resPath))
            {
                m_isDone = false;
                m_state = LoadState.Loading;
                request = Resources.LoadAsync(resPath);
                yield return request;
                m_asset = request.asset;
                m_state = LoadState.Complete;
                m_manager.OnLoadComplete(this);
            }
            else
            {
                m_isDone = true;
                m_state = LoadState.Error;
                m_asset = null;
                yield return null;
                m_manager.OnLoadComplete(this);
            }
        }

        public override void LoadAsset(string assetPath, Callback_1<Object> callback)
        {
            if (isDone)
                callback(base.LoadAsset());
            else
                AddCompleteEvent(m_assetPath, callback);
        }
    }

    public class BundleLoader : AssetLoader
    {
        protected AssetBundle bundle;
        private Dictionary<string, Object> assetMap= new Dictionary<string, Object>();
        public BundleLoader(string bundlePath, int priority = 0) : base(bundlePath, priority) { }

        public override void Load()
        {
            string bundleName = FileManager.MD5(m_assetPath);
            string realPath = FileManager.GetBundleRealPath(bundleName);
            try
            {
                bundle = AssetBundle.LoadFromFile(realPath);
                m_state = LoadState.Complete;
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                m_state = LoadState.Error;
            }
            m_manager.OnLoadComplete(this);
        }

        public override void InvokeComplete()
        {
            m_manager.LoadAllDependencies(m_assetPath);
            foreach (var complete in m_onCompletes)
            {
                foreach (var callback in complete.Value)
                {
                    callback(LoadAsset(complete.Key));
                }
            }
            m_onCompletes.Clear();
        }

        public override void UnLoad(bool unloadFromMenery)
        {
            m_refCount = m_refCount > 0 ? m_refCount-1 : 0;
            if(bundle != null)
                bundle.Unload(unloadFromMenery);

            if (unloadFromMenery)
            {
                m_manager.UnLoadAllDependencies(m_assetPath);
            }
        }

        public override Object LoadAsset(string assetPath)
        {
            Object obj = null;
            if (!assetMap.TryGetValue(assetPath, out obj))
            {
                if (bundle == null)
                    return null;
                obj = bundle.LoadAsset(assetPath);
                assetMap.Add(assetPath, obj);
            }
            if(obj != null)
                m_refCount++;
            return obj;
        }
    }

    public class BundleSyncLoader : BundleLoader
    {
        private AssetBundleCreateRequest request;
        private bool m_isDone = false;
        public BundleSyncLoader(string bundlePath, int priority) : base(bundlePath, priority) { }


        public override bool isDone
        {
            get { return m_isDone; }
        }

        public override float progress
        {
            get
            {
                if (request == null)
                {
                    if (state == LoadState.Error)
                        return 1;
                    return 0;
                }
                else
                    return request.progress;
            }
        }


        public override IEnumerator SyncLoad()
        {
            string bundleName = FileManager.MD5(m_assetPath);
            string realPath = FileManager.GetBundleRealPath(bundleName);
            request = AssetBundle.LoadFromFileAsync(realPath);
            m_isDone = false;
            m_state = LoadState.Loading;
            yield return request;
            bundle = request.assetBundle;
            m_isDone = true;
            m_state = LoadState.Complete;
            m_manager.OnLoadComplete(this);
        }

        //依赖变化
        public void RefSet(int refAdd)
        {
            refAdd = Mathf.Clamp(refAdd, -1, 1);
            m_refCount += refAdd;
            if (m_refCount <= 0)
                m_refCount = 0;
        }

        public override void LoadAsset(string assetPath, Callback_1<Object> callback)
        {
            if (m_isDone)
                callback(LoadAsset(assetPath));
            else
                AddCompleteEvent(assetPath, callback);
        }

    }

}