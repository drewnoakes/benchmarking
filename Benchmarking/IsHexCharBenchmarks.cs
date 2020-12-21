using System;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmarking
{
    [SimpleJob(RuntimeMoniker.Net48)]
    [MemoryDiagnoser]
    public class IsHexCharBenchmarks
    {
        /*
                       Method |   P |     Mean |     Error |    StdDev | Allocated |
        --------------------- |---- |---------:|----------:|----------:|----------:|
         IsHexCharNaiveBetter |   0 | 7.379 ns | 0.0253 ns | 0.0225 ns |       0 B |
          IsHexCharBranchless |   0 | 2.960 ns | 0.0477 ns | 0.0398 ns |       0 B |
         IsHexCharNaiveBetter | 0.5 | 6.428 ns | 0.0236 ns | 0.0209 ns |       0 B |
          IsHexCharBranchless | 0.5 | 2.917 ns | 0.0123 ns | 0.0102 ns |       0 B |
         IsHexCharNaiveBetter |   1 | 5.220 ns | 0.0124 ns | 0.0110 ns |       0 B |
          IsHexCharBranchless |   1 | 2.889 ns | 0.0127 ns | 0.0119 ns |       0 B |
         */
        private const int StringLength = 10000;

        private readonly char[] characters = new char[StringLength];
        private readonly bool[] results = new bool[StringLength];

        [Params(0, 0.5, 1.0)]
        public double P;

        [GlobalSetup]
        public void Setup()
        {
            string valid = "0123456789abcdefABCDEF";
            string invalid = new string(Enumerable.Range(0, 255).Select(i => (char) i).Where(c => !valid.Contains(c)).ToArray());

            var random = new Random(42);

            for (int i = 0; i < StringLength; i++)
            {
                var source = random.NextDouble() > P ? valid : invalid;

                characters[i] = source[random.Next(source.Length)];
            }

            for (var c = 0; c < 256; c++)
            {
                if (IsHexCharNaive((char) c) != IsHexCharNaiveBetter((char) c))
                    throw new Exception($"Invalid implementation 1: {c} {((char) c)}");
                if (IsHexCharBranchless((char) c) != IsHexCharNaiveBetter((char) c))
                    throw new Exception($"Invalid implementation 2: {c} {((char) c)}");
            }
        }

        [Benchmark(OperationsPerInvoke = StringLength)]
        public void IsHexCharNaiveBetter()
        {
            for (var i = 0; i < characters.Length; i++)
            {
                results[i] = IsHexCharNaiveBetter(characters[i]);
            }
        }

        [Benchmark(OperationsPerInvoke = StringLength)]
        public void IsHexCharBranchless()
        {
            for (var i = 0; i < characters.Length; i++)
            {
                results[i] = IsHexCharBranchless(characters[i]);
            }
        }

        public static bool IsHexCharNaive(char c)
        {
            return (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');
        }

        public static bool IsHexCharNaiveBetter(char c)
        {
            char c2 = (char) (c | 0b_0010_0000); // to lower (no effect on digits)

            return (c >= '0' && c <= '9') || (c2 >= 'a' && c2 <= 'f');
        }

        public static bool IsHexCharBranchless(char c)
        {
            char c2 = (char)(c | 0b_0010_0000); // to lower (no effect on digits)
//            char c2 = (char)0b_0010_0000; // to lower (no effect on digits)

//            int k = 0b0011_0000;

            return (((c & 0b1111_1000) ^ 0b0011_0000)  | // 1,2,3,4,5,6,7
                    ((c & 0b1111_1110) ^ 0b0011_1000)  | // 8,9
                    ((c2 & 0b1101_1000) ^ 0b0011_1000) | // A,B,C,D,E,a,b,c,d,e
                    ((c2 & 0b1101_1100) ^ 0b0011_1100))  // F,f
                   == 0;
        }
    }
}