using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Benchmarking
{
    [ClrJob]
    [MemoryDiagnoser]
    public class ImmutableArrayConstructionBenchmarks3
    {
        /*
                               Method | Count |      Mean |     Error |    StdDev |    Median |  Gen 0 | Allocated |
------------------------------------- |------ |----------:|----------:|----------:|----------:|-------:|----------:|
  ImmutableArray_CreateRange_FromList |     3 | 182.44 ns |  3.246 ns |  2.534 ns | 182.33 ns | 0.0143 |      76 B |
 ImmutableArray_Builder_UnknownLength |     3 | 125.56 ns |  2.509 ns |  2.347 ns | 125.74 ns | 0.0160 |      84 B |
   ImmutableArray_Builder_KnownLength |     3 |  65.98 ns |  1.484 ns |  4.088 ns |  64.32 ns | 0.0075 |      40 B |
  ImmutableArray_CreateRange_FromList |    30 | 714.48 ns | 14.095 ns | 29.731 ns | 707.27 ns | 0.0839 |     444 B |
 ImmutableArray_Builder_UnknownLength |    30 | 622.25 ns | 10.158 ns |  7.931 ns | 620.82 ns | 0.0772 |     408 B |
   ImmutableArray_Builder_KnownLength |    30 | 373.65 ns |  3.964 ns |  3.310 ns | 374.40 ns | 0.0281 |     148 B |
         */
        private string[] items;

        [Params(3, 30)]
        public int Count;

        [GlobalSetup]
        public void Setup()
        {
            items = Enumerable.Range(0, Count).Select(_ => "").ToArray();
        }

        [Benchmark]
        public ImmutableArray<string> ImmutableArray_CreateRange_FromList()
        {
            var list = new List<string>();
            foreach (var item in items)
            {
                list.Add(item);
            }
            return ImmutableArray.CreateRange(list);
        }

        [Benchmark]
        public ImmutableArray<string> ImmutableArray_Builder_UnknownLength()
        {
            var builder = ImmutableArray.CreateBuilder<string>();
            foreach (var item in items)
            {
                builder.Add(item);
            }
            return builder.ToImmutable();
        }

        [Benchmark]
        public ImmutableArray<string> ImmutableArray_Builder_KnownLength()
        {
            var builder = ImmutableArray.CreateBuilder<string>(Count);
            foreach (var item in items)
            {
                builder.Add(item);
            }
            return builder.MoveToImmutable();
        }
    }
}