using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmarking
{
    [SimpleJob(RuntimeMoniker.Net48)]
    [MemoryDiagnoser]
    public class AttributeSealingBenchmarks
    {
        /*
         */

        [Benchmark]
        public object[] GetUnsealedAttribute()
        {
            return typeof(UnsealedAttributeClass).GetCustomAttributes(typeof(UnsealedAttribute), false);
        }

        [Benchmark]
        public object[] GetSealedAttribute()
        {
            return typeof(SealedAttributeClass).GetCustomAttributes(typeof(SealedAttribute), false);
        }

        public class UnsealedAttribute : Attribute { }
        public sealed class SealedAttribute : Attribute { }

        [Unsealed] public class UnsealedAttributeClass { }
        [Sealed] public class SealedAttributeClass { }
    }
}