using System.Collections.Immutable;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmarking
{
    [SimpleJob(RuntimeMoniker.Net48)]
    [RankColumn]
    [MemoryDiagnoser]
    public class ImmutableListArrayCreateRangeBenchmarks
    {
        /*
           BenchmarkDotNet=v0.11.1, OS=Windows 10.0.17134.345 (1803/April2018Update/Redstone4)
           Intel Xeon Silver 4110 CPU 2.10GHz, 2 CPU, 32 logical and 16 physical cores
           Frequency=2045973 Hz, Resolution=488.7650 ns, Timer=TSC
             [Host] : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.3190.0
             Clr    : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.3190.0

           Job=Clr  Runtime=Clr

                   |   N |       Mean |     Error |     StdDev |  Gen 0 |  Gen 1 | Allocated |
            ------ |---- |-----------:|----------:|-----------:|-------:|-------:|----------:|
              List |  10 |   526.8 ns | 10.411 ns |  20.551 ns | 0.0572 |      - |     304 B |
             Array |  10 |   207.7 ns |  3.527 ns |   3.299 ns | 0.0095 |      - |      52 B |
              List | 100 | 3,959.5 ns | 78.617 ns | 167.538 ns | 0.5379 | 0.0038 |    2824 B |
             Array | 100 |   271.3 ns |  5.298 ns |   7.426 ns | 0.0782 |      - |     412 B |
        */
        private string[] data;

        [Params(10, 100)]
        public int N;

        [GlobalSetup]
        public void Setup()
        {
            data = Enumerable.Range(1, N).Select(_ => "").ToArray();
        }

        [Benchmark]
        public ImmutableList<string> List() => ImmutableList.CreateRange(data);

        [Benchmark]
        public ImmutableArray<string> Array() => ImmutableArray.CreateRange(data);
    }
}