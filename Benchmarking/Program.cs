using System;
using System.Collections.Immutable;

using BenchmarkDotNet.Running;

namespace Benchmarking
{
    public static class Program
    {
        public static void Main()
        {
//            BenchmarkRunner.Run<Md5VsSha256>();
//            BenchmarkRunner.Run<ImmutableListArrayCreateRangeBenchmarks>();
//            BenchmarkRunner.Run<StringBuilderChainingBenchmarks>();
//            BenchmarkRunner.Run<StringComparisonEqualsBenchmarks>();
//            BenchmarkRunner.Run<ImmutableArrayConstructionBenchmarks>();
//            BenchmarkRunner.Run<ImmutableArrayConstructionBenchmarks2>();
//            BenchmarkRunner.Run<ImmutableArrayConstructionBenchmarks3>();
//            BenchmarkRunner.Run<IncrementBenchmarks>();
//            BenchmarkRunner.Run<StringFormatBenchmarks>();
//            BenchmarkRunner.Run<ImmutableArrayListAddBenchmarks>();
//            BenchmarkRunner.Run<BuildImmutableFromSetBenchmarks>();
//            BenchmarkRunner.Run<AttributeSealingBenchmarks>();
//            BenchmarkRunner.Run<ConcurrentDictionaryBenchmarks>();
//            BenchmarkRunner.Run<StringEqualityBenchmarks>();
//            BenchmarkRunner.Run<StringContainsCharBenchmarks>();
//            BenchmarkRunner.Run<HashSetVsDistinctBenchmarks>();
//            BenchmarkRunner.Run<SetBuilderVersusCopyBenchmarks>();
//            BenchmarkRunner.Run<ExchangeVsCompareExchangeBenchmarks>();
//            BenchmarkRunner.Run<ImmutableDictionaryEnumerationBenchmarks>();
//            BenchmarkRunner.Run<IsHexCharBenchmarks>();
//            BenchmarkRunner.Run<XmlDocumentVsXDocumentBenchmark>();
//            BenchmarkRunner.Run<LockedDictionaryVsImmutableDictionaryBenchmarks>();
//            BenchmarkRunner.Run<StringComparisonEqualsBenchmarks>();
//            BenchmarkRunner.Run<PopCountBenchmarks>();
//            BenchmarkRunner.Run<IteratorVsEnumeratorBenchmarks>();
//            BenchmarkRunner.Run<EmptyEnumeratorBenchmarks>();
//            BenchmarkRunner.Run<ImmutableSortedSetIterationBenchmarks>();
//            BenchmarkRunner.Run<RefLocalArrayAccessBenchmarks>();
//            BenchmarkRunner.Run<ImmutableDictionaryAggregationBenchmarks>();
//            BenchmarkRunner.Run<CharArrayContainsCharBenchmarks>();

//            Console.Out.WriteLine(typeof(ImmutableSortedSet).Assembly);

//            BenchmarkRunner.Run<FilePathHashBenchmarks>();
            BenchmarkRunner.Run<ImmutableDictionaryInterfaceEnumerationBenchmarks>();
//            BenchmarkRunner.Run<ImmutableDictionaryConstructionBenchmarks>();

//            Console.ReadLine();
//            var benchmarks = new IImmutableDictionaryEnumerationBenchmarks();
//            for (int i = 0; i < 100_000; i++) benchmarks.ForEachOptimisticNoAllocate();
//
//            Console.Out.WriteLine("Done");
//            Console.ReadLine();

//            BenchmarkRunner.Run<LazyDiffBenchmark>();

//            for (var c = 0; c < 256; c++)
//            {
//                if (IsHexCharBenchmarks.IsHexCharNaive((char) c) != IsHexCharBenchmarks.IsHexCharNaiveBetter((char) c))
//                    throw new Exception($"Invalid implementation 1: {c} {((char) c)}");
//                if (IsHexCharBenchmarks.IsHexCharBranchless((char) c) != IsHexCharBenchmarks.IsHexCharNaiveBetter((char) c))
//                    throw new Exception($"Invalid implementation 2: {c} {((char) c)}");
//            }

/*            var foo = new IImmutableDictionaryEnumerationBenchmarks();
            
            foo.ForEachAllocate();
            foo.ForEachAllocate();
            foo.ForEachAllocate();
//            foo.ForEachOptimisticNoAllocate();
//            foo.ForEachOptimisticNoAllocate();
//            foo.ForEachOptimisticNoAllocate();
            
            Console.WriteLine("Press enter to enumerate");
            Console.ReadLine();
//            foo.ForEachOptimisticNoAllocate();
            foo.ForEachAllocate();
            Console.ReadLine();*/
        }
    }
}
