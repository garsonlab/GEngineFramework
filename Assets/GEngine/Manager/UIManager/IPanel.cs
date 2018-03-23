/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: IPanel
 * Date    : 2018/03/10
 * Version : v1.0
 * Describe: 
 */

namespace GEngine.Managers
{
    public enum PanelType
    {
        HUD = 0,
        Normal = 1,
        Fixed = 2,
        PopUp = 3,
        None = 4,
    }

    public enum PanelMode
    {
        DoNothing,
        HideOthers,
        HideSameLayer,
    }

    /// <summary>
    /// UI加载方式
    /// </summary>
    public enum PanelLoad
    {
        Load = 0,//同步
        SyncLoad = 1,//异步
    }

    /// <summary>
    /// 面板状态
    /// </summary>
    public enum PanelState
    {
        None,//刚初始化
        Loading,//正在加载中
        Showing,//正在显示
        Close,//已关闭
        Destroy,//已销毁
    }

    public interface IPanel
    {
        /// <summary>
        /// 当前面板状态
        /// </summary>
        PanelState PanelState { get; }
        /// <summary>
        /// 显示界面
        /// </summary>
        /// <param name="parm">参数</param>
        /// <param name="callback">回调</param>
        void Show(object parm = null, Callback_0 callback = null);
        /// <summary>
        /// 本地化
        /// </summary>
        void Localize();
        //void Init();
        /// <summary>
        /// UI适配
        /// </summary>
        void Resize();
        /// <summary>
        /// 关闭界面
        /// </summary>
        /// <param name="callback"></param>
        void Close();
        /// <summary>
        /// 销毁
        /// </summary>
        void Destroy();
    }
}