using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Benchmarking
{
    [ClrJob]
    [RankColumn]
    [MemoryDiagnoser]
    public class LazyDiffBenchmark
    {
        /*
                      Method |       Mean |     Error |    StdDev | Rank |  Gen 0 | Allocated |
            ---------------- |-----------:|----------:|----------:|-----:|-------:|----------:|
             LazyDiffCompute |   554.5 ns |  8.256 ns |  6.446 ns |    1 | 0.0639 |     336 B |
                      Except | 1,328.4 ns | 25.740 ns | 24.077 ns |    2 | 0.1354 |     716 B |
         */

        private IEnumerable<int> Before()
        {
            yield return 1;
            yield return 2;
            yield return 3;
            yield return 4;
            yield return 5;
        }

        private IEnumerable<int> After()
        {
            yield return 2;
            yield return 4;
            yield return 6;
            yield return 8;
        }

        [Benchmark]
        public int LazyDiffCompute()
        {
            var diff = LazyDiff.Compute(Before(), After());

            var dir = 0;

            foreach (var i in diff.Added)
            {
                dir += i;
            }

            foreach (var i in diff.Removed)
            {
                dir -= i;
            }

            return dir;
        }

        [Benchmark]
        public int Except()
        {
            var before = Before().ToList();
            var after = After().ToList();

            var removed = before.Except(after);
            var added = after.Except(before);

            var dir = 0;

            foreach (var i in added)
            {
                dir += i;
            }

            foreach (var i in removed)
            {
                dir -= i;
            }

            return dir;
        }

        internal static class LazyDiff
        {
            private const byte FlagBefore = 0;
            private const byte FlagAfter = 1;

            public static Result<T> Compute<T>(IEnumerable<T> before, IEnumerable<T> after)
            {
                var dic = new Dictionary<T, byte>();

                foreach (T item in before)
                {
                    dic[item] = FlagBefore;
                }

                foreach (T item in after)
                {
                    if (!dic.Remove(item))
                    {
                        dic[item] = FlagAfter;
                    }
                }

                return new Result<T>(dic);
            }

            public readonly struct Result<T>
            {
                private readonly Dictionary<T, byte> _dic;

                public Result(Dictionary<T, byte> dic)
                {
                    _dic = dic;
                }

                public ResultPart<T> Removed => new ResultPart<T>(_dic, FlagBefore);
                public ResultPart<T> Added => new ResultPart<T>(_dic, FlagAfter);
            }

            public readonly struct ResultPart<T>
            {
                private readonly Dictionary<T, byte> _dic;
                private readonly byte _flag;

                public ResultPart(Dictionary<T, byte> dic, byte flag)
                {
                    _dic = dic;
                    _flag = flag;
                }

                public Enumerator GetEnumerator()
                {
                    return new Enumerator(_dic, _flag);
                }

                public struct Enumerator
                {
                    private readonly byte _flag;

                    // IMPORTANT cannot be readonly
                    private Dictionary<T, byte>.Enumerator _enumerator;

                    public Enumerator(Dictionary<T, byte> dic, byte flag)
                    {
                        _flag = flag;
                        _enumerator = dic.GetEnumerator();
                        Current = default;
                    }

                    public bool MoveNext()
                    {
                        while (_enumerator.MoveNext())
                        {
                            if (_enumerator.Current.Value == _flag)
                            {
                                Current = _enumerator.Current.Key;
                                return true;
                            }
                        }

                        return false;
                    }

                    public T Current { get; private set; }
                }
            }
        }
    }
}