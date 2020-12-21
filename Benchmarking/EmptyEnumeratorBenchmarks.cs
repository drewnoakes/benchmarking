using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmarking
{
    [SimpleJob(RuntimeMoniker.Net48)]
    [MemoryDiagnoser]
    public class EmptyEnumeratorBenchmarks
    {
        [Benchmark] public bool EnumerableEmpty() => Enumerable.Empty<string>().GetEnumerator().MoveNext();

        [Benchmark] public bool Custom() => EmptyEnumerator<string>.Instance.MoveNext();

        private sealed class EmptyEnumerator<T>
        {
            public static readonly IEnumerator<T> Instance = new Enumerator();

            private sealed class Enumerator : IEnumerator<T>
            {
                public void Dispose()
                {
                }

                public bool MoveNext()
                {
                    return false;
                }

                public void Reset()
                {
                }

                public T Current => throw new InvalidOperationException();

                object IEnumerator.Current => Current;
            }
        }

/*
        private abstract class LazyRedChildren : IEnumerable<string>
        {
            /// <summary>
            /// The number of children when we switch to use a two levels array, instead of a flat array.
            /// The main reason is that the size of the array can grow bigger for a large folder to the point that it can get into LOH,
            /// which leads into more expensive GCs. We also lazily create the second level array, so it uses less memory space in most cases.
            /// </summary>
            protected const int SmallDirectoryThreshold = 512;

            /// <summary>
            /// project tree that owns the children
            /// </summary>
            protected readonly ProjectTree owner;

            /// <summary>
            /// Initializes a new instance of the <see cref="LazyRedChildren"/> class.
            /// </summary>
            protected LazyRedChildren(ProjectTree owner)
            {
                this.owner = owner;
            }

            /// <summary>
            /// number of nodes in the list
            /// </summary>
            public virtual int Count
            {
                get { return this.owner.greenNode.Children.Count; }
            }

            /// <summary>
            /// return red node for the given index in the list
            /// setter not supported
            /// </summary>
            public abstract string this[int index]
            {
                get;
            }

            public static LazyRedChildren CreateInstance(ProjectTree owner)
            {
                Requires.NotNull(owner, nameof(owner));

                if (owner.greenNode.Children.Count > SmallDirectoryThreshold)
                {
                    return new LazyRedChildrenLarge(owner);
                }

                return new LazyRedChildrenSmall(owner);
            }

            /// <summary>
            /// red node enumerator
            /// </summary>
            public IEnumerator<string> GetEnumerator()
            {
                return new Enumerator(this);
            }

            /// <summary>
            /// explicit implementation of <see cref="GetEnumerator"/>
            /// </summary>
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            public virtual int IndexOf(string child)
            {
                Requires.NotNull(child, nameof(child));
                if (child.Parent == this.owner)
                {
                    return this.owner.GreenNode.Children.IndexOf((child as ProjectTree)?.GreenNode);
                }

                return -1;
            }

            public bool Contains(string child)
            {
                return this.IndexOf(child) >= 0;
            }

            /// <summary>
            /// simple implementation of red node enumerator
            /// </summary>
            private class Enumerator : IEnumerator<string>
            {
                /// <summary>
                /// owner of this enumerator
                /// </summary>
                private readonly LazyRedChildren owner;

                /// <summary>
                /// current index
                /// </summary>
                private int index;

                /// <summary>
                /// Initializes a new instance of the <see cref="Enumerator"/> class.
                /// </summary>
                public Enumerator(LazyRedChildren owner)
                {
                    this.owner = owner;
                    this.index = -1;
                }

                public string Current
                {
                    get
                    {
                        if (this.index < 0 || this.owner.Count <= this.index)
                        {
                            throw new InvalidOperationException("Current");
                        }

                        return this.owner[this.index];
                    }
                }

                object System.Collections.IEnumerator.Current
                {
                    get { return this.Current; }
                }

                public void Dispose()
                {
                }

                public bool MoveNext()
                {
                    this.index++;

                    return this.index < this.owner.Count;
                }

                public void Reset()
                {
                    this.index = -1;
                }
            }
        }

        private class LazyRedChildrenSmall : LazyRedChildren
        {
            private readonly string[] children;

            public LazyRedChildrenSmall(ProjectTree owner)
                : base(Requires.NotNull(owner, nameof(owner)))
            {
                var greenNode = owner.greenNode;
                this.children = new string[greenNode.Children.Count];
            }

            public override int Count => this.children.Length;

            public override string this[int index]
            {
                get
                {
                    if (this.children[index] == null)
                    {
                        Interlocked.CompareExchange(ref this.children[index], Wrap(this.owner.greenNode.Children[index], this.owner), null);
                    }

                    return this.children[index];
                }
            }

            public override int IndexOf(string child)
            {
                Requires.NotNull(child, nameof(child));
                if (child.Parent == this.owner)
                {
                    // Instead of using binary search, we uses linear search when the collection is small.
                    // The reason is that we only need compare reference in the array, instead of doing string comparsion in a binary search.
                    // The binary search does scale better, when the size of children is very big.
                    return this.children.IndexOf(child);
                }

                return -1;
            }
        }

        /// <summary>
        /// When a directory contains large amount of files, the children array can be very big and gets into LOH.
        /// We should prevent this by dividing it to small buckets.
        /// </summary>
        private class LazyRedChildrenLarge : LazyRedChildren
        {
            /// <summary>
            /// storage for red node children
            /// </summary>
            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            private readonly string[][] children;

            public LazyRedChildrenLarge(ProjectTree owner)
                : base(Requires.NotNull(owner, nameof(owner)))
            {
                var greenNode = owner.greenNode;
                int count = greenNode.Children.Count;
                int bucketCount = (count / SmallDirectoryThreshold) + 1;
                this.children = new string[bucketCount][];
            }

            public override string this[int index]
            {
                get
                {
                    Requires.Range(index >= 0 && index < this.owner.greenNode.Children.Count, nameof(index));
                    int bucketIndex = index / SmallDirectoryThreshold;
                    int indexInBucket = index % SmallDirectoryThreshold;

                    if (this.children[bucketIndex] == null)
                    {
                        int bucketSize = SmallDirectoryThreshold;
                        if (bucketIndex == this.children.Length - 1)
                        {
                            bucketSize = this.owner.greenNode.Children.Count % SmallDirectoryThreshold;
                        }

                        Interlocked.CompareExchange(
                            ref this.children[bucketIndex],
                            new string[bucketSize],
                            null);
                    }

                    string[] bucket = this.children[bucketIndex];
                    if (bucket[indexInBucket] == null)
                    {
                        Interlocked.CompareExchange(ref bucket[indexInBucket], Wrap(this.owner.greenNode.Children[index], this.owner), null);
                    }

                    return bucket[indexInBucket];
                }
            }
        }
    */
    }
}