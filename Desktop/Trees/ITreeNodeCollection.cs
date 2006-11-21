using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Trees
{
    public interface ITreeNodeCollection : System.Collections.IEnumerable
    {
        int Count { get; }
    }
}
