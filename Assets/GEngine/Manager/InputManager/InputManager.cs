/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: InputManager
 * Date    : 2018/01/25
 * Version : v1.0
 * Describe: 
 */

using GEngine.Patterns;
using UnityEngine;

namespace GEngine.Managers
{
    public enum InputType
    {
        /// <summary>
        /// 触摸开始
        /// </summary>
        OnTouchBegin = 0,
        /// <summary>
        /// 长按
        /// </summary>
        OnLongPress = 1,
        /// <summary>
        /// 开始移动
        /// </summary>
        OnMoveBegin,
        /// <summary>
        /// 移动中
        /// </summary>
        OnMove,
        /// <summary>
        /// 移动结束
        /// </summary>
        OnMoveEnd,
        /// <summary>
        /// 点击
        /// </summary>
        OnClick,
        /// <summary>
        /// 触摸结束
        /// </summary>
        OnTouchEnd,
        /// <summary>
        /// 缩放，回调x,y表示缩放初始中心点，z为当前缩放增量
        /// </summary>
        OnScale,
        /// <summary>
        /// 手指抬起时仍未长按状态，和OnClick不能同时响应
        /// </summary>
        OnEndLongPress,
    }

    /// <summary>
    /// 输入控制管理器， 支持鼠标、点触的八种单点操作及缩放
    /// </summary>
    public class InputManager : Manager<InputManager>
    {
        public new const string NAME = "InputManager";

        /// <summary>
        /// 移动容忍度：移动超过多少算开始滑动
        /// </summary>
        public readonly float m_moveTolerace = 10f;
        /// <summary>
        /// 长按间隔：按住多久开始处理成长按事件
        /// </summary>
        public readonly float m_longPressSpan = 0.5f;

        #region Members

        private uint m_timerId;
        private InputBase m_input;
        private bool m_isActive;

        private event Callback_1<Vector3> m_OnTouchBegin;
        private event Callback_1<Vector3> m_OnLongPress;
        private event Callback_1<Vector3> m_OnMoveBegin;
        private event Callback_1<Vector3> m_OnMove;
        private event Callback_1<Vector3> m_OnMoveEnd;
        private event Callback_1<Vector3> m_OnClick;
        private event Callback_1<Vector3> m_OnTouchEnd;
        private event Callback_1<Vector3> m_OnScale;
        private event Callback_1<Vector3> m_OnEndLongPress;
        #endregion

        /// <summary>
        /// 是否可用
        /// </summary>
        public bool IsActive
        {
            get { return m_isActive; }
            set
            {
                m_isActive = value;
                m_input.SetActive(value);
            }
        }
        /// <summary>
        /// 是否点击在ui上
        /// </summary>
        public bool IsTouchOnUI
        {
            get { return m_input.IsTouchOnUI; }
        }

        public void AddListener(InputType inputType, Callback_1<Vector3> callback)
        {
            switch (inputType)
            {
                case InputType.OnTouchBegin:
                    m_OnTouchBegin -= callback;
                    m_OnTouchBegin += callback;
                    break;
                case InputType.OnLongPress:
                    m_OnLongPress -= callback;
                    m_OnLongPress += callback;
                    break;
                case InputType.OnMoveBegin:
                    m_OnMoveBegin -= callback;
                    m_OnMoveBegin += callback;
                    break;
                case InputType.OnMove:
                    m_OnMove -= callback;
                    m_OnMove += callback;
                    break;
                case InputType.OnMoveEnd:
                    m_OnMoveEnd -= callback;
                    m_OnMoveEnd += callback;
                    break;
                case InputType.OnScale:
                    m_OnScale -= callback;
                    m_OnScale += callback;
                    break;
                case InputType.OnEndLongPress:
                    m_OnEndLongPress -= callback;
                    m_OnEndLongPress += callback;
                    break;
                case InputType.OnClick:
                    m_OnClick -= callback;
                    m_OnClick += callback;
                    break;
                case InputType.OnTouchEnd:
                    m_OnTouchEnd -= callback;
                    m_OnTouchEnd += callback;
                    break;
            }
        }

        public void RemoveListener(InputType inputType, Callback_1<Vector3> callback)
        {
            switch (inputType)
            {
                case InputType.OnTouchBegin:
                    m_OnTouchBegin -= callback;
                    break;
                case InputType.OnLongPress:
                    m_OnLongPress -= callback;
                    break;
                case InputType.OnMoveBegin:
                    m_OnMoveBegin -= callback;
                    break;
                case InputType.OnMove:
                    m_OnMove -= callback;
                    break;
                case InputType.OnMoveEnd:
                    m_OnMoveEnd -= callback;
                    break;
                case InputType.OnScale:
                    m_OnScale -= callback;
                    break;
                case InputType.OnEndLongPress:
                    m_OnEndLongPress -= callback;
                    break;
                case InputType.OnClick:
                    m_OnClick -= callback;
                    break;
                case InputType.OnTouchEnd:
                    m_OnTouchEnd -= callback;
                    break;
            }
        }

        public void RemoveListeners(InputType inputType)
        {
            switch (inputType)
            {
                case InputType.OnTouchBegin:
                    m_OnTouchBegin = null;
                    break;
                case InputType.OnLongPress:
                    m_OnLongPress = null;
                    break;
                case InputType.OnMoveBegin:
                    m_OnMoveBegin = null;
                    break;
                case InputType.OnMove:
                    m_OnMove = null;
                    break;
                case InputType.OnMoveEnd:
                    m_OnMoveEnd = null;
                    break;
                case InputType.OnScale:
                    m_OnScale = null;
                    break;
                case InputType.OnEndLongPress:
                    m_OnEndLongPress = null;
                    break;
                case InputType.OnClick:
                    m_OnClick = null;
                    break;
                case InputType.OnTouchEnd:
                    m_OnTouchEnd = null;
                    break;
            }
        }


        #region Private and Internal Methods
        public override void OnRegister()
        {
            if (Application.isMobilePlatform)
                m_input = new MobileInput(this);
            else
                m_input = new EditorInput(this);

            m_isActive = true;
            var timer = TimerManager.RepeatedCall(-1, 0, 0, false, OnUpdate);
            m_timerId = timer.Id;
        }

        private void OnUpdate(object parm)
        {
            m_input.OnUpdate();
        }

        internal void InvokeInput(InputType inputType, Vector3 position)
        {
            switch (inputType)
            {
                case InputType.OnTouchBegin:
                    if (m_OnTouchBegin != null)
                        m_OnTouchBegin(position);
                    break;
                case InputType.OnLongPress:
                    if (m_OnLongPress != null)
                        m_OnLongPress(position);
                    break;
                case InputType.OnMoveBegin:
                    if (m_OnMoveBegin != null)
                        m_OnMoveBegin(position);
                    break;
                case InputType.OnMove:
                    if (m_OnMove != null)
                        m_OnMove(position);
                    break;
                case InputType.OnMoveEnd:
                    if (m_OnMoveEnd != null)
                        m_OnMoveEnd(position);
                    break;
                case InputType.OnScale:
                    if (m_OnScale != null)
                        m_OnScale(position);
                    break;
                case InputType.OnEndLongPress:
                    if (m_OnEndLongPress != null)
                        m_OnEndLongPress(position);
                    break;
                case InputType.OnClick:
                    if (m_OnClick != null)
                        m_OnClick(position);
                    break;
                case InputType.OnTouchEnd:
                    if (m_OnTouchEnd != null)
                        m_OnTouchEnd(position);
                    break;
            }
        }

        public override void OnRemove()
        {
            TimerManager.CancelCallback(m_timerId);
        }

        #endregion
    }
}