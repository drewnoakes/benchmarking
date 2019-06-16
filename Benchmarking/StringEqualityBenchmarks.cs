﻿using System;
using BenchmarkDotNet.Attributes;

namespace Benchmarking
{
    [ClrJob]
    [MemoryDiagnoser]
    public class StringEqualityBenchmarks
    {
        [Benchmark]
        public bool StaticStringEquals()
        {
            return string.Equals("Foo", "FOO", StringComparison.OrdinalIgnoreCase);
        }

        [Benchmark]
        public bool InstanceStringEquals()
        {
            return "Foo".Equals("FOO", StringComparison.OrdinalIgnoreCase);
        }

        [Benchmark]
        public bool StringComparerEquals()
        {
            return StringComparer.OrdinalIgnoreCase.Equals("Foo", "FOO");
        }
    }
}