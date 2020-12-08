using System;
using System.IO;
using System.Threading.Tasks;
using QueenOfHearts.CoreLibrary.Models;
using QueenOfHearts.CoreLibrary.Serialization;
using QueenOfHearts.CoreLibrary.Storages;
using QueenOfHearts.ExecutionService.Configuration;
using QueenOfHearts.ExecutionService.OpLog;
using Vostok.Commons.Time;
using Vostok.Logging.Abstractions;

namespace QueenOfHearts.ExecutionService
{
    internal class StateManager : IStateManager
    {
        private readonly ILog _log;
        private readonly IOpLogManager _opLogManager;

        private readonly ISettingsProvider _settingsProvider;
        private IState _currentState;
        private AsyncPeriodicalAction _periodicalSnapshot;

        public StateManager(IOpLogManager opLogManager, ISettingsProvider settingsProvider, ILog log)
        {
            _opLogManager = opLogManager;
            _settingsProvider = settingsProvider;
            _log = log;
        }

        public IState ObtainState()
        {
            return _currentState;
        }

        public void Initialize()
        {
            _currentState = ReadFromSnapshotAndOpLog();

            _periodicalSnapshot = new AsyncPeriodicalAction(async () => await MakeStateSnapshot(),
                exception => _log.Error(exception, "Can't snapshot"), () => 5.Seconds());
            _periodicalSnapshot.Start();
        }

        private async Task MakeStateSnapshot()
        {
            var state = ReadFromSnapshotAndOpLog();

            var tmpSnapshot = $"{_settingsProvider.ApplicationSettings.SnapshotFileName}_tmp";
            await using (var outputStream = File.Create(tmpSnapshot))
            {
                var serializer = new BinarySerializer(outputStream);
                state.CommandIndex.Serialize(serializer, s => serializer.Write(s),
                    command => command.Serialize(serializer));
                state.ExecutorIndex.Serialize(serializer, s => serializer.Write(s),
                    executor => executor.Serialize(serializer));
            }


            File.Move(tmpSnapshot, _settingsProvider.ApplicationSettings.SnapshotFileName, true);

            await _opLogManager.Lock();

            ApplyOpLog(state, _opLogManager.LastReadeOffset);
            _currentState = state;
            _opLogManager.Delete();

            _opLogManager.Release();

            _log.Info("Snapshot created, oplog shrinked");
        }

        private State ReadFromSnapshotAndOpLog()
        {
            var state = File.Exists(_settingsProvider.ApplicationSettings.SnapshotFileName)
                ? BuildFromSnapshot()
                : new State(new SerializableIndex<string, Executor>(),
                    new SerializableIndex<string, Command>());

            ApplyOpLog(state, 0);
            return state;
        }

        private void ApplyOpLog(IState state, long lastReadeOffset)
        {
            foreach (var operation in _opLogManager.Read(lastReadeOffset))
                try
                {
                    operation.Apply(() => state);
                }
                catch (Exception e)
                {
                    _log.Error(e, "Can't apply operation");
                }
        }

        private State BuildFromSnapshot()
        {
            using var fileStream = File.OpenRead(_settingsProvider.ApplicationSettings.SnapshotFileName);
            var binaryDeserializer = new BinaryDeserializer(fileStream);
            var commandIndex = GenericIndex.Deserialize(binaryDeserializer, deserializer => deserializer.ReadString(),
                Command.Deserialize);
            var executorIndex = GenericIndex.Deserialize(binaryDeserializer, deserializer => deserializer.ReadString(),
                Executor.Deserialize);
            var state = new State(executorIndex, commandIndex);
            return state;
        }
    }
}