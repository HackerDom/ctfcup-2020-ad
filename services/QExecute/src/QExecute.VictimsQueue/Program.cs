using Vostok.Logging.Console;
using Vostok.Logging.File;
using Vostok.Logging.File.Configuration;

namespace QueenOfHearts.VictimsProvider
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var builder = new ServiceBuilder();
            builder.Build(new FileLog(new FileLogSettings()
            {
                FilePath = "./data/victim_log" 
            }));
            var app = builder.Get<IApplication>();
            app.Start();
        }
    }
}