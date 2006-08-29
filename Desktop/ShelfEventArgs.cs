using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    public class ShelfEventArgs : CollectionEventArgs<IShelf>
    {
        public ShelfEventArgs()
        {
        }

        public ShelfEventArgs(IShelf shelf)
        {
            this.Item = shelf;
        }
    }
}
