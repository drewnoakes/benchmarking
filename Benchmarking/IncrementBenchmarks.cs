using System.Threading;
using BenchmarkDotNet.Attributes;

namespace Benchmarking
{
    [ClrJob]
    [RankColumn]
    [MemoryDiagnoser]
    public class IncrementBenchmarks
    {
        /*
                Method |       Mean |     Error |    StdDev | Rank | Allocated |
---------------------- |-----------:|----------:|----------:|-----:|----------:|
           Unprotected |  0.1409 ns | 0.1611 ns | 0.1918 ns |    1 |       0 B |
 Interlocked_Increment |  7.4125 ns | 0.7813 ns | 0.9301 ns |    2 |       0 B |
                  Lock | 31.8284 ns | 0.5410 ns | 0.4796 ns |    3 |       0 B |
        */
        private readonly object _lock = new object();
        private int i;

        [Benchmark]
        public int Unprotected()
        {
            return ++i;
        }

        [Benchmark]
        public int Interlocked_Increment()
        {
            return Interlocked.Increment(ref i);
        }

        [Benchmark]
        public int Lock()
        {
            lock (_lock)
            {
                return ++i;
            }
        }
    }
}