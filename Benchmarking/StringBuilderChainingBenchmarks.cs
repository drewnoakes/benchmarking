using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmarking
{
    [SimpleJob(RuntimeMoniker.Net48)]
    [MemoryDiagnoser]
    public class StringBuilderChainingBenchmarks
    {
        [Benchmark]
        public StringBuilder Chained()
        {
            return new StringBuilder()
                .Append("A")
                .Append("B")
                .Append("C")
                .Append("D");
        }

        [Benchmark]
        public StringBuilder Unchained()
        {
            var sb = new StringBuilder();
            sb.Append("A");
            sb.Append("B");
            sb.Append("C");
            sb.Append("D");
            return sb;
        }
    }
}