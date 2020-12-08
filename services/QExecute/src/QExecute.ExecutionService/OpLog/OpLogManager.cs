using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using QueenOfHearts.CoreLibrary.Serialization;
using QueenOfHearts.ExecutionService.Configuration;
using QueenOfHearts.ExecutionService.OpLog.Operations;
using Vostok.Logging.Abstractions;

namespace QueenOfHearts.ExecutionService.OpLog
{
    internal class OpLogManager : IOpLogManager
    {
        public const int Marker = 0xD7FF;
        public const int ErrorMarker = -0xC0FFEE;
        private readonly ILog _log;

        private readonly Dictionary<int, Func<IBinaryDeserializer, IOperation>> _opCodeToFactory = new()
        {
            {0, AddCommandOperation.Deserialize},
            {1, AddCommandAdminOperation.Deserialize},
            {2, AddExecutorOperation.Deserialize},
        };

        private readonly SemaphoreSlim _readerLock = new(1, 1);

        private readonly ISettingsProvider _settingsProvider;

        private readonly Dictionary<Type, int> _typesToOpCodes = new()
        {
            {typeof(AddCommandOperation), 0},
            {typeof(AddCommandAdminOperation), 1},
            {typeof(AddExecutorOperation), 2},
        };

        private readonly SemaphoreSlim _writerLock = new(1, 1);

        private IBinarySerializer _binarySerializer;
        private long _lastProcessedOperationOffset;


        public OpLogManager(ISettingsProvider settingsProvider, ILog log)
        {
            _settingsProvider = settingsProvider;
            _log = log;
            _binarySerializer = GetBinarySerializer();
        }

        public long LastReadeOffset { get; private set; }

        public IEnumerable<IOperation> Read(long fromOffset = 0)
        {
            _readerLock.Wait();
            using var binaryDeserializer = GetDeserializer(fromOffset);
            foreach (var operation in ReadInternal(binaryDeserializer)) yield return operation;
            LastReadeOffset = binaryDeserializer.Position;
            _readerLock.Release();
        }

        public void Release()
        {
            _writerLock.Release();
        }

        public void Delete()
        {
            _readerLock.WaitAsync();
            _binarySerializer.Dispose();
            File.Delete(_settingsProvider.ApplicationSettings.OpLogFileName);
            _binarySerializer = GetBinarySerializer();
            _readerLock.Release();
        }

        public async Task Lock()
        {
            await _writerLock.WaitAsync();
        }


        public async Task Write<TOperation>(TOperation operation) where TOperation : IOperation
        {
            await _writerLock.WaitAsync();
            try
            {
                _binarySerializer.Write(Marker);
                var opCode = _typesToOpCodes[operation.GetType()];
                _binarySerializer.Write(opCode);
                operation.Serialize(_binarySerializer);
            }
            catch (Exception e)
            {
                _log.Error(e, "Can't serialize operation");
                _binarySerializer.Write(ErrorMarker);
                throw;
            }
            finally
            {
                await _binarySerializer.FlushAsync();
                _writerLock.Release();
            }
        }

        private IEnumerable<IOperation> ReadInternal(BinaryDeserializer binaryDeserializer)
        {
            while (binaryDeserializer.CanReadSize(sizeof(int)))
            {
                IOperation readInternal;
                try
                {
                    var marker = binaryDeserializer.ReadInt32();
                    if (marker != Marker)
                    {
                        _log.Error($"OpLog corrupted after {_lastProcessedOperationOffset}");
                        SeekToNextMarker(binaryDeserializer);
                        continue;
                    }
                    
                    var opCode = binaryDeserializer.ReadInt32();
                    var factory = _opCodeToFactory[opCode];
                    readInternal = factory(binaryDeserializer);
                    _lastProcessedOperationOffset = binaryDeserializer.Position;
                }
                catch (Exception e)
                {
                    _log.Error(e, $"OpLog corrupted after {_lastProcessedOperationOffset}");
                    SeekToNextMarker(binaryDeserializer);
                    continue;
                }

                yield return readInternal;
            }
        }

        private BinarySerializer GetBinarySerializer()
        {
            var outStream = File.Open(_settingsProvider.ApplicationSettings.OpLogFileName, FileMode.Append,
                FileAccess.Write,
                FileShare.ReadWrite);
            var binarySerializer = new BinarySerializer(outStream);
            return binarySerializer;
        }

        private BinaryDeserializer GetDeserializer(long fromOffset)
        {
            var inStream = File.Open(_settingsProvider.ApplicationSettings.OpLogFileName, FileMode.Open,
                FileAccess.ReadWrite,
                FileShare.ReadWrite);
            ;
            var binaryDeserializer = new BinaryDeserializer(inStream) {Position = fromOffset};
            return binaryDeserializer;
        }

        private void SeekToNextMarker(BinaryDeserializer binaryDeserializer)
        {
            var markerSize = sizeof(int);
            binaryDeserializer.Position = _lastProcessedOperationOffset + markerSize;
            while (binaryDeserializer.CanReadSize(markerSize))
            {
                if (Marker == binaryDeserializer.ReadInt32())
                {
                    binaryDeserializer.Position -= markerSize;
                    _lastProcessedOperationOffset = binaryDeserializer.Position;
                    return;
                }
                binaryDeserializer.Position -= markerSize - 1;
            }
        }
    }
}