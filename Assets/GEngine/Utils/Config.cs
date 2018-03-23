/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: Config
 * Date    : 2018/01/10
 * Version : v1.0
 * Describe: 
 */

namespace GEngine
{
    public class Config
    {

        /// <summary>
        /// 设计尺寸宽度
        /// </summary>
        public const float DesignWidth = 720f;
        /// <summary>
        /// 设计尺寸长度
        /// </summary>
        public const float DesignHeight = 1280f;
        /// <summary>
        /// 使用Bundle模式
        /// </summary>
        public static bool BundleMode = false;
        /// <summary>
        /// 网络加密
        /// </summary>
        public const bool NetEncrypt = false;
        /// <summary>
        /// 2D声音播放器最大同时存在个数
        /// </summary>
        public const int Audio2DMaxPile = 5;
        /// <summary>
        /// Bundle关系表名
        /// </summary>
        public const string BundleManifest = "BundleManifest.manifest";
        /// <summary>
        /// 语言包
        /// </summary>
        public static LaunghType LaunghType = LaunghType.Zh_CN;
    }
}