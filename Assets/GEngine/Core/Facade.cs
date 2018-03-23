/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: Facade
 * Date    : 2018/01/05
 * Version : v1.0
 * Describe: 
 */

using GEngine.Interface;
using GEngine.Managers;
using GEngine.Patterns;

namespace GEngine.Core
{
    public abstract class Facade: KernalBase, IModel, IView, IController, IGManager
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

        // 实例化所有的模块
        protected virtual void InitializeFacade()
        {
            m_model = new Model();
            m_view = new View();
            m_controller = new Controller();
            m_manager = new GManager();
        }

        // 添加管理器
        protected virtual void InitializeManager()
        {
            RegisterManager(new MessageManager());
            RegisterManager(new TimerManager());
            RegisterManager(new GarbageManager());
            RegisterManager(new ResourceManager());
            RegisterManager(new InputManager());//依赖TimerManager
            RegisterManager(new UIManager());
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
        public void RegisterManager<T>(T manager) where T : Manager
        {
            m_manager.RegisterManager(manager);
        }

        public new T RetrieveManager<T>() where T : Manager
        {
            return m_manager.RetrieveManager<T>();
        }

        public T RemoveManager<T>() where T : Manager
        {
            return m_manager.RemoveManager<T>();
        }
        #endregion

    }
}
