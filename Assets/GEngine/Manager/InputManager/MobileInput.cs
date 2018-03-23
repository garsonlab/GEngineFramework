/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: MobileInput
 * Date    : 2018/01/25
 * Version : v1.0
 * Describe: 
 */

using UnityEngine;

namespace GEngine.Managers
{
    /// <summary>
    /// 移动设备上操作，只响应第一触摸点
    /// </summary>
    public class MobileInput : InputBase
    {
        Touch m_touch;
        Touch m_touchScale;
        bool m_isTouchScale;
        float m_scaleDis;

        public MobileInput(InputManager manager)
            : base(manager)
        {
            m_isTouchScale = false;
            m_scaleDis = 0;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (Input.touchCount > 0)
            {
                m_touch = Input.GetTouch(0);

                if (Input.touchCount == 1 && m_touch.phase == TouchPhase.Began)
                {
                    CheckOnUI(m_touch.fingerId);
                    OnTouchStart(m_touch.position);
                }

                if (m_touchStatus != TouchStatus.None &&
                    (m_touch.phase == TouchPhase.Stationary || m_touch.phase == TouchPhase.Moved))
                {
                    OnTouch(m_touch.position);
                }

                if (m_touchStatus != TouchStatus.None &&
                    (m_touch.phase == TouchPhase.Canceled || m_touch.phase == TouchPhase.Ended))
                {
                    OnTouchEnd(m_touch.position);
                }

                OnScale();

            }

        }

        protected override void OnScale()
        {
            if ((m_touchStatus == TouchStatus.None || m_touchStatus == TouchStatus.Touch) && Input.touchCount > 1)
            {
                m_touchScale = Input.GetTouch(1);
                if (m_touch.phase == TouchPhase.Began || m_touchScale.phase == TouchPhase.Began)
                {
                    m_touchStatus = TouchStatus.None;
                    m_isTouchScale = true;
                    m_scaleDelata = (m_touch.position + m_touchScale.position) * 0.5f;
                    m_scaleDis = Vector2.Distance(m_touch.position, m_touchScale.position);
                }
            }

            if (m_isTouchScale && (m_touch.phase == TouchPhase.Moved || m_touchScale.phase == TouchPhase.Moved))
            {
                m_moveDis = Vector2.Distance(m_touch.position, m_touchScale.position);
                if (m_moveDis >= m_inputManager.m_moveTolerace)
                {
                    m_scaleDelata.z = Mathf.Clamp(m_scaleDis / m_moveDis, -1f, 1f);
                    m_scaleDis = m_moveDis;
                    m_inputManager.InvokeInput(InputType.OnScale, m_scaleDelata);
                }
            }

            if (m_isTouchScale &&
                (m_touch.phase == TouchPhase.Canceled || m_touch.phase == TouchPhase.Ended ||
                    m_touchScale.phase == TouchPhase.Canceled || m_touchScale.phase == TouchPhase.Ended))
            {
                m_isTouchScale = false;
            }
        }
    }
}