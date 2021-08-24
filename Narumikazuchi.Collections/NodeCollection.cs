using System;

namespace Narumikazuchi.Collections
{
    /// <summary>
    /// Contains the <see cref="ITreeNode{TNode, TConent}"/> objects of any given <see cref="ITree{TNode, TContent}"/> as a colleciton of child-nodes.
    /// </summary>
    public sealed class NodeCollection<TNode, TContent> : ReadOnlyRegister<TNode> where TNode : ITreeNode<TNode, TContent>
    {
        #region Constructor

        internal NodeCollection(EqualityComparison<TNode> comparison) : base(comparison) => this._items = new TNode[1];

        #endregion

        #region Collection Management

        internal void Add(in TNode item) => this.AddInternal(item);

        internal void Insert(in Int32 index, in TNode item) => this.InsertInternal(index, item);

        internal Boolean Remove(in TNode item) => this.RemoveInternal(item);

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
