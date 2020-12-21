using System.Threading;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmarking
{
    [SimpleJob(RuntimeMoniker.Net48)]
    public class RefLocalArrayAccessBenchmarks
    {
        /*
         With static zero index:

            |     Method |     Mean |    Error |   StdDev |   Median |
            |----------- |---------:|---------:|---------:|---------:|
            | WithoutRef | 11.91 ns | 0.377 ns | 1.113 ns | 12.34 ns |
            |    WithRef | 13.88 ns | 0.395 ns | 0.961 ns | 14.19 ns |
         
         With static one index:

            |     Method |     Mean |    Error |   StdDev |
            |----------- |---------:|---------:|---------:|
            | WithoutRef | 11.50 ns | 0.238 ns | 0.222 ns |
            |    WithRef | 11.81 ns | 0.055 ns | 0.049 ns |
         
         With variable zero index:

            |     Method |     Mean |    Error |   StdDev |
            |----------- |---------:|---------:|---------:|
            | WithoutRef | 12.50 ns | 0.200 ns | 0.188 ns |
            |    WithRef | 11.88 ns | 0.052 ns | 0.049 ns |

         With variable one index:

            |     Method |     Mean |    Error |   StdDev |
            |----------- |---------:|---------:|---------:|
            | WithoutRef | 12.37 ns | 0.336 ns | 0.724 ns |
            |    WithRef | 11.16 ns | 0.145 ns | 0.129 ns |
         */

        private object[] objects = { new object(), new object() };
        private object obj = new object();
//        private int index = 1;

        [Benchmark]
        public object WithoutRef()
        {
            if (objects[1] != null)
            {
                Interlocked.Exchange(ref objects[1], obj);
            }

            return objects[1];
        }

        [Benchmark]
        public object WithRef()
        {
            ref object o = ref objects[1];

            if (o != null)
            {
                Interlocked.Exchange(ref o, obj);
            }

            return o;
        }
    }
}