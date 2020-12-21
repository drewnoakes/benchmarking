using System.Xml;
using System.Xml.Linq;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmarking
{
    /*
        BenchmarkDotNet=v0.11.1, OS=Windows 10.0.18362
        Intel Xeon Silver 4110 CPU 2.10GHz, 2 CPU, 32 logical and 16 physical cores
          [Host] : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.8.3801.0
          Clr    : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.8.3801.0

        Job=Clr  Runtime=Clr

                  Method |     Mean |     Error |   StdDev |   Median |  Gen 0 |  Gen 1 | Allocated |
        ---------------- |---------:|----------:|---------:|---------:|-------:|-------:|----------:|
         XmlDocumentLoad | 210.1 us | 10.640 us | 10.93 us | 204.9 us | 9.7656 | 0.2441 |  51.25 KB |
           XDocumentLoad | 196.7 us |  6.266 us | 18.47 us | 191.5 us | 8.7891 | 0.2441 |  45.87 KB |
     */

    [SimpleJob(RuntimeMoniker.Net48)]
    [MemoryDiagnoser]
    public class XmlDocumentVsXDocumentBenchmark
    {
        const string path = @"D:\repos\spikes\Benchmarking\Benchmarking\Benchmarking.csproj";

        [Benchmark]
        public XmlDocument XmlDocumentLoad()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            return doc;
        }

        [Benchmark]
        public XDocument XDocumentLoad()
        {
            return XDocument.Load(path);
        }
    }
}