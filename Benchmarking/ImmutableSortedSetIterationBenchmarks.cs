using System;
using System.Collections.Immutable;
using System.Linq;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmarking
{
    [SimpleJob(RuntimeMoniker.Net48)]
    [SimpleJob(RuntimeMoniker.CoreRt31)]
    public class ImmutableSortedSetIterationBenchmarks
    {
        /*
            |  Method |    Runtime | Count |           Mean |         Error |        StdDev |
            |-------- |----------- |------ |---------------:|--------------:|--------------:|
            | Foreach |   .NET 4.8 |     1 |     376.414 ns |     7.4909 ns |    12.9214 ns |
            | ForLoop |   .NET 4.8 |     1 |       4.992 ns |     0.0901 ns |     0.0753 ns |
            | Foreach | CoreRt 3.1 |     1 |     360.885 ns |     2.8646 ns |     2.6796 ns |
            | ForLoop | CoreRt 3.1 |     1 |       4.905 ns |     0.0653 ns |     0.0578 ns |
            | Foreach |   .NET 4.8 |    10 |   1,322.343 ns |    16.3168 ns |    12.7391 ns |
            | ForLoop |   .NET 4.8 |    10 |      77.545 ns |     1.5846 ns |     1.5562 ns |
            | Foreach | CoreRt 3.1 |    10 |   1,079.793 ns |    15.3006 ns |    12.7767 ns |
            | ForLoop | CoreRt 3.1 |    10 |      78.040 ns |     0.6698 ns |     0.6266 ns |
            | Foreach |   .NET 4.8 |    50 |   5,375.774 ns |    47.3741 ns |    44.3138 ns |
            | ForLoop |   .NET 4.8 |    50 |     553.756 ns |     4.5654 ns |     4.0471 ns |
            | Foreach | CoreRt 3.1 |    50 |   4,260.279 ns |    91.8036 ns |    85.8732 ns |
            | ForLoop | CoreRt 3.1 |    50 |     497.019 ns |     7.6752 ns |     7.1794 ns |
            | Foreach |   .NET 4.8 |   100 |  10,312.971 ns |   202.4086 ns |   189.3331 ns |
            | ForLoop |   .NET 4.8 |   100 |   1,206.282 ns |    14.0805 ns |    12.4820 ns |
            | Foreach | CoreRt 3.1 |   100 |   8,181.352 ns |   103.6609 ns |    91.8927 ns |
            | ForLoop | CoreRt 3.1 |   100 |   1,132.844 ns |    22.3796 ns |    21.9798 ns |
            | Foreach |   .NET 4.8 |   500 |  51,068.065 ns |   735.3326 ns |   687.8306 ns |
            | ForLoop |   .NET 4.8 |   500 |  22,065.454 ns |   354.1798 ns |   331.3000 ns |
            | Foreach | CoreRt 3.1 |   500 |  39,410.555 ns |   903.0918 ns |   927.4088 ns |
            | ForLoop | CoreRt 3.1 |   500 |  19,802.825 ns |   351.4609 ns |   328.7567 ns |
            | Foreach |   .NET 4.8 |  1000 | 103,140.166 ns | 2,051.8266 ns | 3,254.4144 ns |
            | ForLoop |   .NET 4.8 |  1000 |  49,174.719 ns |   974.7339 ns | 1,083.4141 ns |
            | Foreach | CoreRt 3.1 |  1000 |  78,940.633 ns | 1,278.5821 ns | 1,195.9865 ns |
            | ForLoop | CoreRt 3.1 |  1000 |  49,001.032 ns |   607.5614 ns |   568.3133 ns |
        */

        private ImmutableSortedSet<string> set;

        [Params(1000, 100_000, 10_000_000)]
//        [Params(1, 10, 50, 100, 500, 1000)]
        public int Count;

        [GlobalSetup]
        public void Setup()
        {
            set = ImmutableSortedSet.CreateRange(
                StringComparer.Ordinal,
                Enumerable.Range(0, Count).Select(i => i.ToString()));
        }

        [Benchmark]
        public int Foreach()
        {
            var sum = 0;
            
            foreach (var item in set)
            {
                sum += item.Length;
            }

            return sum;
        }

        [Benchmark]
        public int ForLoop()
        {
            var sum = 0;

            for (var i = 0; i < set.Count; i++)
            {
                sum += set[i].Length;
            }

            return sum;
        }
    }
}