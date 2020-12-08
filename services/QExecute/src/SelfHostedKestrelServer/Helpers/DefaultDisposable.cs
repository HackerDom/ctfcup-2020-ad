using System;

namespace HttpServer.Helpers
{
    internal class DefaultDisposable : IDisposable
    {
        public static IDisposable Defult = new DefaultDisposable();

        public void Dispose()
        {
        }
    }
}