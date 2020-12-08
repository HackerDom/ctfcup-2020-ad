using HttpServer.Abstractions;
using QueenOfHearts.CoreLibrary.Http;
using QueenOfHearts.VictimsProvider.Configuration;
using QueenOfHearts.VictimsProvider.Handlers;
using SimpleInjector;
using Vostok.Logging.Abstractions;

namespace QueenOfHearts.VictimsProvider
{
    internal class ServiceBuilder
    {
        private Container _container;

        public void Build(ILog consoleLog)
        {
            _container = new Container();
            _container.RegisterInstance(consoleLog);
            _container.RegisterSingleton<ISettingsProvider, SettingsProvider>();
            _container.Register<IApplication, Application>();
            _container.RegisterSingleton<IVictimIndex, VictimIndex>();
            RegisterHandlers();
            _container.Verify();
        }

        private void RegisterHandlers()
        {
            _container.Register<IHttpHandler, CompositeApiHandler>();

            _container.Collection.Register<IApiHandler>(
                typeof(PutVictimsHandler),
                typeof(GetVictimsListHandler),
                typeof(GetVictimHandler)
            );
        }

        public T Get<T>() where T : class
        {
            return _container.GetInstance<T>();
        }
    }
}