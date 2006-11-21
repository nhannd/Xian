using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Trees
{
    public interface ITreeNode
    {
        string NodeText { get; }
        ITreeNodeCollection ChildNodes { get; }
    }
}
