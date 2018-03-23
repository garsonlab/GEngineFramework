/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: ObjectPool
 * Date    : 2018/01/05
 * Version : v1.0
 * Describe: 
 */

using System.Collections.Generic;
using UnityEngine;

namespace GEngine
{
    /// <summary>
    /// 缓存池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectPool<T>
    {
        readonly Stack<T> m_Stack = new Stack<T>();
        readonly T_Callback<T> m_Creater;
        readonly Callback_1<T> m_OnGet;
        readonly Callback_1<T> m_OnRelease;
        readonly Callback_1<T> m_OnClear;

        public int countAll { get; private set; }
        public int countActive { get { return countAll - countInactive; } }
        public int countInactive { get { return m_Stack.Count; } }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="creater">创建调用，如 new T()或 GameObject.Instantiate</param>
        /// <param name="onGet">获取时调用</param>
        /// <param name="onRelease">释放时调用</param>
        /// <param name="onClear">清除时调用</param>
        public ObjectPool(T_Callback<T> creater, Callback_1<T> onGet, Callback_1<T> onRelease, Callback_1<T> onClear)
        {
            this.m_Creater = creater;
            this.m_OnGet = onGet;
            this.m_OnRelease = onRelease;
            this.m_OnClear = onClear;
        }


        public T Get()
        {
            if (m_Creater == null)
                throw new System.NotImplementedException("Internal error. The Creater function is not assigned.");

            T element;
            if (m_Stack.Count == 0)
            {
                element = m_Creater();
                countAll++;
            }
            else
            {
                element = m_Stack.Pop();
            }
            if (m_OnGet != null)
                m_OnGet(element);
            return element;
        }


        public void Release(T element)
        {
            if (m_Stack.Count > 0 && ReferenceEquals(m_Stack.Peek(), element))
            {
                Debug.LogError("Internal error. Trying to destroy object that is already released to pool.");
                return;
            }

            if (m_OnRelease != null)
                m_OnRelease(element);
            m_Stack.Push(element);
        }

        public void Clear()
        {
            var enumerator = m_Stack.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (m_OnRelease != null)
                    m_OnClear(enumerator.Current);
            }
            enumerator.Dispose();
            m_Stack.Clear();
        }

    }

}