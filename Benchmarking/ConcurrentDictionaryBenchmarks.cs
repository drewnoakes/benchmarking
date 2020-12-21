using System.Collections.Concurrent;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmarking
{
    [SimpleJob(RuntimeMoniker.Net48)]
    [MemoryDiagnoser]
    public class ConcurrentDictionaryBenchmarks
    {
        /*
        |                             Method | Count |          Mean |         Error |        StdDev |   Gen 0 |  Gen 1 | Allocated |
        |----------------------------------- |------ |--------------:|--------------:|--------------:|--------:|-------:|----------:|
        | ConcurrentDictionary_Concurrency32 |     0 |     370.23 ns |    13.7004 ns |    12.8154 ns |  0.1655 | 0.0005 |     868 B |
        | ConcurrentDictionary_Concurrency16 |     0 |     223.92 ns |    18.6681 ns |    24.9214 ns |  0.0920 |      - |     484 B |
        |  ConcurrentDictionary_Concurrency8 |     0 |     127.51 ns |     2.6457 ns |     2.2093 ns |  0.0556 |      - |     292 B |
        |  ConcurrentDictionary_Concurrency4 |     0 |      90.44 ns |     0.5310 ns |     0.4707 ns |  0.0373 |      - |     196 B |
        |  ConcurrentDictionary_Concurrency2 |     0 |      71.72 ns |     0.4975 ns |     0.3884 ns |  0.0281 |      - |     148 B |
        |  ConcurrentDictionary_Concurrency1 |     0 |      61.67 ns |      1.297 ns |      1.332 ns |  0.0236 |      - |     124 B |
        |                         Dictionary |     0 |     16.65 ns  |     0.2444 ns |     0.2041 ns |  0.0091 |      - |      48 B |
        | ConcurrentDictionary_Concurrency32 |     1 |     451.41 ns |     6.1107 ns |     5.7159 ns |  0.1698 | 0.0005 |     892 B |
        | ConcurrentDictionary_Concurrency16 |     1 |     291.28 ns |     2.2490 ns |     2.1037 ns |  0.0968 |      - |     508 B |
        |  ConcurrentDictionary_Concurrency8 |     1 |     209.89 ns |     2.1512 ns |     2.0123 ns |  0.0601 |      - |     316 B |
        |  ConcurrentDictionary_Concurrency4 |     1 |     170.77 ns |     4.7882 ns |     5.1233 ns |  0.0417 |      - |     220 B |
        |  ConcurrentDictionary_Concurrency2 |     1 |     162.44 ns |     3.3334 ns |     9.4018 ns |  0.0327 |      - |     172 B |
        |  ConcurrentDictionary_Concurrency1 |     1 |     143.28 ns |      2.210 ns |      2.067 ns |  0.0281 |      - |     148 B |
        |                         Dictionary |     1 |      70.65 ns |     1.0488 ns |     0.8189 ns |  0.0252 |      - |     132 B |
        | ConcurrentDictionary_Concurrency32 |    10 |   1,164.41 ns |    22.7543 ns |    26.2039 ns |  0.2098 |      - |    1108 B |
        | ConcurrentDictionary_Concurrency16 |    10 |   1,016.14 ns |    13.6054 ns |    11.3612 ns |  0.1373 |      - |     724 B |
        |  ConcurrentDictionary_Concurrency8 |    10 |   1,502.08 ns |    12.0815 ns |    10.7099 ns |  0.1755 |      - |     928 B |
        |  ConcurrentDictionary_Concurrency4 |    10 |   1,268.21 ns |     9.6514 ns |     9.0279 ns |  0.1583 |      - |     832 B |
        |  ConcurrentDictionary_Concurrency2 |    10 |     818.93 ns |     9.3175 ns |     7.7805 ns |  0.0792 |      - |     420 B |
        |  ConcurrentDictionary_Concurrency1 |    10 |     804.48 ns |      5.487 ns |      4.581 ns |  0.0753 |      - |     400 B |
        |                         Dictionary |    10 |    268.57 ns  |     6.7485 ns |     9.6785 ns |  0.0553 |      - |     292 B |
        | ConcurrentDictionary_Concurrency32 |   100 |  11,349.97 ns |   126.0412 ns |   105.2501 ns |  1.2970 | 0.0305 |    6880 B |
        | ConcurrentDictionary_Concurrency16 |   100 |  10,859.56 ns |   335.0693 ns |   329.0829 ns |  1.2360 | 0.0305 |    6496 B |
        |  ConcurrentDictionary_Concurrency8 |   100 |  11,377.01 ns |   215.2443 ns |   190.8084 ns |  1.1902 | 0.0305 |    6304 B |
        |  ConcurrentDictionary_Concurrency4 |   100 |   7,928.09 ns |    57.5768 ns |    48.0792 ns |  0.5646 |      - |    2980 B |
        |  ConcurrentDictionary_Concurrency2 |   100 |   7,941.07 ns |    66.2525 ns |    58.7311 ns |  0.5493 |      - |    2940 B |
        |  ConcurrentDictionary_Concurrency1 |   100 |   7,788.00 ns |     89.199 ns |     69.641 ns |  0.5493 |      - |    2920 B |
        |                         Dictionary |   100 |  2,056.53 ns  |    40.8145 ns |    48.5867 ns |  0.4196 | 0.0038 |    2212 B |
        | ConcurrentDictionary_Concurrency32 |  1000 | 104,636.70 ns | 3,572.5065 ns | 3,341.7244 ns | 11.4746 | 0.1221 |   60761 B |
        | ConcurrentDictionary_Concurrency16 |  1000 | 102,770.06 ns |   589.9337 ns |   492.6213 ns | 11.4746 | 0.2441 |   60377 B |
        |  ConcurrentDictionary_Concurrency8 |  1000 |  72,621.69 ns |   849.4132 ns |   663.1659 ns |  5.3711 | 0.1221 |   28261 B |
        |  ConcurrentDictionary_Concurrency4 |  1000 |  75,603.64 ns | 1,841.3842 ns | 1,537.6390 ns |  5.3711 | 0.1221 |   28181 B |
        |  ConcurrentDictionary_Concurrency2 |  1000 |  74,025.54 ns |   433.6287 ns |   362.0996 ns |  5.2490 | 0.8545 |   28141 B |
        |  ConcurrentDictionary_Concurrency1 |  1000 |  77,335.37 ns |  1,668.697 ns |  3,556.129 ns |  5.2490 | 0.1221 |   28121 B |
        |                         Dictionary |  1000 | 20,503.37 ns  |   419.3512 ns |   559.8218 ns |  4.2114 | 0.5188 |   22148 B |
        */
        [Params(0, 1, 10, 100, 1000)]
        public int Count;

/*
        [Benchmark]
        public ConcurrentDictionary<int, string> ConcurrentDictionary_Concurrency32()
        {
            var dic = new ConcurrentDictionary<int, string>(concurrencyLevel: 32, capacity: Count);

            for (int i = 0; i < Count; i++)
            {
                dic[i] = "Hello";
            }

            return dic;
        }

        [Benchmark]
        public ConcurrentDictionary<int, string> ConcurrentDictionary_Concurrency16()
        {
            var dic = new ConcurrentDictionary<int, string>(concurrencyLevel: 16, capacity: Count);

            for (int i = 0; i < Count; i++)
            {
                dic[i] = "Hello";
            }

            return dic;
        }

        [Benchmark]
        public ConcurrentDictionary<int, string> ConcurrentDictionary_Concurrency8()
        {
            var dic = new ConcurrentDictionary<int, string>(concurrencyLevel: 8, capacity: Count);

            for (int i = 0; i < Count; i++)
            {
                dic[i] = "Hello";
            }

            return dic;
        }

        [Benchmark]
        public ConcurrentDictionary<int, string> ConcurrentDictionary_Concurrency4()
        {
            var dic = new ConcurrentDictionary<int, string>(concurrencyLevel: 4, capacity: Count);

            for (int i = 0; i < Count; i++)
            {
                dic[i] = "Hello";
            }

            return dic;
        }

        [Benchmark]
        public ConcurrentDictionary<int, string> ConcurrentDictionary_Concurrency2()
        {
            var dic = new ConcurrentDictionary<int, string>(concurrencyLevel: 2, capacity: Count);

            for (int i = 0; i < Count; i++)
            {
                dic[i] = "Hello";
            }

            return dic;
        }
*/

        [Benchmark]
        public ConcurrentDictionary<int, string> ConcurrentDictionary_Concurrency1()
        {
            var dic = new ConcurrentDictionary<int, string>(concurrencyLevel: 1, capacity: Count);

            for (int i = 0; i < Count; i++)
            {
                dic[i] = "Hello";
            }

            return dic;
        }

/*
        [Benchmark]
        public Dictionary<int, string> Dictionary()
        {
            var dic = new Dictionary<int, string>(capacity: Count);

            for (int i = 0; i < Count; i++)
            {
                dic[i] = "Hello";
            }

            return dic;
        }
*/
    }
}