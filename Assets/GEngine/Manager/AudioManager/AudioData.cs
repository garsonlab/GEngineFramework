/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: AudioData
 * Date    : 2018/03/12
 * Version : v1.0
 * Describe: 
 */

namespace GEngine.Managers
{
    public class AudioData
    {
        public enum AudioType
        {
            Backageground,//背景音
            Effect2D,//2D声音
            Effect3D,//3D声音
        }


        //int m_audioId;
        //string m_assetPath;
        //AudioType m_audioType;
        //uint m_loops;
        //float m_delay;
        //float m_isComplete;
        //bool m_fadeIn;
        //bool m_fadeOut;

        public Callback_0 OnPlayBegin;
        public Callback_0 OnPlayEnd;


        //public int AudioId { get { return m_audioId; } }

    }
}