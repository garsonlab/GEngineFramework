/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: VersionMediator
 * Date    : 2018/03/16
 * Version : v1.0
 * Describe: 
 */

using GEngine.Patterns;

namespace GEngine.Modules
{
    public class VersionMediator : Mediator
    {
        public new const string NAME = "VersionMediator";

        public VersionMediator() : base(NAME) { }



        internal void ShowNewVersion(Callback_0 downloadNewVersion)
        {
            //展示对话框，下载, 提示network中的wifi状态
            downloadNewVersion();
        }

        internal void ShowNewResources(Callback_0 DownloadNewResources)
        {
            //下载新资源
        }

        internal void ShowNetBreak(Callback_1<MessageArgs> CheckVeison)
        {
            //显示网络不可用
            CheckVeison(null);
        }
    }
}