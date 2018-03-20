/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: IController
 * Date    : 2018/01/04
 * Version : v1.0
 * Describe: 
 */

using GEngine.Patterns;

namespace GEngine.Interface
{
    public interface IController
    {
        void RegisterCommand(Command command);
        T RetrieveCommand<T>(string commandName) where T : Command;
        T RemoveCommand<T>(string commandName) where T : Command;
        bool HasCommand(string commandName);
    }
}
