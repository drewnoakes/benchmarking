using System.Collections.Immutable;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmarking
{
    [SimpleJob(RuntimeMoniker.Net48)]
    [MemoryDiagnoser]
    public class ImmutableDictionaryEnumerationBenchmarks
    {
        private readonly ImmutableDictionary<string, int> _dictionary;

        public ImmutableDictionaryEnumerationBenchmarks()
        {
            _dictionary = Enumerable.Range(0, 1000).ToImmutableDictionary(i => i.ToString(), i => i);
        }

        /*
        |              Method |     Mean |   Error |  StdDev | Gen 0 | Gen 1 | Gen 2 | Allocated |
        |-------------------- |---------:|--------:|--------:|------:|------:|------:|----------:|
        | ForEachKeyValuePair | 206.0 us | 3.11 us | 2.91 us |     - |     - |     - |         - |
        |  ForEachDeconstruct | 206.6 us | 2.31 us | 1.93 us |     - |     - |     - |         - |
        */

        [Benchmark]
        public long ForEachKeyValuePair()
        {
            long l = 0;

            foreach (var pair in _dictionary)
            {
                l += pair.Key.Length;
                l += pair.Value;
            }

            return l;
        }

        [Benchmark]
        public long ForEachDeconstruct()
        {
            long l = 0;

            foreach (var (key, value) in _dictionary)
            {
                l += key.Length;
                l += value;
            }

            return l;
        }
    }
}