/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: IGManager
 * Date    : 2018/01/05
 * Version : v1.0
 * Describe: 
 */

using GEngine.Patterns;

namespace GEngine.Interface
{
    public interface IGManager
    {
        void RegisterManager<T>(T manager) where T : Manager;
        T RetrieveManager<T>() where T : Manager;
        T RemoveManager<T>() where T : Manager;
    }
}