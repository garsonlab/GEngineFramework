/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: IView
 * Date    : 2018/01/04
 * Version : v1.0
 * Describe: 
 */

using GEngine.Patterns;

namespace GEngine.Interface
{
    public interface IView
    {
        void RegisterMediator(Mediator mediator);
        T RetrieveMediator<T>(string mediatorName) where T : Mediator;
        T RemoveMediator<T>(string mediatorName) where T : Mediator;
        bool HasMediator(string mediatorName);
    }
}