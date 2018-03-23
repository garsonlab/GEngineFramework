/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: Controller
 * Date    : 2018/01/04
 * Version : v1.0
 * Describe: 
 */

using System.Collections.Generic;
using GEngine.Interface;
using GEngine.Patterns;

namespace GEngine.Core
{
    /// <summary>
    /// 控制器，管理所有的Command
    /// </summary>
    public class Controller : IController
    {
        protected IDictionary<string, Command> m_commandMap;
        protected readonly object m_syncRoot = new object();

        public Controller()
        {
            m_commandMap = new Dictionary<string, Command>();
            InitializeController();
        }

        protected virtual void InitializeController()
        {
        }

        public void RegisterCommand(Command command)
        {
            lock (m_syncRoot)
            {
                if (m_commandMap.ContainsKey(command.CommandName))
                    m_commandMap[command.CommandName] = command;
                else
                    m_commandMap.Add(command.CommandName, command);
            }
            command.OnRegister();
        }

        public T RetrieveCommand<T>(string commandName) where T : Command
        {
            Command command = null;
            lock (m_syncRoot)
            {
                if (m_commandMap.ContainsKey(commandName))
                    command = m_commandMap[commandName];
            }
            return (T)command;
        }

        public T RemoveCommand<T>(string commandName) where T : Command
        {
            Command command = null;
            lock (m_syncRoot)
            {
                if (m_commandMap.TryGetValue(commandName, out command))
                {
                    command.RemoveAllMessages();
                    m_commandMap.Remove(commandName);
                }
            }
            if (command != null)
                command.OnRemove();
            return (T)command;
        }

        public bool HasCommand(string commandName)
        {
            lock (m_syncRoot)
            {
                return m_commandMap.ContainsKey(commandName);
            }
        }
    }
}