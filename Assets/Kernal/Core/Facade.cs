/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: Facade
 * Date    : 2018/01/05
 * Version : v1.0
 * Describe: 
 */

using Kernal.Interface;
using Kernal.Managers;
using Kernal.Patterns;

namespace Kernal.Core
{
    public class Facade: KernalBase, IModel, IView, IController, IGManager
    {
        protected static Facade m_instance;
        private IModel m_model;
        private IView m_view;
        private IController m_controller;
        private IGManager m_manager;

        //单例只在函数构造时赋值
        public static Facade Instance{get { return m_instance; }}

        public Facade()
        {
            if (m_instance == null)
            {
                m_instance = this;
                InitializeFacade();
                InitializeManager();
                OnInitializeEnd();
            }
        }

        protected virtual void InitializeFacade()
        {
            m_model = new Model();
            m_view = new View();
            m_controller = new Controller();
            m_manager = new GManager();
        }

        protected virtual void InitializeManager()
        {
            RegisterManager(Managers.MessageManager.NAME, new MessageManager());
            RegisterManager(Managers.TimerManager.NAME, new TimerManager());
        }

        protected virtual void OnInitializeEnd(){}

        #region Model
        public void RegisterProxy(Proxy proxy)
        {
            m_model.RegisterProxy(proxy);
        }

        public new T RetrieveProxy<T>(string proxyName) where T : Proxy
        {
            return m_model.RetrieveProxy<T>(proxyName);
        }

        public T RemoveProxy<T>(string proxyName) where T : Proxy
        {
            return m_model.RemoveProxy<T>(proxyName);
        }

        public bool HasProxy(string proxyName)
        {
            return m_model.HasProxy(proxyName);
        }
        #endregion

        #region View
        public void RegisterMediator(Mediator mediator)
        {
            m_view.RegisterMediator(mediator);
        }

        public new T RetrieveMediator<T>(string mediatorName) where T : Mediator
        {
            return m_view.RetrieveMediator<T>(mediatorName);
        }

        public T RemoveMediator<T>(string mediatorName) where T : Mediator
        {
            return m_view.RemoveMediator<T>(mediatorName);
        }

        public bool HasMediator(string mediatorName)
        {
            return m_view.HasMediator(mediatorName);
        }
        #endregion

        #region Controller
        public void RegisterCommand(Command command)
        {
            m_controller.RegisterCommand(command);
        }

        public new T RetrieveCommand<T>(string commandName) where T : Command
        {
            return m_controller.RetrieveCommand<T>(commandName);
        }

        public T RemoveCommand<T>(string commandName) where T : Command
        {
            return m_controller.RemoveCommand<T>(commandName);
        }

        public bool HasCommand(string commandName)
        {
            return m_controller.HasCommand(commandName);
        }
        #endregion

        #region Manager
        public void RegisterManager<T>(string managerName, T manager) where T : Manager
        {
            m_manager.RegisterManager<T>(managerName, manager);
        }

        public new T RetrieveManager<T>(string managerName) where T : Manager
        {
            return m_manager.RetrieveManager<T>(managerName);
        }

        public T RemoveManager<T>(string managerName) where T : Manager
        {
            return m_manager.RemoveManager<T>(managerName);
        }
        #endregion

    }
}
