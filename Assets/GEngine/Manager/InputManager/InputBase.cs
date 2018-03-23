/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: InputBase
 * Date    : 2018/01/25
 * Version : v1.0
 * Describe: 
 */

using UnityEngine;

namespace GEngine.Managers
{
    public class InputBase
    {
        protected bool m_isTouchOnUI;
        protected TouchStatus m_touchStatus; //状态
        protected Vector3 m_pointPos; //点击位置
        protected Vector3 m_curPos; //当前点击位置
        protected Vector3 m_scaleDelata; //缩放位置比例

        protected InputManager m_inputManager;//输入管理类
        protected float m_longPressTimer; //长按计时器
        protected float m_moveDis; //移动距离

        public InputBase(InputManager manager)
        {
            m_inputManager = manager;
            Reset();
        }

        /// <summary>
        /// 只能由InputManager调用的更新
        /// </summary>
        public virtual void OnUpdate()
        {
            if (!m_inputManager.IsActive)
                return;

        }

        /// <summary>
        /// 设置是否可用
        /// </summary>
        /// <param name="active"></param>
        public void SetActive(bool active)
        {
            if (!active)
                Reset();
        }

        /// <summary>
        /// 是否点击在UI上
        /// </summary>
        public bool IsTouchOnUI
        {
            get { return m_isTouchOnUI; }
        }


        /// <summary>
        /// 重置状态
        /// </summary>
        protected virtual void Reset()
        {
            m_isTouchOnUI = false;
            m_touchStatus = TouchStatus.None;
            m_longPressTimer = 0;
        }

        /// <summary>
        /// 开始点击
        /// </summary>
        /// <param name="pointPos"></param>
        protected virtual void OnTouchStart(Vector3 curPos)
        {
            m_touchStatus = TouchStatus.Touch;
            m_pointPos = curPos;
            m_curPos = curPos;
            m_longPressTimer = 0;

            m_inputManager.InvokeInput(InputType.OnTouchBegin, m_pointPos);
        }

        /// <summary>
        /// 点击中
        /// </summary>
        /// <param name="pointPos"></param>
        protected virtual void OnTouch(Vector3 curPos)
        {
            m_longPressTimer += Time.deltaTime;
            m_curPos = curPos;

            if (m_touchStatus != TouchStatus.Moving)
            {
                m_moveDis = Vector2.Distance(m_pointPos, m_curPos);
                if (m_moveDis >= m_inputManager.m_moveTolerace)
                {
                    m_touchStatus = TouchStatus.Moving;
                    m_inputManager.InvokeInput(InputType.OnMoveBegin, m_curPos);
                }

                if (m_touchStatus == TouchStatus.Touch && m_longPressTimer >= m_inputManager.m_longPressSpan)
                {
                    m_touchStatus = TouchStatus.LongPress;
                    m_inputManager.InvokeInput(InputType.OnLongPress, m_curPos);
                }
            }
            else
            {
                m_inputManager.InvokeInput(InputType.OnMove, m_curPos);
            }
        }

        protected virtual void OnTouchEnd(Vector3 curPos)
        {
            m_curPos = curPos;

            if (m_touchStatus == TouchStatus.Moving)
                m_inputManager.InvokeInput(InputType.OnMoveEnd, m_curPos);
            if (m_touchStatus == TouchStatus.LongPress)
                m_inputManager.InvokeInput(InputType.OnEndLongPress, m_curPos);
            if (m_touchStatus == TouchStatus.Touch)
                m_inputManager.InvokeInput(InputType.OnClick, m_curPos);

            m_inputManager.InvokeInput(InputType.OnTouchEnd, m_curPos);

            m_touchStatus = TouchStatus.None;
            m_isTouchOnUI = false;
        }

        protected virtual void OnScale()
        {

        }

        protected virtual void CheckOnUI(int fingerId)
        {
            if (UnityEngine.EventSystems.EventSystem.current == null)
                return;
            if (fingerId < 0)
                m_isTouchOnUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
            else
                m_isTouchOnUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(fingerId);
        }

        /// <summary>
        /// 手势状态
        /// </summary>
        protected enum TouchStatus
        {
            /// <summary>
            /// 无状态
            /// </summary>
            None,

            /// <summary>
            /// 刚刚按下
            /// </summary>
            Touch,

            /// <summary>
            /// 长按
            /// </summary>
            LongPress,

            /// <summary>
            /// 移动
            /// </summary>
            Moving,
        }
    }
}