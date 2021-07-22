using Narumikazuchi.Collections.Abstract;
using System;

namespace Narumikazuchi.Collections.Trees
{
    /// <summary>
    /// Contains the <see cref="ITreeNode{TNode}"/> objects of any given <see cref="ITree{TNode}"/> as a colleciton of child-nodes.
    /// </summary>
    public class NodeCollection<T> : ReadOnlyRegister<T> where T : ITreeNode<T>
    {
        #region Constructor

        internal NodeCollection(EqualityComparison<T> comparison) : base(comparison) => this._items = new T[1];

        #endregion

        #region Collection Management

        internal void Add(in T item) => this.AddInternal(item);

        internal void Insert(in Int32 index, in T item) => this.InsertInternal(index, item);

        internal Boolean Remove(in T item) => this.RemoveInternal(item);

        internal void Clear()
        {
            lock (this._syncRoot)
            {
                Array.Clear(this._items, 0, this._size);
                this._size = 0;
            }
        }

        #endregion
    }
}
