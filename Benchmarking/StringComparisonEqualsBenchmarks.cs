using System;
using BenchmarkDotNet.Attributes;

namespace Benchmarking
{
    /*
        BenchmarkDotNet=v0.11.1, OS=Windows 10.0.18362
        Intel Xeon Silver 4110 CPU 2.10GHz, 2 CPU, 32 logical and 16 physical cores
          [Host] : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.8.3801.0
          Clr    : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.8.3801.0

        Job=Clr  Runtime=Clr

          Method | SameLength |      Mean |     Error |    StdDev |    Median | Allocated |
        -------- |----------- |----------:|----------:|----------:|----------:|----------:|
          Static |      False |  7.313 ns | 0.3302 ns | 0.3670 ns |  7.134 ns |       0 B |
          Member |      False |  3.243 ns | 0.1447 ns | 0.4267 ns |  3.037 ns |       0 B |
         Compare |      False | 29.313 ns | 0.2899 ns | 0.2711 ns | 29.293 ns |       0 B |
          Static |       True | 34.843 ns | 1.1148 ns | 3.2342 ns | 33.764 ns |       0 B |
          Member |       True | 22.241 ns | 1.2367 ns | 3.6465 ns | 20.983 ns |       0 B |
         Compare |       True | 28.628 ns | 0.2898 ns | 0.2420 ns | 28.604 ns |       0 B |

        // * Warnings *
        MultimodalDistribution
          StringComparisonEqualsBenchmarks.Member: Clr -> It seems that the distribution can have several modes (mValue = 2.87)
          StringComparisonEqualsBenchmarks.Member: Clr -> It seems that the distribution is bimodal (mValue = 3.87)
     */

    [ClrJob]
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
        public bool Static()
        {
            return StringComparer.OrdinalIgnoreCase.Equals(x, y);
        }

        [Benchmark]
        public bool Member()
        {
            return x != null && x.Equals(y, StringComparison.OrdinalIgnoreCase);
        }

        [Benchmark]
        public bool Compare()
        {
            return String.Compare(x, y, StringComparison.OrdinalIgnoreCase) == 0;
        }
    }
}