using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Server.ShredHost
{
    public class ShredList : MarshallableList<IShred>
    {
        public ShredList()
        {

        }

        public ReadOnlyCollection<IShred> Shreds
        {
            get { return this.ContainedObjects; }
        }
    }
}
