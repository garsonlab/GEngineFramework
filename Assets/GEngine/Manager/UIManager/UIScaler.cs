/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: UIScaler
 * Date    : 2018/03/14
 * Version : v1.0
 * Describe: 
 */

using UnityEngine;

namespace GEngine.Managers
{
    /// <summary>
    /// 非全屏缩放模式
    /// </summary>
    public enum ScaleMode
    {
        /// <summary>
        /// 匹配宽度
        /// </summary>
        MatchWidth = 1,
        /// <summary>
        /// 匹配高度
        /// </summary>
        MatchHeight
    }

    /// <summary>
    /// 适配方案
    /// </summary>
    public enum AdaptPolicy
    {
        TopLeft = 0,
        TopCenter = 1,
        TopRight = 2,
        MiddleLeft = 3,
        MiddleCenter = 4,
        MiddleRight = 5,
        BottomLeft = 6,
        BottomCenter = 7,
        BottomRight = 8,
    }


    /// <summary>
    /// UI适配解决方案
    /// </summary>
    public class UIScaler
    {
        public static ScaleMode scaleMode = ScaleMode.MatchWidth;
        private static float m_scaleX;
        private static float m_scaleY;
        private static float m_scaleM;

        /// <summary>
        /// 重置适配参数
        /// </summary>
        public static void ResetScaler()
        {
            m_scaleX = Screen.width/Config.DesignWidth;
            m_scaleY = Screen.height/Config.DesignHeight;
            m_scaleM = m_scaleX >= m_scaleY ? m_scaleX : m_scaleY;
        }

        /// <summary>
        /// 相对屏幕适配缩放（会修改锚点为中心）
        /// </summary>
        /// <param name="policy">适配策略</param>
        /// <param name="transform">适配目标</param>
        /// <param name="isStretch">是否拉伸, 指按最大缩放比例</param>
        public static void Rescale(AdaptPolicy policy, RectTransform transform, bool isStretch = false)
        {
            float scale = isStretch ? m_scaleM : (scaleMode == ScaleMode.MatchWidth ? m_scaleX : m_scaleY);
            transform.localScale = Vector3.one*scale;

            transform.anchorMin = Vector2.one*0.5f;
            transform.anchorMax = Vector2.one*0.5f;
            switch (policy)
            {
                case AdaptPolicy.TopLeft:
                    transform.localPosition = new Vector3(-Screen.width*0.5f + transform.sizeDelta.x*scale*0.5f,
                        Screen.height*0.5f - transform.sizeDelta.y*scale*0.5f, 0);
                    break;
                case AdaptPolicy.TopCenter:
                    transform.localPosition = new Vector3(0, Screen.height*0.5f - transform.sizeDelta.y*scale*0.5f, 0);
                    break;
                case AdaptPolicy.TopRight:
                    transform.localPosition = new Vector3(Screen.width*0.5f - transform.sizeDelta.x*scale*0.5f,
                        Screen.height*0.5f - transform.sizeDelta.y*scale*0.5f, 0);
                    break;
                case AdaptPolicy.MiddleLeft:
                    transform.localPosition = new Vector3(-Screen.width*0.5f + transform.sizeDelta.x*scale*0.5f, 0, 0);
                    break;
                case AdaptPolicy.MiddleCenter:
                    transform.localPosition = Vector3.zero;
                    break;
                case AdaptPolicy.MiddleRight:
                    transform.localPosition = new Vector3(Screen.width*0.5f - transform.sizeDelta.x*scale*0.5f, 0, 0);
                    break;
                case AdaptPolicy.BottomLeft:
                    transform.localPosition = new Vector3(-Screen.width*0.5f + transform.sizeDelta.x*scale*0.5f,
                        -Screen.height*0.5f + transform.sizeDelta.y*scale*0.5f, 0);
                    break;
                case AdaptPolicy.BottomCenter:
                    transform.localPosition = new Vector3(0, -Screen.height*0.5f + transform.sizeDelta.y*scale*0.5f, 0);
                    break;
                case AdaptPolicy.BottomRight:
                    transform.localPosition = new Vector3(Screen.width*0.5f - transform.sizeDelta.x*scale*0.5f,
                        -Screen.height*0.5f + transform.sizeDelta.y*scale*0.5f, 0);
                    break;
            }
        }


        /// <summary>
        /// 相对关联组件适配位置（会修改锚点为中心）
        /// </summary>
        /// <param name="target">适配目标</param>
        /// <param name="relative">关联目标</param>
        /// <param name="policy">适配策略</param>
        public static void SetRelativePos(RectTransform target, RectTransform relative, AdaptPolicy policy)
        {
            Vector2 pos = target.anchoredPosition;

            Vector2 p_middle = relative.sizeDelta * 0.5f;
            p_middle.x = p_middle.x * relative.localScale.x; p_middle.y = p_middle.y * relative.localScale.y;//考虑到缩放
            Vector2 s_middle = target.sizeDelta * 0.5f;
            s_middle.x = s_middle.x * target.localScale.x; s_middle.y = s_middle.y * target.localScale.y;

            target.anchorMax = Vector2.one * 0.5f;//重置锚点位置为居中
            target.anchorMin = Vector2.one * 0.5f;
            target.anchoredPosition = Vector2.zero;//重置UI位置为正中

            switch (policy)
            {
                case AdaptPolicy.TopLeft:
                    pos.x = -(p_middle.x - s_middle.x);
                    pos.y = (p_middle.y - s_middle.y);
                    break;
                case AdaptPolicy.TopCenter:
                    pos.x = 0;
                    pos.y = (p_middle.y - s_middle.y);
                    break;
                case AdaptPolicy.TopRight:
                    pos.x = (p_middle.x - s_middle.x);
                    pos.y = (p_middle.y - s_middle.y);
                    break;
                case AdaptPolicy.MiddleLeft:
                    pos.x = -(p_middle.x - s_middle.x);
                    pos.y = 0;
                    break;
                case AdaptPolicy.MiddleCenter:
                    pos.x = 0;
                    pos.y = 0;
                    break;
                case AdaptPolicy.MiddleRight:
                    pos.x = (p_middle.x - s_middle.x);
                    pos.y = 0;
                    break;
                case AdaptPolicy.BottomLeft:
                    pos.x = -(p_middle.x - s_middle.x);
                    pos.y = -(p_middle.y - s_middle.y);
                    break;
                case AdaptPolicy.BottomCenter:
                    pos.x = 0;
                    pos.y = -(p_middle.y - s_middle.y);
                    break;
                case AdaptPolicy.BottomRight:
                    pos.x = (p_middle.x - s_middle.x);
                    pos.y = -(p_middle.y - s_middle.y);
                    break;
            }
            target.anchoredPosition = pos;
        }

    }
}
