using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmarking
{
    [SimpleJob(RuntimeMoniker.Net48)]
    [MemoryDiagnoser]
    public class ImmutableDictionaryConstructionBenchmarks
    {
/*
BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19042
Intel Xeon Silver 4110 CPU 2.10GHz, 2 CPU, 32 logical and 16 physical cores
  [Host]   : .NET Framework 4.8 (4.8.4250.0), X64 RyuJIT
  .NET 4.8 : .NET Framework 4.8 (4.8.4250.0), X64 RyuJIT

Job=.NET 4.8  Runtime=.NET 4.8

10 ITERATIONS

|                                           Method | Count |            Mean |           Error |          StdDev |     Gen 0 |    Gen 1 | Gen 2 |  Allocated |
|------------------------------------------------- |------ |----------------:|----------------:|----------------:|----------:|---------:|------:|-----------:|
|  ImmutableDictionary_CreateRange_DefaultComparer |     3 |        973.5 ns |        11.53 ns |        10.22 ns |    0.0519 |        - |     - |      305 B |
| ImmutableDictionary_CreateRange_SpecificComparer |     3 |      1,143.4 ns |         7.27 ns |         6.45 ns |    0.0641 |        - |     - |      377 B |
|      ImmutableDictionary_Builder_DefaultComparer |     3 |        843.5 ns |        13.68 ns |        12.80 ns |    0.0488 |        - |     - |      289 B |
|     ImmutableDictionary_Builder_SpecificComparer |     3 |      1,113.0 ns |        17.72 ns |        16.58 ns |    0.0626 |        - |     - |      361 B |
|  ImmutableDictionary_RepeatedAdd_DefaultComparer |     3 |      1,196.2 ns |        23.93 ns |        31.12 ns |    0.0763 |        - |     - |      441 B |
| ImmutableDictionary_RepeatedAdd_SpecificComparer |     3 |      1,168.3 ns |        22.97 ns |        21.48 ns |    0.0763 |        - |     - |      441 B |
|  ImmutableDictionary_CreateRange_DefaultComparer |   300 |    181,800.8 ns |     1,616.82 ns |     1,350.12 ns |    3.3203 |   0.1953 |     - |    19370 B |
| ImmutableDictionary_CreateRange_SpecificComparer |   300 |    202,742.0 ns |     3,947.38 ns |     3,876.85 ns |    3.1250 |        - |     - |    19443 B |
|      ImmutableDictionary_Builder_DefaultComparer |   300 |    199,439.6 ns |     1,416.97 ns |     1,183.23 ns |    3.1250 |        - |     - |    19354 B |
|     ImmutableDictionary_Builder_SpecificComparer |   300 |    210,912.4 ns |     4,198.19 ns |     8,763.19 ns |    3.1250 |        - |     - |    19427 B |
|  ImmutableDictionary_RepeatedAdd_DefaultComparer |   300 |    417,828.9 ns |     5,756.99 ns |     5,103.42 ns |   28.9063 |   2.3438 |     - |   170780 B |
| ImmutableDictionary_RepeatedAdd_SpecificComparer |   300 |    416,169.9 ns |     5,730.78 ns |     5,080.18 ns |   28.9063 |   2.3438 |     - |   170780 B |
|  ImmutableDictionary_CreateRange_DefaultComparer | 30000 | 31,830,334.6 ns |   251,138.37 ns |   209,711.91 ns |  300.0000 | 100.0000 |     - |  1925966 B |
| ImmutableDictionary_CreateRange_SpecificComparer | 30000 | 35,041,757.1 ns |   407,258.40 ns |   361,023.97 ns |  300.0000 | 100.0000 |     - |  1925966 B |
|      ImmutableDictionary_Builder_DefaultComparer | 30000 | 32,711,087.5 ns |   579,269.21 ns |   667,087.28 ns |  300.0000 | 100.0000 |     - |  1925966 B |
|     ImmutableDictionary_Builder_SpecificComparer | 30000 | 32,188,841.5 ns |   372,924.56 ns |   311,408.89 ns |  300.0000 | 100.0000 |     - |  1925966 B |
|  ImmutableDictionary_RepeatedAdd_DefaultComparer | 30000 | 82,905,756.7 ns | 1,108,876.05 ns | 1,037,243.34 ns | 4900.0000 | 500.0000 |     - | 31172435 B |
| ImmutableDictionary_RepeatedAdd_SpecificComparer | 30000 | 86,848,330.9 ns | 1,690,957.96 ns | 2,632,617.83 ns | 4900.0000 | 500.0000 |     - | 31172446 B |

Again, adding a benchmark which uses an existing empty dictionary to create the builder. This is the fastest and least allocatey.

|                                                                   Method | Count |       Mean |    Error |   StdDev |     Median |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------------------------------------------------------------------- |------ |-----------:|---------:|---------:|-----------:|-------:|------:|------:|----------:|
|                          ImmutableDictionary_CreateRange_DefaultComparer |     3 | 1,139.9 ns | 22.73 ns | 60.68 ns | 1,137.2 ns | 0.0519 |     - |     - |     305 B |
|                         ImmutableDictionary_CreateRange_SpecificComparer |     3 | 1,438.8 ns | 28.75 ns | 48.82 ns | 1,434.1 ns | 0.0641 |     - |     - |     377 B |
|                              ImmutableDictionary_Builder_DefaultComparer |     3 |   914.6 ns | 32.86 ns | 95.86 ns |   878.5 ns | 0.0488 |     - |     - |     289 B |
|                             ImmutableDictionary_Builder_SpecificComparer |     3 | 1,033.5 ns | 19.21 ns | 30.46 ns | 1,022.8 ns | 0.0626 |     - |     - |     361 B |
| ImmutableDictionary_Builder_SpecificComparer_FromExistingEmptyDictionary |     3 |   753.8 ns |  6.98 ns |  6.53 ns |   753.0 ns | 0.0496 |     - |     - |     289 B |
|                          ImmutableDictionary_RepeatedAdd_DefaultComparer |     3 | 1,165.7 ns | 11.53 ns | 10.79 ns | 1,167.0 ns | 0.0763 |     - |     - |     441 B |
|                         ImmutableDictionary_RepeatedAdd_SpecificComparer |     3 | 1,108.1 ns | 12.79 ns | 11.34 ns | 1,107.2 ns | 0.0763 |     - |     - |     441 B |
 
 */

        private IEnumerable<string> enumerable;
        private ImmutableArray<string> immutableArray;
        private IEnumerable<KeyValuePair<string, string>> keyValuePairs;

        public const int Iterations = 10;

        [Params(3/*, 300, 30_000*/)]
        public int Count;

        private static readonly ImmutableDictionary<string, string> EmptyOrdinal = ImmutableDictionary<string, string>.Empty.WithComparers(SpecificComparer);
        private static readonly IEqualityComparer<string> SpecificComparer = StringComparer.Ordinal; //EqualityComparer<string>.Default;

        [GlobalSetup]
        public void Setup()
        {
            enumerable = Enumerable.Range(0, Count).Select(i => i.ToString());
            immutableArray = enumerable.ToImmutableArray();
            keyValuePairs = immutableArray.Select(s => new KeyValuePair<string,string>(s, s));
        }

        [Benchmark(OperationsPerInvoke = Iterations)]
        public ImmutableDictionary<string, string> ImmutableDictionary_CreateRange_DefaultComparer()
        {
            ImmutableDictionary<string, string> dic = null;
            for (int i = 0; i < Iterations; i++)
                dic = ImmutableDictionary.CreateRange(keyValuePairs);
            return dic;
        }

        [Benchmark(OperationsPerInvoke = Iterations)]
        public ImmutableDictionary<string, string> ImmutableDictionary_CreateRange_SpecificComparer()
        {
            ImmutableDictionary<string, string> dic = null;
            for (int i = 0; i < Iterations; i++)
                dic = ImmutableDictionary.CreateRange(SpecificComparer, keyValuePairs);
            return dic;
        }

        [Benchmark(OperationsPerInvoke = Iterations)]
        public ImmutableDictionary<string, string> ImmutableDictionary_Builder_DefaultComparer()
        {
            ImmutableDictionary<string, string> dic = null;
            for (int i = 0; i < Iterations; i++)
            {
                var builder = ImmutableDictionary.CreateBuilder<string, string>();
                for (int j = 0; j < Count; j++)
                    builder.Add(immutableArray[j], immutableArray[j]);
                dic = builder.ToImmutable();
            }

            return dic;
        }

        [Benchmark(OperationsPerInvoke = Iterations)]
        public ImmutableDictionary<string, string> ImmutableDictionary_Builder_SpecificComparer()
        {
            ImmutableDictionary<string, string> dic = null;
            for (int i = 0; i < Iterations; i++)
            {
                var builder = ImmutableDictionary.CreateBuilder<string, string>(SpecificComparer);
                for (int j = 0; j < Count; j++)
                    builder.Add(immutableArray[j], immutableArray[j]);
                dic = builder.ToImmutable();
            }

            return dic;
        }

        [Benchmark(OperationsPerInvoke = Iterations)]
        public ImmutableDictionary<string, string> ImmutableDictionary_Builder_SpecificComparer_FromExistingEmptyDictionary()
        {
            ImmutableDictionary<string, string> dic = null;
            for (int i = 0; i < Iterations; i++)
            {
                var builder = EmptyOrdinal.ToBuilder();
                for (int j = 0; j < Count; j++)
                    builder.Add(immutableArray[j], immutableArray[j]);
                dic = builder.ToImmutable();
            }

            return dic;
        }

        [Benchmark(OperationsPerInvoke = Iterations)]
        public ImmutableDictionary<string, string> ImmutableDictionary_RepeatedAdd_DefaultComparer()
        {
            ImmutableDictionary<string, string> dic = null;
            for (int i = 0; i < Iterations; i++)
            {
                dic = EmptyOrdinal;
                for (int j = 0; j < Count; j++)
                    dic = dic.Add(immutableArray[j], immutableArray[j]);
            }

            return dic;
        }

        [Benchmark(OperationsPerInvoke = Iterations)]
        public ImmutableDictionary<string, string> ImmutableDictionary_RepeatedAdd_SpecificComparer()
        {
            ImmutableDictionary<string, string> dic = null;
            for (int i = 0; i < Iterations; i++)
            {
                dic = ImmutableDictionary<string, string>.Empty;
                for (int j = 0; j < Count; j++)
                    dic = dic.Add(immutableArray[j], immutableArray[j]);
            }
            return dic;
        }
    }
}