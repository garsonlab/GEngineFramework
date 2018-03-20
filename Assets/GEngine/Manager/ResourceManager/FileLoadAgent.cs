/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: FileLoadAgent
 * Date    : 2018/03/15
 * Version : v1.0
 * Describe: 
 */
using System.Collections;
using UnityEngine;

namespace GEngine.Managers
{
    public class FileLoadAgent
    {
        private bool m_isDone;
        private WWW m_www;
        public float progress
        {
            get
            {
                if (m_www == null)
                    return 0;
                return m_www.progress;
            }
        }

        public bool IsDone
        {
            get { return m_isDone; }
        }

        public IEnumerator LoadSync(string path, Callback_1<byte[]> onLoadSuccess, Callback_1<string> onLoadFaile)
        {
            m_isDone = false;
            m_www = new WWW(path);
            yield return m_www;
            if (m_www.isDone && string.IsNullOrEmpty(m_www.error))
            {
                byte[] bytes = m_www.bytes;
                m_www.Dispose();
                if (onLoadSuccess != null)
                    onLoadSuccess(bytes);
                m_isDone = true;
            }
            else
            {
                m_www.Dispose();
                if (onLoadFaile != null)
                    onLoadFaile("Load Error:: " + m_www.error);
            }
        }

        public byte[] Load(string path)
        {
            m_isDone = false;
            m_www = new WWW(path);
            while (!m_www.isDone){}
            byte[] bytes = m_www.bytes;
            m_www.Dispose();
            return bytes;
        }

    }
}
