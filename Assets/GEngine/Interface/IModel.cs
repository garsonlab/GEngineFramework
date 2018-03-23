/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: IModel
 * Date    : 2018/01/04
 * Version : v1.0
 * Describe: 
 */

using GEngine.Patterns;

namespace GEngine.Interface
{
    public interface IModel
    {
        /// <summary>
        /// 注册Proxy
        /// </summary>
        /// <param name="proxy"></param>
        void RegisterProxy(Proxy proxy);
        /// <summary>
        /// 获取Proxy
        /// </summary>
        /// <param name="proxyName">名字</param>
        /// <returns></returns>
        T RetrieveProxy<T>(string proxyName) where T : Proxy;
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="proxyName"></param>
        /// <returns></returns>
        T RemoveProxy<T>(string proxyName) where T : Proxy;
        /// <summary>
        /// 是否已存在
        /// </summary>
        /// <param name="proxyName"></param>
        /// <returns></returns>
        bool HasProxy(string proxyName);
    }
}
