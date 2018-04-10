/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: MonoManager
 * Date    : 2018/01/05
 * Version : v1.0
 * Describe: 用于启用需要Mono的Manager
 */

using UnityEngine;

namespace GEngine.Managers
{
    public class MonoManager : MonoBehaviour
    {
        //用于启用Timer,其他的从TimerManager中调用
        public Callback_0 onUpdate;
        //用于启用GUI,其他的从GUIManager中调用
        public Callback_0 onDraw;

        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        void Update()
        {
            if (onUpdate != null)
                onUpdate();
        }
        
        void OnGUI()
        {
            if (onDraw != null)
                onDraw();
        }
    }
}