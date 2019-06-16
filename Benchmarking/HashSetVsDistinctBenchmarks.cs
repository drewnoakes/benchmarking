using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Benchmarking
{
    [ClrJob]
    [MemoryDiagnoser]
    public class HashSetVsDistinctBenchmarks
    {
        /*
         */
        [Benchmark]
        public void HashSet()
        {
            foreach (var o in new HashSet<string>(GetStrings()))
            {
            }
        }

        [Benchmark]
        public void Distinct()
        {
            foreach (var o in GetStrings().Distinct())
            {
                
            }
        }

        private IEnumerable<string> GetStrings()
        {
            yield return "Foo";
            yield return "Bar";
            yield return "Baz";
            yield return "Foo";
            yield return "Baz";
            yield return "Bar";
        }
    }
}