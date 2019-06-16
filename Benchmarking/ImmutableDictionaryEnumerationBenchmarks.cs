using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using BenchmarkDotNet.Attributes;

namespace Benchmarking
{
    [ClrJob]
    [MemoryDiagnoser]
    public class ImmutableDictionaryEnumerationBenchmarks
    {
        private readonly ImmutableDictionary<string, int> _dictionary;

        public ImmutableDictionaryEnumerationBenchmarks()
        {
            _dictionary = Enumerable.Range(0, 1000).ToImmutableDictionary(i => i.ToString(), i => i);
        }

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