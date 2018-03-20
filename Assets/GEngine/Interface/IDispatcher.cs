/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: IDispatcher
 * Date    : 2018/01/04
 * Version : v1.0
 * Describe: 
 */

namespace GEngine.Interface
{
    public interface IDispatcher
    {
        void SendMessage(int messageType);
        void SendMessage(int messageType, object body);
        void SendMessage(int messageType, object body, string type);
    }
}