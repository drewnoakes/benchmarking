using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Benchmarking
{
    [ClrJob]
    [MemoryDiagnoser]
    public class ImmutableArrayConstructionBenchmarks
    {
        /*
                                                 Method | Count |        Mean |       Error |      StdDev |      Median | Rank |  Gen 0 | Allocated |
        ----------------------------------------------- |------ |------------:|------------:|------------:|------------:|-----:|-------:|----------:|
                  ImmutableArray_CreateRange_Enumerable |     3 |   429.72 ns |  15.2872 ns |  20.9253 ns |   421.48 ns |   13 | 0.0243 |     128 B |
                       ImmutableArray_CreateRange_Array |     3 |   204.44 ns |   4.4029 ns |  12.8435 ns |   198.11 ns |    9 | 0.0045 |      24 B |
              ImmutableArray_CreateRange_ImmutableArray |     3 |    31.23 ns |   0.4979 ns |   0.4158 ns |    31.09 ns |    1 | 0.0023 |      12 B |
               ImmutableArray_CreateRange_ImmutableList |     3 |   574.94 ns |   5.6508 ns |   5.2857 ns |   575.29 ns |   14 | 0.0134 |      72 B |
             ImmutableArray_Builder_AddRange_Enumerable |     3 |   353.28 ns |   4.9435 ns |   3.8595 ns |   352.27 ns |   11 | 0.0286 |     152 B |
                  ImmutableArray_Builder_AddRange_Array |     3 |    98.69 ns |   0.4517 ns |   0.3772 ns |    98.78 ns |    3 | 0.0144 |      76 B |
         ImmutableArray_Builder_AddRange_ImmutableArray |     3 |   107.89 ns |   1.7315 ns |   1.4459 ns |   107.21 ns |    5 | 0.0144 |      76 B |
          ImmutableArray_Builder_AddRange_ImmutableList |     3 |   618.24 ns |  12.4143 ns |  11.0049 ns |   616.77 ns |   15 | 0.0229 |     124 B |
                       ImmutableArray_Builder_Add_Array |     3 |    92.54 ns |   0.5342 ns |   0.4997 ns |    92.43 ns |    2 | 0.0144 |      76 B |
              ImmutableArray_Builder_Add_ImmutableArray |     3 |   100.75 ns |   2.2130 ns |   4.2637 ns |   101.90 ns |    4 | 0.0144 |      76 B |
               ImmutableArray_Builder_Add_ImmutableList |     3 |   112.56 ns |   1.8465 ns |   1.7272 ns |   113.11 ns |    6 | 0.0143 |      76 B |

                  ImmutableArray_CreateRange_Enumerable |    30 | 1,589.95 ns |  45.8680 ns | 135.2427 ns | 1,540.94 ns |   18 | 0.0935 |     496 B |
                       ImmutableArray_CreateRange_Array |    30 |   217.30 ns |   1.1259 ns |   0.9402 ns |   217.57 ns |   10 | 0.0250 |     132 B |
              ImmutableArray_CreateRange_ImmutableArray |    30 |    31.26 ns |   0.3311 ns |   0.2765 ns |    31.29 ns |    1 | 0.0023 |      12 B |
               ImmutableArray_CreateRange_ImmutableList |    30 | 2,939.73 ns | 145.5122 ns | 189.2070 ns | 2,851.85 ns |   20 | 0.0305 |     180 B |
             ImmutableArray_Builder_AddRange_Enumerable |    30 | 1,125.97 ns |  23.1135 ns |  66.6878 ns | 1,126.38 ns |   17 | 0.0687 |     368 B |
                  ImmutableArray_Builder_AddRange_Array |    30 |   159.87 ns |   9.7224 ns |  14.2510 ns |   152.43 ns |    8 | 0.0556 |     292 B |
         ImmutableArray_Builder_AddRange_ImmutableArray |    30 |   147.64 ns |   3.0160 ns |   3.8142 ns |   147.27 ns |    7 | 0.0556 |     292 B |
          ImmutableArray_Builder_AddRange_ImmutableList |    30 | 2,805.17 ns |  55.7564 ns | 138.8528 ns | 2,790.15 ns |   19 | 0.0610 |     340 B |
                       ImmutableArray_Builder_Add_Array |    30 |   387.86 ns |   7.7780 ns |  17.2356 ns |   381.87 ns |   12 | 0.0553 |     292 B |
              ImmutableArray_Builder_Add_ImmutableArray |    30 |   384.65 ns |   4.0676 ns |   3.8048 ns |   384.31 ns |   12 | 0.0553 |     292 B |
               ImmutableArray_Builder_Add_ImmutableList |    30 |   831.12 ns |  16.6728 ns |  29.6360 ns |   823.44 ns |   16 | 0.0553 |     292 B |
         */
        private IEnumerable<string> enumerable;
        private string[] array;
        private ImmutableArray<string> immutableArray;
        private ImmutableList<string> immutableList;

        [Params(3, 30)]
        public int Count;

        [GlobalSetup]
        public void Setup()
        {
            enumerable = Enumerable.Range(0, Count).Select(_ => "");
            array = enumerable.ToArray();
            immutableArray = array.ToImmutableArray();
            immutableList = array.ToImmutableList();
        }

        [Benchmark]
        public ImmutableArray<string> ImmutableArray_CreateRange_Enumerable()
        {
            return ImmutableArray.CreateRange(enumerable);
        }

        [Benchmark]
        public ImmutableArray<string> ImmutableArray_CreateRange_Array()
        {
            return ImmutableArray.CreateRange(array);
        }

        [Benchmark]
        public ImmutableArray<string> ImmutableArray_CreateRange_ImmutableArray()
        {
            return ImmutableArray.CreateRange(immutableArray);
        }

        [Benchmark]
        public ImmutableArray<string> ImmutableArray_CreateRange_ImmutableList()
        {
            return ImmutableArray.CreateRange(immutableList);
        }

        ////////////

        [Benchmark]
        public IImmutableList<string> ImmutableArray_Builder_AddRange_Enumerable()
        {
            var builder = ImmutableArray.CreateBuilder<string>(Count);
            builder.AddRange(enumerable);
            return builder.ToImmutable();
        }

        [Benchmark]
        public IImmutableList<string> ImmutableArray_Builder_AddRange_Array()
        {
            var builder = ImmutableArray.CreateBuilder<string>(array.Length);
            builder.AddRange(array);
            return builder.ToImmutable();
        }

        [Benchmark]
        public IImmutableList<string> ImmutableArray_Builder_AddRange_ImmutableArray()
        {
            var builder = ImmutableArray.CreateBuilder<string>(immutableArray.Length);
            builder.AddRange(immutableArray);
            return builder.ToImmutable();
        }

        [Benchmark]
        public IImmutableList<string> ImmutableArray_Builder_AddRange_ImmutableList()
        {
            var builder = ImmutableArray.CreateBuilder<string>(immutableList.Count);
            builder.AddRange(immutableList);
            return builder.ToImmutable();
        }

        ////////////

        [Benchmark]
        public IImmutableList<string> ImmutableArray_Builder_Add_Array()
        {
            var builder = ImmutableArray.CreateBuilder<string>(array.Length);

            for (int i = 0; i < array.Length; i++)
            {
                builder.Add(array[i]);
            }

            return builder.ToImmutable();
        }

        [Benchmark]
        public IImmutableList<string> ImmutableArray_Builder_Add_ImmutableArray()
        {
            var builder = ImmutableArray.CreateBuilder<string>(immutableArray.Length);

            for (int i = 0; i < immutableArray.Length; i++)
            {
                builder.Add(immutableArray[i]);
            }

            return builder.ToImmutable();
        }

        [Benchmark]
        public IImmutableList<string> ImmutableArray_Builder_Add_ImmutableList()
        {
            var builder = ImmutableArray.CreateBuilder<string>(immutableList.Count);

            for (int i = 0; i < immutableList.Count; i++)
            {
                builder.Add(immutableList[i]);
            }

            return builder.ToImmutable();
        }
    }
}