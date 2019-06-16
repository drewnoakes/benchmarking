using System.Threading;
using BenchmarkDotNet.Attributes;

namespace Benchmarking
{
    [ClrJob]
    public class ExchangeVsCompareExchangeBenchmarks
    {
        private const int OperationCount = 100_000_000;

        private int _i;

        [Benchmark(OperationsPerInvoke = OperationCount * 2)]
        public void Exchange()
        {
            _i = 0;

            for (var i = 0; i < OperationCount; i++)
            {
                Interlocked.Exchange(ref _i, 0);
                Interlocked.Exchange(ref _i, 1);
            }
        }

        [Benchmark(OperationsPerInvoke = OperationCount * 2)]
        public void CompareExchange_Match()
        {
            _i = 0;

            for (var i = 0; i < OperationCount; i++)
            {
                Interlocked.CompareExchange(ref _i, 0, 1);
                Interlocked.CompareExchange(ref _i, 1, 0);
            }
        }

        [Benchmark(OperationsPerInvoke = OperationCount * 2)]
        public void CompareExchange_NoMatch()
        {
            _i = 0;

            for (var i = 0; i < OperationCount; i++)
            {
                Interlocked.CompareExchange(ref _i, 1, 1);
                Interlocked.CompareExchange(ref _i, 1, 1);
            }
        }
    }
}