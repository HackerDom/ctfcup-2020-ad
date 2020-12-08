using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using QueenOfHearts.CoreLibrary.Models;
using QueenOfHearts.CoreLibrary.Serialization;
using QueenOfHearts.VictimsProvider.Configuration;
using Vostok.Commons.Time;
using Vostok.Logging.Abstractions;

namespace QueenOfHearts.VictimsProvider
{
    public class VictimIndex : IVictimIndex
    {
        private readonly AsyncPeriodicalAction _dump;
        private ConcurrentDictionary<string, Victim> _victims;

        public VictimIndex(ISettingsProvider settingsProvider, ILog log)
        {
            Initialize(settingsProvider);

            _dump = new AsyncPeriodicalAction(
                async () =>
                {
                    await File.WriteAllTextAsync(settingsProvider.ApplicationSettings.QueuePath, _victims.ToJson());
                }, exception => log.Error(exception, "Can't dump index"),
                () => settingsProvider.ApplicationSettings.DumpPeriod);
            _dump.Start();
        }

        public IEnumerable<string> Names => _victims.Keys;

        public bool TryGetVictim(string name, out Victim victim)
        {
            return _victims.TryGetValue(name, out victim);
        }

        public void Set(Victim victim)
        {
            _victims[victim.VictimName] = victim;
        }

        private void Initialize(ISettingsProvider settingsProvider)
        {
            var dumpPath = settingsProvider.ApplicationSettings.QueuePath;
            _victims = File.Exists(dumpPath)
                ? new ConcurrentDictionary<string, Victim>(File.ReadAllText(dumpPath)
                    .FromJson<Dictionary<string, Victim>>())
                : new ConcurrentDictionary<string, Victim>();
        }
    }
}