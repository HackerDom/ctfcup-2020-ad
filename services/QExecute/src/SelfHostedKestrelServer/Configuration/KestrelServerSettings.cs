using System.Security.Cryptography.X509Certificates;

namespace HttpServer.Configuration
{
    public class KestrelServerSettings
    {
        public int IOQueueCount = 1;
        public X509Certificate2 SslCertificate;
    }
}