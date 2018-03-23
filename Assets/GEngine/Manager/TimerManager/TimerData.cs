/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: TimerData
 * Date    : 2018/01/05
 * Version : v1.0
 * Describe: 
 */
using UnityEngine;

namespace GEngine.Managers
{
    public class TimerData
    {
        #region Members
        private uint index;
        public int times;
        public float delay;
        public float interval;
        public bool ignoreTimeScale;
        public TimerHandler handler;
        public object parm;

        private bool m_isPause;
        private bool m_isDone;

        private float m_curTimer;
        private bool m_inDelay;
        #endregion
        public uint Id { get { return index; } }
        public bool IsDone { get { return m_isDone; } }


        public void OnStart(uint index, int times, float delay, float interval, bool ignoreTimeScale, TimerHandler handler, object parm = null)
        {
            this.index = index;
            this.times = times;
            this.delay = delay;
            this.interval = interval;
            this.ignoreTimeScale = ignoreTimeScale;
            this.handler = handler;
            this.parm = parm;

            this.m_curTimer = 0;
            this.m_inDelay = true;
            this.m_isPause = false;
            this.m_isDone = false;
        }


        public void OnUpdate()
        {
            if (m_isPause || m_isDone)
                return;

            m_curTimer += ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;

            if (m_inDelay)
            {
                if (m_curTimer >= delay)
                {
                    m_curTimer -= delay > 0 ? delay : 0;
                    m_inDelay = false;
                }
                else
                {
                    return;
                }
            }


            if (m_curTimer >= interval)
            {
                m_curTimer -= interval;
                if (times > 0)
                    times--;
                if (times == 0)
                    m_isDone = true;

                if (handler != null)
                    handler(parm);
            }
        }

        /// <summary>
        /// 设置暂停
        /// </summary>
        /// <param name="pause">是否暂停</param>
        public void SetPause(bool pause)
        {
            this.m_isPause = pause;
        }


        public void OnDestroy()
        {
            this.m_isDone = true;
        }
    }

}