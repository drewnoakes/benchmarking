using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using BenchmarkDotNet.Attributes;

namespace Benchmarking
{
    [ClrJob]
    [MemoryDiagnoser]
    public class BuildImmutableFromSetBenchmarks
    {
        /*
|                           Method | Count |           Mean |         Error |        StdDev |         Median |   Gen 0 |  Gen 1 | Allocated |
|--------------------------------- |------ |---------------:|--------------:|--------------:|---------------:|--------:|-------:|----------:|
|       OptimisticImmutableHashSet |     0 |       621.5 ns |     12.103 ns |     15.306 ns |       616.6 ns |  0.0029 |      - |      20 B |
|          HashSetToImmutableArray |     0 |       145.2 ns |      3.935 ns |     11.603 ns |       138.6 ns |  0.0074 |      - |      40 B |
|       OptimisticImmutableHashSet |     1 |       848.4 ns |     16.019 ns |     20.829 ns |       849.9 ns |  0.0134 |      - |      72 B |
|          HashSetToImmutableArray |     1 |       275.4 ns |      5.205 ns |      5.112 ns |       276.9 ns |  0.0286 |      - |     152 B |
|       OptimisticImmutableHashSet |    10 |     4,547.7 ns |     89.233 ns |    158.612 ns |     4,555.7 ns |  0.2289 |      - |    1212 B |
|          HashSetToImmutableArray |    10 |     1,579.6 ns |     19.275 ns |     17.086 ns |     1,583.4 ns |  0.1144 |      - |     620 B |
|       OptimisticImmutableHashSet |   100 |    71,160.5 ns |  1,463.607 ns |  4,055.647 ns |    70,170.6 ns |  4.3945 |      - |   23300 B |
|          HashSetToImmutableArray |   100 |    13,689.1 ns |    264.648 ns |    325.012 ns |    13,730.5 ns |  1.1749 |      - |    6220 B |
|       OptimisticImmutableHashSet |  1000 | 1,035,607.0 ns | 17,529.009 ns | 14,637.515 ns | 1,032,324.7 ns | 68.3594 | 3.9063 |  360931 B |
|          HashSetToImmutableArray |  1000 |   131,551.6 ns |  2,597.237 ns |  2,027.752 ns |   130,749.8 ns | 11.7188 | 1.2207 |   62429 B |
         */
        [Params(0, 1, 10, 100, 1000)]
        public int Count;

        private string[] _strings;
        private HashSet<string> _hashSet;
        private ImmutableHashSet<string> _immutableHashSet;

        [GlobalSetup]
        public void Setup()
        {
            _strings = Enumerable
                .Range(0, Count)
                .Select(i => i.ToString())
                .ToArray();
        }

        [Benchmark]
        public ImmutableHashSet<string> OptimisticImmutableHashSet()
        {
            _immutableHashSet = ImmutableHashSet.Create<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var s in _strings)
            {
                var before = Volatile.Read(ref _immutableHashSet);
                while (Interlocked.CompareExchange(ref _immutableHashSet, before.Add(s), before) != before)
                {

                }
            }

            return _immutableHashSet;
        }

        [Benchmark]
        public ImmutableArray<string> HashSetToImmutableArray()
        {
            _hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var s in _strings)
            {
                lock (_hashSet)
                {
                    _hashSet.Add(s);
                }
            }

            lock (_hashSet)
            {
                return ImmutableArray.CreateRange(_hashSet);
            }
        }
    }
}