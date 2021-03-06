﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

#nullable enable

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
        | ForEachInterface_OptimisticNoAllocate | 221.4 us | 2.65 us | 2.35 us |     - |     - |     - |         - |
        |                      ForEachInterface | 271.0 us | 3.10 us | 2.90 us |     - |     - |     - |     156 B |
        |                       ForEachConcrete | 228.2 us | 1.39 us | 1.08 us |     - |     - |     - |         - |

        //

        |                                Method |     Mean |   Error |  StdDev | Gen 0 | Gen 1 | Gen 2 | Allocated |
        |-------------------------------------- |---------:|--------:|--------:|------:|------:|------:|----------:|
        | ForEachInterface_OptimisticNoAllocate | 212.6 us | 2.52 us | 2.36 us |     - |     - |     - |         - |
        |                      ForEachInterface | 263.7 us | 4.27 us | 4.00 us |     - |     - |     - |     156 B |
        |                       ForEachConcrete | 228.4 us | 1.84 us | 1.54 us |     - |     - |     - |         - |

        // without boolean (checking fallback for null)

        |                                Method |     Mean |   Error |  StdDev | Gen 0 | Gen 1 | Gen 2 | Allocated |
        |-------------------------------------- |---------:|--------:|--------:|------:|------:|------:|----------:|
        | ForEachInterface_OptimisticNoAllocate | 213.5 us | 4.03 us | 3.96 us |     - |     - |     - |         - |
        |                      ForEachInterface | 261.0 us | 2.37 us | 2.10 us |     - |     - |     - |     156 B |
        |                       ForEachConcrete | 208.1 us | 1.88 us | 1.76 us |     - |     - |     - |         - |
        */

        [Benchmark]
        public long ForEachInterface_OptimisticNoAllocate()
        {
            long l = 0;

            foreach (var (key, value) in _interface.NoAllocateEnumerate())
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

    internal static class ImmutableDictionaryEnumerationExtensions
    {
        public static ImmutableDictionaryEnumerable<TKey, TValue> NoAllocateEnumerate<TKey, TValue>(this IImmutableDictionary<TKey, TValue> dic) where TKey : notnull
        {
            return new ImmutableDictionaryEnumerable<TKey, TValue>(dic);
        }

        public readonly struct ImmutableDictionaryEnumerable<TKey, TValue> where TKey : notnull
        {
            private readonly IImmutableDictionary<TKey, TValue> _dic;
            public ImmutableDictionaryEnumerable(IImmutableDictionary<TKey, TValue> dic) => _dic = dic;
            public Enumerator GetEnumerator() => new(_dic);

            public struct Enumerator : IDisposable
            {
                // TODO faster to check fallback for null than bool?
                private readonly bool _isConcrete;
                private ImmutableDictionary<TKey, TValue>.Enumerator _concreteEnumerator;
                private readonly IEnumerator<KeyValuePair<TKey, TValue>>? _fallbackEnumerator;

                public Enumerator(IImmutableDictionary<TKey, TValue> dic)
                {
                    if (dic is ImmutableDictionary<TKey, TValue> concrete)
                    {
                        _isConcrete = true;
                        _concreteEnumerator = concrete.GetEnumerator();
                        _fallbackEnumerator = null;
                    }
                    else
                    {
                        _isConcrete = false;
                        _fallbackEnumerator = dic.GetEnumerator();
                        _concreteEnumerator = default;
                    }
                }

                public bool MoveNext() => _isConcrete ? _concreteEnumerator.MoveNext() : _fallbackEnumerator!.MoveNext();

                public KeyValuePair<TKey, TValue> Current => _isConcrete ? _concreteEnumerator.Current : _fallbackEnumerator!.Current;

                public void Dispose()
                {
                    if (_isConcrete)
                        _concreteEnumerator.Dispose();
                    else
                        _fallbackEnumerator!.Dispose();
                }
            }
        }
    }
}