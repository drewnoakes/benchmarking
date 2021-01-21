using System.Globalization;
using System.IO;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmarking
{
    /*
    BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19042
    Intel Xeon Silver 4110 CPU 2.10GHz, 2 CPU, 32 logical and 16 physical cores
      [Host]   : .NET Framework 4.8 (4.8.4250.0), X64 RyuJIT
      .NET 4.8 : .NET Framework 4.8 (4.8.4250.0), X64 RyuJIT

    Job=.NET 4.8  Runtime=.NET 4.8

    |      Method |       Mean |    Error |   StdDev | Ratio |
    |------------ |-----------:|---------:|---------:|------:|
    |    Baseline | 1,294.8 ns |  5.35 ns |  4.46 ns |  1.00 |
    | NewApproach |   567.7 ns | 11.12 ns | 12.36 ns |  0.44 |
     */
    [SimpleJob(RuntimeMoniker.Net48)]
    public class FilePathHashBenchmarks
    {
        private string[] paths =
        {
            @"D:\repos\project-system\src\Microsoft.VisualStudio.ProjectSystem.Managed\ProjectSystem\DesignTimeTargets\Microsoft.Managed.DesignTime.targets",
            @"D:/repos/project-system/src/Microsoft.VisualStudio.ProjectSystem.Managed/ProjectSystem/DesignTimeTargets/Microsoft.Managed.DesignTime.targets",
            @"D:\repos\project-system\src\Microsoft.VisualStudio.ProjectSystem.Managed\ProjectSystem\Rules\CollectedFrameworkReference.xaml",
            @"D:/repos/project-system/src/Microsoft.VisualStudio.ProjectSystem.Managed/ProjectSystem/Rules/CollectedFrameworkReference.xaml",
        };

        private static readonly int SeparatorHashCode = Path.DirectorySeparatorChar.GetHashCode();
        private static readonly TextInfo TextInfo = CultureInfo.InvariantCulture.TextInfo;

        [Benchmark(Baseline = true, OperationsPerInvoke = 4)]
        public int Baseline()
        {
            int i = 0;
            
            foreach (var path in paths)
            {
                i ^= GetHashCode(path);
            }

            return i;

            static int GetHashCode(string obj)
            {
                if (obj.Length == 0)
                {
                    return 0;
                }

                // Implementation from CPS's PathComparerOrdinalIgnoreCase
                unchecked
                {
                    int hash1 = 5381;
                    int hash2 = hash1;
                    for (int i = 0; i < obj.Length; i += 2)
                    {
                        char c = obj[i];
                        int cHash = GetCharHashcode(c);
                        hash1 = ((hash1 << 5) + hash1) ^ cHash;

                        if (i + 1 < obj.Length)
                        {
                            c = obj[i + 1];
                            cHash = GetCharHashcode(c);
                            hash2 = ((hash2 << 5) + hash2) ^ cHash;
                        }
                    }

                    return hash1 + (hash2 * 1566083941);
                }

                static int GetCharHashcode(char c)
                {
                    return IsPathSeparator(c) ? SeparatorHashCode : TextInfo.ToUpper(c).GetHashCode();
                }

                static bool IsPathSeparator(char c)
                {
                    return c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar;
                }
            }
        }

        #region New Approach

        // Use FNV hash, with some case sensitive comparison logic copied from
        // https://github.com/dotnet/roslyn/blob/master/src/Compilers/Core/Portable/CaseInsensitiveComparison.cs

        private static readonly TextInfo s_unicodeCultureTextInfo = CultureInfo.InvariantCulture.TextInfo;

        [Benchmark(OperationsPerInvoke = 4)]
        public int NewApproach()
        {
            int i = 0;
            
            foreach (var path in paths)
            {
                i ^= GetHashCode(path);
            }

            return i;

            static int GetHashCode(string obj)
            {
                if (obj.Length == 0)
                {
                    return 0;
                }

                const int FnvOffsetBias = unchecked((int)2166136261);
                const int FnvPrime = 16777619;

                int hashCode = FnvOffsetBias;

                for (int i = 0; i < obj.Length; i++)
                {
                    hashCode = unchecked((hashCode ^ ToUpper(obj[i])) * FnvPrime);
                }

                return hashCode;

                static char ToUpper(char c)
                {
                    if (unchecked((uint)(c - 'a')) <= 'z' - 'a')
                    {
                        return (char)(c & ~0x20);
                    }

                    if (c == Path.AltDirectorySeparatorChar)
                    {
                        return Path.DirectorySeparatorChar;
                    }

                    if (c < 0xE0)
                    {
                        return c;
                    }

                    return s_unicodeCultureTextInfo.ToLower(c);
                }
            }
        }

        #endregion
    }
}