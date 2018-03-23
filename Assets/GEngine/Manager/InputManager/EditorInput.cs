/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: EditorInput
 * Date    : 2018/01/25
 * Version : v1.0
 * Describe: 
 */

using UnityEngine;

namespace GEngine.Managers
{
    /// <summary>
    /// 鼠标操作，鼠标左键的8种行为；缩放使用中央滚轮，z表示缩放增量。
    /// </summary>
    public class EditorInput : InputBase
    {
        public EditorInput(InputManager manager) : base(manager) { }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (Input.GetMouseButtonDown(0))
            {
                CheckOnUI(-1);
                OnTouchStart(Input.mousePosition);
            }

            if (m_touchStatus != TouchStatus.None && Input.GetMouseButton(0))
            {
                OnTouch(Input.mousePosition);
            }

            if (m_touchStatus != TouchStatus.None && Input.GetMouseButtonUp(0))
            {
                OnTouchEnd(Input.mousePosition);
            }
            OnScale();
        }

        protected override void OnScale()
        {
            m_scaleDelata = Input.mousePosition;
            m_scaleDelata.z = Input.GetAxis("Mouse ScrollWheel");
            if (m_scaleDelata.z != 0)
                m_inputManager.InvokeInput(InputType.OnScale, m_scaleDelata);
        }

    }
}