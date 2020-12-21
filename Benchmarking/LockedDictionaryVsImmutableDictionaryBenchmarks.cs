using System;
using System.Collections.Generic;
using System.Collections.Immutable;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmarking
{
    [SimpleJob(RuntimeMoniker.Net48)]
    [MemoryDiagnoser]
    public class LockedDictionaryVsImmutableDictionaryBenchmarks
    {
        /*
        BenchmarkDotNet=v0.11.1, OS=Windows 10.0.18362
        Intel Xeon Silver 4110 CPU 2.10GHz, 2 CPU, 32 logical and 16 physical cores
          [Host] : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.8.3801.0
          Clr    : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.8.3801.0

        Job=Clr  Runtime=Clr

                                       Method |      Mean |    Error |    StdDev |    Median |  Gen 0 | Allocated |
        ------------------------------------- |----------:|---------:|----------:|----------:|-------:|----------:|
                              AcquireLockOnly |  27.57 ns | 1.016 ns |  2.997 ns |  25.90 ns |      - |       0 B |
             LockedDictionary_TryGetValue_Hit |  54.28 ns | 1.427 ns |  4.207 ns |  51.80 ns |      - |       0 B |
            LockedDictionary_TryGetValue_Miss |  44.11 ns | 1.148 ns |  3.384 ns |  43.43 ns |      - |       0 B |
          ImmutableDictionary_TryGetValue_Hit |  51.27 ns | 1.907 ns |  5.622 ns |  48.55 ns |      - |       0 B |
         ImmutableDictionary_TryGetValue_Miss |  35.94 ns | 1.020 ns |  3.007 ns |  34.60 ns |      - |       0 B |
               LockedDictionary_GetOrAdd_Miss |        NA |       NA |        NA |        NA |    N/A |       N/A |
            ImmutableDictionary_GetOrAdd_Hit  |  53.22 ns | 1.116 ns |  3.074 ns |  52.45 ns |      - |       0 B |
            ImmutableDictionary_GetOrAdd_Miss | 442.21 ns | 8.808 ns | 20.239 ns | 443.03 ns | 0.0291 |     168 B |
         */

        private readonly object _lock = new object();

        private ImmutableDictionary<long, int> _immutableDictionary = ImmutableDictionary<long, int>.Empty.Add(0, 0);
        private Dictionary<long, int> _dictionary = new Dictionary<long, int> { [0] = 0 };

        [Benchmark]
        public void AcquireLockOnly()
        {
            lock (_lock) { }
        }

        [Benchmark]
        public int LockedDictionary_TryGetValue_Hit()
        {
            lock (_lock)
            {
                if (_dictionary.TryGetValue(0, out int i))
                    return i;
                throw null;
            }
        }

        [Benchmark]
        public int LockedDictionary_TryGetValue_Miss()
        {
            lock (_lock)
            {
                if (_dictionary.TryGetValue(1, out int i))
                    throw null;
                return i;
            }
        }

        [Benchmark]
        public int ImmutableDictionary_TryGetValue_Hit()
        {
            if (_immutableDictionary.TryGetValue(0, out int i))
                return i;
            throw null;
        }

        [Benchmark]
        public int ImmutableDictionary_TryGetValue_Miss()
        {
            if (_immutableDictionary.TryGetValue(1, out int i))
                throw null;
            return i;
        }

        //////////////

        private long _nextMiss = 1;

        [Benchmark]
        public int LockedDictionary_GetOrAdd_Miss()
        {
            var key = _nextMiss++;

            lock (_lock)
            {
                if (_dictionary.TryGetValue(key, out int _))
                    throw null;
            }

            lock (_lock)
            {
                if (!_dictionary.ContainsKey(key))
                    _dictionary.Add(key, 1);
            }

            return 1;
        }

        [Benchmark]
        public int ImmutableDictionary_GetOrAdd_Hit()
        {
            var key = _nextMiss++;
            var dic = _immutableDictionary;
            return ImmutableInterlocked.GetOrAdd(ref dic, 0, k => throw null);
        }

        [Benchmark]
        public int ImmutableDictionary_GetOrAdd_Miss()
        {
            var key = _nextMiss++;
            var dic = _immutableDictionary;
            return ImmutableInterlocked.GetOrAdd(ref dic, key, k => 1);
        }
    }
}