/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: GUIManager
 * Date    : 2018/01/05
 * Version : v1.0
 * Describe: 
 */
using System.Collections.Generic;
using GEngine.Patterns;
using UnityEngine;

namespace GEngine.Managers
{
    /// <summary>
    /// MonoBehavior的OnGUI管理器
    /// </summary>
    public class GUIManager : Manager
    {
        public new const string NAME = "GUIManager";

        #region Private or Internal
        Dictionary<string, Callback_0> m_guiDrawers;

        public override void OnRegister()
        {
            m_guiDrawers = new Dictionary<string, Callback_0>();

            MonoManager mono = GameObject.FindObjectOfType<MonoManager>();
            if (mono == null)
            {
                GameObject obj = new GameObject("GManager");
                mono = obj.AddComponent<MonoManager>();
            }
            mono.onDraw = onDraw;
        }

        void onDraw()
        {
            var enumerator = m_guiDrawers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.Value();
            }
            enumerator.Dispose();
        }

        public override void OnRemove()
        {
            MonoManager mono = GameObject.FindObjectOfType<MonoManager>();
            if (mono != null)
                mono.onDraw = null;
            if (m_guiDrawers != null)
                m_guiDrawers.Clear();
        }
        #endregion

        #region Public Methods
        public void AddDrawer(string key, Callback_0 callback)
        {
            if (m_guiDrawers.ContainsKey(key))
            {
                m_guiDrawers[key] = callback;
            }
            else
            {
                m_guiDrawers.Add(key, callback);
            }
        }

        public void RemoveDrawer(string key)
        {
            if (m_guiDrawers.ContainsKey(key))
                m_guiDrawers.Remove(key);
        }

        #endregion
    }
}