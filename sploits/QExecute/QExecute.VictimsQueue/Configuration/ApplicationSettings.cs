using System;
using Vostok.Commons.Time;

namespace QueenOfHearts.VictimsProvider.Configuration
{
    public class ApplicationSettings
    {
        public TimeSpan DumpPeriod { get; } = 1.Seconds();
        public string QueuePath { get; } = "queue.dump";
        public string IndexPath { get; } = "index.dump";
        public int Port { get; } = 8000;
    }
}