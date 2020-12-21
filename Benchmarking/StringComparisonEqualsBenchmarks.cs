using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmarking
{
    /*
        BenchmarkDotNet=v0.11.1, OS=Windows 10.0.18362
        Intel Xeon Silver 4110 CPU 2.10GHz, 2 CPU, 32 logical and 16 physical cores
          [Host] : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.8.3815.0
          Clr    : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.8.3815.0

        Job=Clr  Runtime=Clr

                Method | SameLength |      Mean |     Error |    StdDev |    Median | Allocated |
        -------------- |----------- |----------:|----------:|----------:|----------:|----------:|
              Comparer |      False |  3.520 ns | 0.1193 ns | 0.3441 ns |  3.421 ns |       0 B |
          StaticEquals |      False |  2.661 ns | 0.1319 ns | 0.3890 ns |  2.447 ns |       0 B |
          MemberEquals |      False |  2.805 ns | 0.0251 ns | 0.0222 ns |  2.800 ns |       0 B |
         StaticCompare |      False | 24.074 ns | 0.5388 ns | 1.1483 ns | 23.970 ns |       0 B |
              Comparer |       True | 27.444 ns | 0.6542 ns | 1.9288 ns | 27.454 ns |       0 B |
          StaticEquals |       True | 16.903 ns | 0.4885 ns | 1.4328 ns | 16.442 ns |       0 B |
          MemberEquals |       True | 17.859 ns | 0.6551 ns | 1.9315 ns | 17.567 ns |       0 B |
         StaticCompare |       True | 23.432 ns | 0.6640 ns | 1.9263 ns | 23.744 ns |       0 B |

        // * Warnings *
        MultimodalDistribution
          StringComparisonEqualsBenchmarks.Comparer: Clr      -> It seems that the distribution can have several modes (mValue = 3.06)
          StringComparisonEqualsBenchmarks.StaticEquals: Clr  -> It seems that the distribution can have several modes (mValue = 3.19)
          StringComparisonEqualsBenchmarks.Comparer: Clr      -> It seems that the distribution is bimodal (mValue = 3.71)
          StringComparisonEqualsBenchmarks.StaticCompare: Clr -> It seems that the distribution is bimodal (mValue = 3.46)
     */

    [SimpleJob(RuntimeMoniker.Net48)]
    [MemoryDiagnoser]
    public class StringComparisonEqualsBenchmarks
    {
        private string x;
        private string y;

        [Params(true, false)]
        public bool SameLength;

        [GlobalSetup]
        public void Setup()
        {
            x = "FrobbleBoo";
            y = SameLength ? "FrobbleBar" : "FrobbleBa";
        }

        [Benchmark]
        public bool Comparer()
        {
            return StringComparer.OrdinalIgnoreCase.Equals(x, y);
        }

        [Benchmark]
        public bool StaticEquals()
        {
            return string.Equals(x, y, StringComparison.OrdinalIgnoreCase);
        }

        [Benchmark]
        public bool MemberEquals()
        {
            return x != null && x.Equals(y, StringComparison.OrdinalIgnoreCase);
        }

        [Benchmark]
        public bool StaticCompare()
        {
            return String.Compare(x, y, StringComparison.OrdinalIgnoreCase) == 0;
        }
    }
}