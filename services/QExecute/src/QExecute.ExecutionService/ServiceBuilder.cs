using HttpServer.Abstractions;
using QueenOfHearts.CoreLibrary.Http;
using QueenOfHearts.ExecutionService.Configuration;
using QueenOfHearts.ExecutionService.Handlers;
using QueenOfHearts.ExecutionService.OpLog;
using SimpleInjector;
using Vostok.Logging.Abstractions;

namespace QueenOfHearts.ExecutionService
{
    internal class ServiceBuilder
    {
        private Container _container;

        public void Build(ILog log)
        {
            _container = new Container();
            _container.RegisterInstance(log);
            _container.RegisterSingleton<ISettingsProvider, SettingsProvider>();
            _container.RegisterSingleton<IOpLogManager, OpLogManager>();
            _container.RegisterSingleton<IOperationApplier, OperationApplier>();
            _container.Register<IApplication, Application>();
            _container.RegisterSingleton<IStateManager, StateManager>();
            _container.RegisterSingleton<IVictimsStorageClient, VictimsStorageClient>();
            RegisterHandlers();
            _container.Verify();
        }

        private void RegisterHandlers()
        {
            _container.Register<IHttpHandler, CompositeApiHandler>();

            _container.Collection.Register<IApiHandler>(
                typeof(AddCommandAdminHandler),
                typeof(AddCommandHandler),
                typeof(AddExecutorHandler),
                typeof(GetCommandHandler),
                typeof(GetCommandsListHandler),
                typeof(GetExecutorHandler),
                typeof(GetVictimHandler),
                typeof(GetVictimsHandler),
                typeof(AddVictimHandler)
            );
        }

        public T Get<T>() where T : class
        {
            return _container.GetInstance<T>();
        }
    }
}