using System;
using Vostok.Commons.Time;

namespace QueenOfHearts.VictimsProvider.Configuration
{
    public class ApplicationSettings
    {
        public TimeSpan DumpPeriod { get; } = 5.Seconds();
        public string QueuePath { get; } = "./data/queue.dump";
        public int Port { get; } = 8000;
    }
}