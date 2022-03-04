namespace Narumikazuchi.Collections;

/// <summary>
/// Contains the <see cref="ITreeNode{TNode, TConent}"/> objects of any given <see cref="ITree{TNode, TContent}"/> as a colleciton of child-nodes.
/// </summary>
public sealed class NodeCollection<TNode, TContent> : ReadOnlySetBase<Int32, TNode> 
    where TNode : ITreeNode<TNode, TContent>
{
    internal NodeCollection(EqualityComparison<TNode> equality!!,
                            Comparison<TNode?> comparison!!) : 
        base()
    {
        this.Comparer = new __FuncEqualityComparer<TNode>(comparison: equality);
        m_Comparer = new __FuncComparer<TNode?>(comparison: comparison);
    }

    internal void Add(in TNode item)
    {
        this.AppendInternal(item);
        this.Sort(lowBound: 0, 
                  highBound: this.Count);
    }

    internal void Insert(in Int32 index, 
                         in TNode item)
    {
        this.InsertInternal(index: index,
                            item: item);
        this.Sort(lowBound: 0,
                  highBound: this.Count);
    }

    internal Boolean Remove(in TNode item) => 
        this.RemoveInternal(item);

    internal void Clear() => 
        this.ClearInternal();

    private void Sort(Int32 lowBound,
                      Int32 highBound)
    {
        if (lowBound >= highBound)
        {
            return;
        }

        Int32 split = this.SortPivot(lowBound: lowBound, 
                                     highBound: highBound);
        this.Sort(lowBound : lowBound, 
                  highBound: split - 1);
        this.Sort(lowBound: split + 1, 
                  highBound: highBound);
    }

    private Int32 SortPivot(Int32 lowBound,
                            Int32 highBound)
    {
        Int32 left = lowBound + 1;
        Int32 right = highBound;
        TNode? pivot = this[lowBound];
        TNode? temp;

        while (left <= right)
        {
            while (left <= right &&
                   m_Comparer.Compare(x: this[left], 
                                      y: pivot) <= 0)
            {
                ++left;
            }
            while (left <= right &&
                   m_Comparer.Compare(x: this[right],
                                      y: pivot) > 0)
            {
                --right;
            }

            if (left < right)
            {
                temp = this[left];
                this.InsertInternal(index: left,
                                    item: this[right]);
                this.InsertInternal(index: right,
                                    item: temp);
                ++left;
                --right;
            }
        }

        this.InsertInternal(index: lowBound,
                            item: this[right]);
        this.InsertInternal(index: right,
                            item: pivot);
        return right;
    }

    internal IComparer<TNode?> m_Comparer;
}