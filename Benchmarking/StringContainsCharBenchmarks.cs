using System;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmarking
{
    [SimpleJob(RuntimeMoniker.Net48)]
    [MemoryDiagnoser]
    public class StringContainsCharBenchmarks
    {
        /*
                           Method |     Mean |     Error |    StdDev | Allocated |
        ------------------------- |---------:|----------:|----------:|----------:|
         ContainsSingleCharString | 74.99 ns | 0.6283 ns | 0.5246 ns |       0 B |
                      IndexOfChar | 15.49 ns | 0.1420 ns | 0.1259 ns |       0 B |
         */
        [Benchmark]
        public bool ContainsSingleCharString()
        {
            return "abcdefghijklmnopqrstuvwxyz.".Contains(".");
        }

        [Benchmark]
        public bool IndexOfChar()
        {
            return "abcdefghijklmnopqrstuvwxyz.".IndexOf('.') != -1;
        }
    }

    [SimpleJob(RuntimeMoniker.Net48)]
    [MemoryDiagnoser]
    public class CharArrayContainsCharBenchmarks
    {
        private static string str = "abcdefghijklmnopqrstuvwxyz.";
        private static char[] chars = str.ToCharArray();

        /*
            |                 Method |     Mean |    Error |   StdDev | Gen 0 | Gen 1 | Gen 2 | Allocated |
            |----------------------- |---------:|---------:|---------:|------:|------:|------:|----------:|
            | LinqContainsSingleChar | 80.66 ns | 0.747 ns | 0.699 ns |     - |     - |     - |         - |
            |       ArrayIndexOfChar | 37.02 ns | 0.734 ns | 0.650 ns |     - |     - |     - |         - |
            |      StringIndexOfChar | 15.91 ns | 0.102 ns | 0.080 ns |     - |     - |     - |         - |        
        */
        [Benchmark]
        public bool LinqContainsSingleChar()
        {
            return chars.Contains('.');
        }

        [Benchmark]
        public bool ArrayIndexOfChar()
        {
            return Array.IndexOf(chars, '.') != -1;
        }
        
        [Benchmark]
        public bool StringIndexOfChar()
        {
            return str.IndexOf('.') != -1;
        }
    }
}