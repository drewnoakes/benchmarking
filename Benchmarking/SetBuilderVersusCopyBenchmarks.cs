using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmarking
{
    [SimpleJob(RuntimeMoniker.Net48)]
    [MemoryDiagnoser]
    public class SetBuilderVersusCopyBenchmarks
    {
        /*
        BenchmarkDotNet=v0.11.1, OS=Windows 10.0.17134.345 (1803/April2018Update/Redstone4)
        Intel Xeon Silver 4110 CPU 2.10GHz, 2 CPU, 32 logical and 16 physical cores
        Frequency=2045972 Hz, Resolution=488.7652 ns, Timer=TSC
          [Host] : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.3190.0
          Clr    : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.3190.0

        Job=Clr  Runtime=Clr

          Method |      Mean |     Error |    StdDev |  Gen 0 | Allocated |
        -------- |----------:|----------:|----------:|-------:|----------:|
          Update | 124.80 us | 1.2306 us | 1.0909 us | 0.4883 |    3216 B |
         Rebuild |  91.32 us | 0.6944 us | 0.6156 us | 0.1221 |     757 B |
         */

        // We have D dependencies, T of which are top-level
        // Want to understand if it's worth having rule handlers manage both the D and T collections,
        // (which can get out of sync in a variety of ways) or whether the rule handlers should just
        // update D, and then we generate T from D for each snapshot.

        private const int DCount = TEvery * 50;
        private const int TEvery = 8;
        private const int TCount = DCount / TEvery;

        private static readonly ImmutableDictionary<string, int> s_dic = Enumerable.Range(0, DCount).ToImmutableDictionary(i => i.ToString());
        private static readonly ImmutableHashSet<string> s_initialSet = Enumerable.Range(0, TCount).Select(i => (i * TEvery).ToString()).ToImmutableHashSet();

        [Benchmark]
        public ImmutableHashSet<string> Update()
        {
            var builder = s_initialSet.ToBuilder();

            foreach (var (key, value) in s_dic)
            {
                if (value % TEvery == 0)
                {
                    builder.Remove(key);
                    builder.Add(key);
                }
            }

            return builder.ToImmutable();
        }

        [Benchmark]
        public ImmutableArray<string> Rebuild()
        {
            var builder = ImmutableArray.CreateBuilder<string>();

            foreach (var (key, value) in s_dic)
            {
                if (value % TEvery == 0)
                    builder.Add(key);
            }

            return builder.ToImmutable();
        }
    }

    // TODO benchmark whether Deconstruct for KVP is faster with 'ref' or 'in' modifier

    internal static class DeconstructionExtensions
    {
        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> pair, out TKey key, out TValue value)
        {
            key = pair.Key;
            value = pair.Value;
        }
    }
}