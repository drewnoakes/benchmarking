using System;
using BenchmarkDotNet.Attributes;

namespace Benchmarking
{
    [ClrJob]
    [RankColumn]
    [MemoryDiagnoser]
    public class StringFormatBenchmarks
    {
        /*
        */
        private readonly Guid guid = Guid.NewGuid();
        private int i = 1038;

//        [Benchmark]
//        public string Format()
//        {
//            // ReSharper disable once UseStringInterpolation
//            return string.Format("{0};{1}", guid, i);
//        }
//
//        [Benchmark]
//        public string Interpolation()
//        {
//            return $"{guid};{i}";
//        }
//
//        [Benchmark]
//        public string Concat_Objects()
//        {
//            return string.Concat(guid, ";", i);
//        }

        [Benchmark]
        public string Concat_Strings()
        {
            return string.Concat(guid.ToString(), ";", i.ToString());
        }

        [Benchmark]
        public string Concat_Strings_FormatSpecifier()
        {
            return string.Concat(guid.ToString("D"), ";", i.ToString());
        }
    }
}