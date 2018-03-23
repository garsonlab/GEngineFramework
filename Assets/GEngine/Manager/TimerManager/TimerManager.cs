/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: TimerManager
 * Date    : 2018/01/05
 * Version : v1.0
 * Describe: 
 */
using System.Collections.Generic;
using GEngine.Patterns;
using UnityEngine;

namespace GEngine.Managers
{
    public class TimerManager : Manager, ITimer
    {
        #region Members
        public new const string NAME = "TimerManager";
        Dictionary<uint, TimerData> m_timers;
        List<TimerData> m_toRemove;
        List<TimerData> m_toAdd;
        ObjectPool<TimerData> m_timerPool;
        uint timerId;
        bool m_active;
        #endregion
        public bool Active { get { return m_active; } set { m_active = value; } }

        public override void OnRegister()
        {
            m_timers = new Dictionary<uint, TimerData>();
            m_toRemove = new List<TimerData>();
            m_toAdd = new List<TimerData>();
            m_timerPool = new ObjectPool<TimerData>(() => new TimerData(), null, null, null);
            timerId = 0;
            m_active = true;

            MonoManager mono = GameObject.FindObjectOfType<MonoManager>();
            if (mono == null)
            {
                GameObject obj = new GameObject("GManager");
                mono = obj.AddComponent<MonoManager>();
            }
            mono.onUpdate = OnUpdate;
        }

        void OnUpdate()
        {
            if(!m_active)
                return;

            if (m_timers.Count > 0)
            {
                var enumerator = m_timers.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.Value.IsDone)
                        m_toRemove.Add(enumerator.Current.Value);
                    else
                        enumerator.Current.Value.OnUpdate();
                }
                enumerator.Dispose();
            }

            int removeLen = m_toRemove.Count;
            if (removeLen > 0)
            {
                for (int i = 0; i < removeLen; i++)
                {
                    TimerData timer = m_toRemove[i];
                    m_timers.Remove(timer.Id);
                    m_timerPool.Release(timer);
                }
                m_toRemove.Clear();
            }

            int addLen = m_toAdd.Count;
            if (addLen > 0)
            {
                for (int i = 0; i < addLen; i++)
                {
                    m_timers.Add(m_toAdd[i].Id, m_toAdd[i]);
                }
                m_toAdd.Clear();
            }
        }

        #region Public Methods
        public TimerData DelayCall(float delay, TimerHandler handler, object parm = null)
        {
            return RepeatedCall(1, delay, 0, false, handler, parm);
        }

        public TimerData IntervalCall(int times, float interval, TimerHandler handler, object parm = null)
        {
            return RepeatedCall(times, 0, interval, false, handler, parm);
        }

        public TimerData IntervalDelayCall(int times, float delay, float interval, TimerHandler handler, object parm = null)
        {
            return RepeatedCall(times, delay, interval, false, handler, parm);
        }
        public TimerData RepeatedCall(int times, float delay, float interval, bool ignoreTimeScale, TimerHandler handler, object parm = null)
        {
            if (handler == null)
            {
                Debug.Log("Timer Callback can not be null! ");
                return null;
            }

            var timer = m_timerPool.Get();
            timer.OnStart(timerId++, times, delay, interval, ignoreTimeScale, handler, parm);
            m_toAdd.Add(timer);
            return timer;
        }

        public void SetPause(uint timerId, bool pause)
        {
            TimerData timer = GetTimerData(timerId);
            timer.SetPause(pause);
        }

        public void CancelCallback(uint timerId)
        {
            TimerData timer = GetTimerData(timerId);
            timer.SetPause(true);
            timer.OnDestroy();
        }

        public TimerData GetTimerData(uint timerId)
        {
            TimerData timer;
            if (m_timers.TryGetValue(timerId, out timer))
                return timer;
            return null;
        }
        #endregion

        public override void OnRemove()
        {
            MonoManager mono = GameObject.FindObjectOfType<MonoManager>();
            if (mono != null)
                mono.onUpdate = null;
            m_timers.Clear();
            m_toRemove.Clear();
            m_toAdd.Clear();
            m_timerPool.Clear();
            timerId = 0;
            m_active = false;
        }
    }
}