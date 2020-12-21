using System.Globalization;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmarking
{
    [SimpleJob(RuntimeMoniker.Net48)]
    public class PopCountBenchmarks
    {
        private ulong num;

        [Params("FFFFFFFFFFFFFFFF", "0", "1", "8000000000000000", "1111111111111111", "89F74B293D407C93")]
        public string Hex;

        [GlobalSetup]
        public void Setup()
        {
            num = ulong.Parse(Hex, NumberStyles.AllowHexSpecifier);
        }

        [Benchmark]
        public int Iterative()
        {
            ulong v = num;
            int count = 0;

            while (v != 0)
            {
                v &= v - 1;
                count++;
            }

            return count;
        }

        [Benchmark]
        public int Branchless()
        {
            ulong value = num;
            value -= ((value >> 1) & 0x5555555555555555UL);
            value = (value & 0x3333333333333333UL) +
                    ((value >> 2) & 0x3333333333333333UL);
            return (int)(unchecked(((value + (value >> 4)) & 0xF0F0F0F0F0F0F0FUL) * 0x101010101010101UL) >> 56);
        }
    }
}