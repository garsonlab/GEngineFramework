/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: UIManager
 * Date    : 2018/03/09
 * Version : v1.0
 * Describe: 
 */

using System.Collections.Generic;
using GEngine.Patterns;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GEngine.Managers
{
    /// <summary>
    /// UI管理器
    /// </summary>
    public class UIManager : Manager<UIManager>
    {
        public new const string NAME = "UIManager";
        public Transform uiRoot;
        public Transform hudRoot;
        public Transform fixedRoot;
        public Transform normalRoot;
        public Transform popupRoot;
        public Camera uiCamera;

        private Dictionary<string, Panel> m_allPages;
        private List<Panel> m_showing;

        public override void OnRegister()
        {
            InitCanvasAndCamera();
            UIScaler.ResetScaler();
            m_allPages = new Dictionary<string, Panel>();
            m_showing = new List<Panel>();
        }

        public void Show<T>() where T : Panel, new()
        {
            Show<T>(null, null);
        }
        public void Show<T>(Callback_0 callback) where T : Panel, new()
        {
            Show<T>(null, callback);
        }
        public void Show<T>(object parm) where T : Panel, new()
        {
            Show<T>(parm, null);
        }
        public void Show<T>(object parm, Callback_0 callback) where T : Panel, new()
        {
            string name = typeof(T).ToString();
            Panel panel;
            if (!m_allPages.TryGetValue(name, out panel))
            {
                panel = new T();
                m_allPages.Add(name, panel);
            }
            panel.Show(parm, callback);

            if (panel.panelMode == PanelMode.HideSameLayer)
            {
                int count = m_showing.Count;
                for (int i = count - 1; i >= 0; i--)
                {
                    if (m_showing[i].panelType == panel.panelType)
                    {
                        m_showing[i].Close();
                        m_showing.RemoveAt(i);
                    }
                }
            }
            else if (panel.panelMode == PanelMode.HideOthers)
            {
                int count = m_showing.Count;
                for (int i = count - 1; i >= 0; i--)
                {
                    m_showing[i].Close();
                }
                m_showing.Clear();
            }
            m_showing.Add(panel);
        }

        public T GetPanel<T>() where T : Panel
        {
            string name = typeof(T).ToString();
            return (T) GetPanel(name);
        }

        public Panel GetPanel(string name)
        {
            Panel panel;
            if (!m_allPages.TryGetValue(name, out panel))
            {
                panel = null;
            }
            return panel;
        }

        public void Close<T>() where T : Panel
        {
            Close(typeof(T).ToString());
        }
        public void Close(string name)
        {
            Panel panel;
            if (m_allPages.TryGetValue(name, out panel))
            {
                panel.Close();
            }
        }

        public void Destroy<T>() where T : Panel
        {
            Destroy(typeof(T).ToString());
        }
        public void Destroy(string name)
        {
            Panel panel;
            if (m_allPages.TryGetValue(name, out panel))
            {
                panel.Destroy();
                m_allPages.Remove(name);
            }
        }

        public void Localize()
        {
            var er = m_allPages.GetEnumerator();
            while (er.MoveNext())
            {
                er.Current.Value.Localize();
            }
            er.Dispose();
        }

        public void Resize()
        {
            UIScaler.ResetScaler();
            var er = m_allPages.GetEnumerator();
            while (er.MoveNext())
            {
                er.Current.Value.Resize();
            }
            er.Dispose();
        }

        private void InitCanvasAndCamera()
        {
            Canvas canvas = GameObject.FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                GameObject obj = new GameObject();
                obj.layer = LayerMask.NameToLayer("UI");
                obj.AddComponent<RectTransform>();

                canvas = obj.AddComponent<Canvas>();
                obj.AddComponent<GraphicRaycaster>();
            }
            uiRoot = canvas.transform;
            canvas.gameObject.name = "UIRoot";
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.pixelPerfect = true;

            uiCamera = canvas.worldCamera;
            if (uiCamera == null)
            {
                GameObject cam = new GameObject();
                uiCamera = cam.AddComponent<Camera>();
                canvas.worldCamera = uiCamera;
            }
            uiCamera.gameObject.name = "UICamera";
            uiCamera.gameObject.layer = LayerMask.NameToLayer("UI");
            uiCamera.transform.SetParent(uiRoot);
            uiCamera.transform.localPosition = new Vector3(0, 0, -100);
            uiCamera.clearFlags = CameraClearFlags.Depth;//以后换成Dep
            uiCamera.orthographic = true;
            uiCamera.nearClipPlane = -50;
            uiCamera.farClipPlane = 200;
            uiCamera.cullingMask = 1 << 5;


            EventSystem eventSystem = GameObject.FindObjectOfType<EventSystem>();
            if (eventSystem == null)
            {
                GameObject evt = new GameObject();
                evt.layer = LayerMask.NameToLayer("UI");
                eventSystem = evt.AddComponent<EventSystem>();
                evt.AddComponent<StandaloneInputModule>();

            }
            eventSystem.gameObject.name = "EventSystem";

            hudRoot = CreateLayer("Hud", 1);
            fixedRoot = CreateLayer("Fixed", 10);
            normalRoot = CreateLayer("Normal", 20);
            popupRoot = CreateLayer("Popup", 30);
        }

        private Transform CreateLayer(string layer, int sibling)
        {
            Transform root = uiRoot.Find(layer);
            if (root == null)
            {
                GameObject obj = new GameObject(layer);
                obj.layer = LayerMask.NameToLayer("UI");
                obj.transform.SetParent(uiRoot);
                RectTransform rect = obj.AddComponent<RectTransform>();
                rect.anchorMin = Vector2.one*0.5f;
                rect.anchorMax = Vector2.one*0.5f;
                rect.sizeDelta = new Vector2(Config.DesignWidth, Config.DesignHeight);
                rect.localPosition = Vector3.zero;
                rect.localScale = Vector3.one;
                root = rect;
            }

            root.SetSiblingIndex(sibling);
            return root;
        }

    }
}