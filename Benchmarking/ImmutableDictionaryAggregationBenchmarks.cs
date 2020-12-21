using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmarking
{
    [SimpleJob(RuntimeMoniker.Net48)]
    [MemoryDiagnoser]
    public class ImmutableDictionaryAggregationBenchmarks
    {
        private readonly IEnumerable<IImmutableDictionary<int, IComparable>> _dics;

        /*

        |             Method |     Mean |     Error |    StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
        |------------------- |---------:|----------:|----------:|-------:|------:|------:|----------:|
        |          Aggregate | 2.021 us | 0.0458 us | 0.0528 us | 0.0801 |     - |     - |     481 B |
        |      NestedForeach | 2.892 us | 0.0565 us | 0.0735 us | 0.0648 |     - |     - |     385 B |
        | ForEachAndAddRange | 3.691 us | 0.0535 us | 0.0500 us | 0.1183 |     - |     - |     690 B |

        |              Method |     Mean |     Error |    StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
        |-------------------- |---------:|----------:|----------:|-------:|------:|------:|----------:|
        |           Aggregate | 2.196 us | 0.0502 us | 0.0635 us | 0.0801 |     - |     - |     481 B |
        |       NestedForeach | 3.534 us | 0.0383 us | 0.0320 us | 0.1183 |     - |     - |     690 B |
        |  ForEachAndAddRange | 3.452 us | 0.0530 us | 0.0495 us | 0.1183 |     - |     - |     690 B |
        | VersionMapAggregate | 2.528 us | 0.0278 us | 0.0232 us | 0.1411 |     - |     - |     826 B |        

        */

        public ImmutableDictionaryAggregationBenchmarks()
        {
            _dics = ImmutableArray.Create(
                ImmutableDictionary<int, IComparable>.Empty.Add(1, 1).Add(2, 2),
                ImmutableDictionary<int, IComparable>.Empty.Add(3, 3).Add(4, 4));
        }

        [Benchmark]
        public IImmutableDictionary<int, IComparable> Aggregate()
        {
            return _dics.Aggregate((acc, val) => acc.SetItems(val));
        }

        [Benchmark]
        public IImmutableDictionary<int, IComparable> NestedForeach()
        {
            var builder = ImmutableDictionary.CreateBuilder<int, IComparable>();

            foreach (var dic in _dics)
            {
                foreach (var (key, value) in dic)
                {
                    builder[key] = value;
                }
            }

            return builder.ToImmutable();
        }

        [Benchmark]
        public IImmutableDictionary<int, IComparable> ForEachAndAddRange()
        {
            var builder = ImmutableDictionary.CreateBuilder<int, IComparable>();

            foreach (var dic in _dics)
            {
                builder.AddRange(dic); // THIS DISALLOWS DUPLICATES?
            }

            return builder.ToImmutable();
        }

        [Benchmark]
        public IImmutableDictionary<int, IComparable> VersionMapAggregate()
        {
            return VersionMap.Aggregate(_dics);
        }
    }

    internal class VersionMap : IImmutableDictionary<int, IComparable>
    {
        // TODO comparers
        
        public static VersionMap Aggregate(IEnumerable<IImmutableDictionary<int, IComparable>> values)
        {
            var dic = new IComparable[17];
            
            foreach (var value in values)
            {
                foreach (var pair in value)
                {
                    dic[pair.Key] = pair.Value;
                }
            }
            
            return new VersionMap(dic);
        }

        private readonly IComparable[] _dic;

        private VersionMap(IComparable[] dic)
        {
            _dic = dic;
        }

        public VersionMap()
        {
            _dic = new IComparable[17];
        }

        public IEnumerator<KeyValuePair<int, IComparable>> GetEnumerator()
        {
            for (int i = 0; i < _dic.Length; i++)
            {
                IComparable value = _dic[i];
                if (value != null)
                    yield return new KeyValuePair<int, IComparable>(i, value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int Count => _dic.Length;
        
        public bool ContainsKey(int key) => _dic[key] != null;

        public bool TryGetValue(int key, out IComparable value)
        {
            value = _dic[key];
            return value != null;
        }

        public IComparable this[int key] => _dic[key];

        public IEnumerable<int> Keys
        {
            get
            {
                for (int i = 0; i < _dic.Length; i++)
                {
                    IComparable value = _dic[i];
                    if (value != null)
                        yield return i;
                }
            }
        }

        public IEnumerable<IComparable> Values
        {
            get
            {
                for (int i = 0; i < _dic.Length; i++)
                {
                    IComparable value = _dic[i];
                    if (value != null)
                        yield return value;
                }
            }
        }

        public IImmutableDictionary<int, IComparable> Clear() => new VersionMap();

        public IImmutableDictionary<int, IComparable> Add(int key, IComparable value)
        {
            throw new NotImplementedException();
        }

        public IImmutableDictionary<int, IComparable> AddRange(IEnumerable<KeyValuePair<int, IComparable>> pairs)
        {
            throw new NotImplementedException();
        }

        public IImmutableDictionary<int, IComparable> SetItem(int key, IComparable value)
        {
            throw new NotImplementedException();
        }

        public IImmutableDictionary<int, IComparable> SetItems(IEnumerable<KeyValuePair<int, IComparable>> items)
        {
            throw new NotImplementedException();
        }

        public IImmutableDictionary<int, IComparable> RemoveRange(IEnumerable<int> keys)
        {
            throw new NotImplementedException();
        }

        public IImmutableDictionary<int, IComparable> Remove(int key)
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<int, IComparable> pair)
        {
            var value = _dic[pair.Key];
            return Equals(value, pair.Value);
        }

        public bool TryGetKey(int equalKey, out int actualKey)
        {
            if (_dic[equalKey] != null)
            {
                actualKey = equalKey;
                return true;
            }

            actualKey = default;
            return false;
        }
    }
}