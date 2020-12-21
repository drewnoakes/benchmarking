using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmarking
{
    [SimpleJob(RuntimeMoniker.Net48)]
//    [ShortRunJob]
    [MemoryDiagnoser]
    public class ImmutableDictionaryInterfaceEnumerationBenchmarks
    {
        private readonly ImmutableDictionary<string, int> _dictionary;
        private readonly IImmutableDictionary<string, int> _interface;

        public ImmutableDictionaryInterfaceEnumerationBenchmarks()
        {
            _interface = _dictionary = Enumerable.Range(0, 1000).ToImmutableDictionary(i => i.ToString(), i => i);
        }

/*
|                                Method |     Mean |   Error |  StdDev | Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------------------------------------- |---------:|--------:|--------:|------:|------:|------:|----------:|
| ForEachInterface_OptimisticNoAllocate | 212.2 us | 1.70 us | 1.59 us |     - |     - |     - |         - |
|                      ForEachInterface | 266.9 us | 2.24 us | 2.09 us |     - |     - |     - |     156 B |
|                       ForEachConcrete | 210.2 us | 1.44 us | 1.20 us |     - |     - |     - |         - |
*/

        [Benchmark]
        public long ForEachInterface_OptimisticNoAllocate()
        {
            long l = 0;

            var enumerable = new ImmutableDictionaryEnumerable<string, int>(_interface);
            
            foreach (var (key, value) in enumerable)
            {
                l += key.Length;
                l += value;
            }

            return l;
        }
        
        [Benchmark]
        public long ForEachInterface()
        {
            long l = 0;

            foreach (var (key, value) in _interface)
            {
                l += key.Length;
                l += value;
            }

            return l;
        }
        
        [Benchmark]
        public long ForEachConcrete()
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

    internal readonly struct ImmutableDictionaryEnumerable<TKey, TValue>
    {
        private readonly IImmutableDictionary<TKey, TValue> _dic;
        public ImmutableDictionaryEnumerable(IImmutableDictionary<TKey, TValue> dic) => _dic = dic;
        public Enumerator GetEnumerator() => new(_dic);

        internal struct Enumerator : IDisposable
        {
            //            private readonly bool _isConcrete;
            private ImmutableDictionary<TKey, TValue>.Enumerator _concreteEnumerator;
            //            private IEnumerator<KeyValuePair<TKey, TValue>> _fallbackEnumerator;

            public Enumerator(IImmutableDictionary<TKey, TValue> dic)
            {
                _concreteEnumerator = ((ImmutableDictionary<TKey, TValue>)dic).GetEnumerator();
                //                if (dic is ImmutableDictionary<TKey, TValue> concrete)
                //                {
                ////                    _isConcrete = true;
                //                    _concreteEnumerator = concrete.GetEnumerator();
                ////                    _fallbackEnumerator = null;
                //                }
                //                else throw new Exception();

                //                else
                //                {
                //                    _isConcrete = false;
                //                    _fallbackEnumerator = dic.GetEnumerator();
                //                    _concreteEnumerator = default;
                //                }
            }

            public bool MoveNext()
            {
                return /*_isConcrete ? */_concreteEnumerator.MoveNext()/* : _fallbackEnumerator.MoveNext()*/;
            }

            public KeyValuePair<TKey, TValue> Current => /*_isConcrete ? */_concreteEnumerator.Current/* : _fallbackEnumerator.Current*/;

            public void Dispose() => _concreteEnumerator.Dispose();
        }
    }
}