using System;
using System.Collections.Concurrent;
using System.Threading;
using QueenOfHearts.ExecutionService.OpLog.Operations;
using Vostok.Commons.Time;
using Vostok.Logging.Abstractions;

namespace QueenOfHearts.ExecutionService.OpLog
{
    internal class OperationApplier : IOperationApplier
    {
        private readonly ConcurrentQueue<IOperation> _operationsQueue;
        private readonly SemaphoreSlim _signal = new(0, 1);
        private IStateManager _stateManager;

        public OperationApplier(IOpLogManager opLogManager, IStateManager stateManager, ILog log)
        {
            _stateManager = stateManager;
            _operationsQueue = new ConcurrentQueue<IOperation>();
            StartProcessQueue(opLogManager, stateManager, log);
        }

        public void EnqueueOperation(IOperation operation)
        {
            _operationsQueue.Enqueue(operation);
            _signal.Release();
        }

        private void StartProcessQueue(IOpLogManager opLogManager, IStateManager stateManager, ILog log)
        {
            new AsyncPeriodicalAction(async () =>
            {
                while (_operationsQueue.TryDequeue(out var operation))
                    try
                    {
                        await opLogManager.Write(operation);
                        operation.Apply(stateManager.ObtainState);
                        operation.Complete(OperationResult.Success);
                    }
                    catch (Exception e)
                    {
                        operation.Complete(OperationResult.Failed);
                        log.Error(e, "Can't apply operation");
                    }

                await _signal.WaitAsync();
            }, exception => log.Error(exception, "Can't apply operation"), () => 0.Seconds()).Start();
        }
    }
}