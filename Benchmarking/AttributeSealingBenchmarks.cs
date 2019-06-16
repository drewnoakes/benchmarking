using System;
using BenchmarkDotNet.Attributes;

namespace Benchmarking
{
    [ClrJob]
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