using System.Xml;
using System.Xml.Linq;

using BenchmarkDotNet.Attributes;

namespace Benchmarking
{
    [ClrJob]
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