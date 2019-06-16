using BenchmarkDotNet.Attributes;

namespace Benchmarking
{
    [ClrJob]
    [MemoryDiagnoser]
    public class StringContainsCharBenchmarks
    {
        /*
                           Method |     Mean |     Error |    StdDev | Allocated |
        ------------------------- |---------:|----------:|----------:|----------:|
         ContainsSingleCharString | 74.99 ns | 0.6283 ns | 0.5246 ns |       0 B |
                      IndexOfChar | 15.49 ns | 0.1420 ns | 0.1259 ns |       0 B |
         */
        [Benchmark]
        public bool ContainsSingleCharString()
        {
            return "abcdefghijklmnopqrstuvwxyz.".Contains(".");
        }

        [Benchmark]
        public bool IndexOfChar()
        {
            return "abcdefghijklmnopqrstuvwxyz.".IndexOf('.') != -1;
        }
    }
}