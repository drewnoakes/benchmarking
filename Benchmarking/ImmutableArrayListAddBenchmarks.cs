using System.Collections.Immutable;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Benchmarking
{
    [ClrJob]
    [RankColumn]
    [MemoryDiagnoser]
    public class ImmutableArrayListAddBenchmarks
    {
        /*
                          Method | Count |            Mean |         Error |         StdDev | Rank |    Gen 0 |  Gen 1 | Allocated |
        ------------------------ |------ |----------------:|--------------:|---------------:|-----:|---------:|-------:|----------:|
              ImmutableArray_Add |     0 |       7.6652 ns |     0.0908 ns |      0.0759 ns |    2 |        - |      - |       0 B |
               ImmutableList_Add |     0 |       0.5266 ns |     0.1085 ns |      0.1901 ns |    1 |        - |      - |       0 B |
         ImmutableArray_AddRange |     0 |      96.2422 ns |     2.3104 ns |      3.5970 ns |    5 |        - |      - |       0 B |
          ImmutableList_AddRange |     0 |     113.0067 ns |     1.1714 ns |      0.9782 ns |    6 |   0.0021 |      - |      12 B |
              ImmutableArray_Add |     1 |      25.4626 ns |     0.4232 ns |      0.3752 ns |    3 |   0.0030 |      - |      16 B |
               ImmutableList_Add |     1 |      38.9691 ns |     0.5280 ns |      0.4681 ns |    4 |   0.0076 |      - |      40 B |
         ImmutableArray_AddRange |     1 |     228.3424 ns |     4.5940 ns |      7.0155 ns |    8 |   0.0029 |      - |      16 B |
          ImmutableList_AddRange |     1 |     181.8192 ns |     3.6904 ns |      3.6245 ns |    7 |   0.0098 |      - |      52 B |
              ImmutableArray_Add |    10 |     515.3996 ns |    10.1598 ns |     12.4772 ns |   11 |   0.0639 |      - |     340 B |
               ImmutableList_Add |    10 |   1,198.0503 ns |    22.8180 ns |     20.2276 ns |   14 |   0.2079 |      - |    1100 B |
         ImmutableArray_AddRange |    10 |     264.4922 ns |     5.8107 ns |     13.1156 ns |    9 |   0.0095 |      - |      52 B |
          ImmutableList_AddRange |    10 |     569.5811 ns |    12.1038 ns |     24.4502 ns |   12 |   0.0572 |      - |     304 B |
              ImmutableArray_Add |   100 |  10,865.3452 ns |   215.6425 ns |    586.6714 ns |   16 |   4.0741 |      - |   21400 B |
               ImmutableList_Add |   100 |  22,706.3288 ns |   400.6623 ns |    334.5711 ns |   17 |   3.8147 | 0.0305 |   20044 B |
         ImmutableArray_AddRange |   100 |     355.9512 ns |     3.5549 ns |      3.3253 ns |   10 |   0.0782 |      - |     412 B |
          ImmutableList_AddRange |   100 |   4,303.3618 ns |    84.2021 ns |     74.6430 ns |   15 |   0.5341 |      - |    2824 B |
              ImmutableArray_Add |  1000 | 557,859.9208 ns | 2,309.8162 ns |  1,928.8010 ns |   20 | 383.7891 | 2.9297 | 2014514 B |
               ImmutableList_Add |  1000 | 389,242.6636 ns | 7,731.5839 ns | 11,806.9453 ns |   19 |  55.1758 | 0.4883 |  291358 B |
         ImmutableArray_AddRange |  1000 |   1,108.3424 ns |    19.9701 ns |     16.6760 ns |   13 |   0.7648 |      - |    4014 B |
          ImmutableList_AddRange |  1000 |  37,008.4613 ns |   716.1815 ns |    766.3061 ns |   18 |   5.3101 | 0.0610 |   28024 B |
         */
        [Params(0, 1, 10, 100, 1000)]
        public int Count;

        private string[] strings;

        [GlobalSetup]
        public void Setup()
        {
            strings = Enumerable.Repeat("Foo", Count).ToArray();
        }

        [Benchmark]
        public ImmutableArray<string> ImmutableArray_Add()
        {
            var array = ImmutableArray.Create<string>();

            for (int i = 0; i < Count; i++)
            {
                array = array.Add("Foo");
            }

            return array;
        }

        [Benchmark]
        public ImmutableList<string> ImmutableList_Add()
        {
            var array = ImmutableList.Create<string>();

            for (int i = 0; i < Count; i++)
            {
                array = array.Add("Foo");
            }

            return array;
        }

        [Benchmark]
        public ImmutableArray<string> ImmutableArray_AddRange()
        {
            return ImmutableArray.Create<string>().AddRange(strings);
        }

        [Benchmark]
        public ImmutableList<string> ImmutableList_AddRange()
        {
            return ImmutableList.Create<string>().AddRange(strings);
        }
    }
}