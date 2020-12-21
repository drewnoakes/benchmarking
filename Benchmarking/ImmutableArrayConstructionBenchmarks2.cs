using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmarking
{
    [SimpleJob(RuntimeMoniker.Net48)]
    [MemoryDiagnoser]
    public class ImmutableArrayConstructionBenchmarks2
    {
        /*
                    Method | Count |        Mean |  Gen 0 | Allocated |
-------------------------- |------ |------------:|-------:|----------:|
ImmutableArray_CreateRange |     3 |   601.20 ns | 0.0334 |     176 B |
ImmutableArray_Builder_Add |     3 |    92.42 ns | 0.0122 |      64 B |
ImmutableArray_CreateRange |    30 | 1,824.16 ns | 0.1030 |     544 B |
ImmutableArray_Builder_Add |    30 |   378.90 ns | 0.0529 |     280 B |
         */
        private IEnumerable<string> enumerable;
        private IImmutableList<string> immutableArray;

        [Params(3, 30)]
        public int Count;

        [GlobalSetup]
        public void Setup()
        {
            enumerable = Enumerable.Range(0, Count).Select(_ => "");
            immutableArray = enumerable.ToImmutableArray();
        }

        [Benchmark]
        public ImmutableArray<string> ImmutableArray_CreateRange()
        {
            return ImmutableArray.CreateRange(immutableArray.Select(i => i));
        }

        [Benchmark]
        public ImmutableArray<string> ImmutableArray_Builder_ToImmutable()
        {
            var ids = ImmutableArray.CreateBuilder<string>(Count);
            for (int i = 0; i < Count; i++)
                ids.Add(immutableArray[i]);
            return ids.ToImmutable();
        }

        [Benchmark]
        public ImmutableArray<string> ImmutableArray_Builder_MoveToImmutable()
        {
            var ids = ImmutableArray.CreateBuilder<string>(Count);
            for (int i = 0; i < Count; i++)
                ids.Add(immutableArray[i]);
            return ids.MoveToImmutable();
        }
    }
}