/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: IGManager
 * Date    : 2018/01/05
 * Version : v1.0
 * Describe: 
 */

using Kernal.Patterns;

namespace Kernal.Interface
{
    public interface IGManager
    {
        void RegisterManager<T>(string managerName, T manager) where T : Manager;
        T RetrieveManager<T>(string managerName) where T : Manager;
        T RemoveManager<T>(string managerName) where T : Manager;
    }
}