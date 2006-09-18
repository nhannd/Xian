using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

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
