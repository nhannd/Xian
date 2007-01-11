using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace ClearCanvas.Server.ShredHost
{
    internal class ShredInfoList : MarshallableList<ShredInfo>
    {
        public ShredInfoList()
        {

        }

        public ReadOnlyCollection<ShredInfo> AllShredInfo
        {
            get { return this.ContainedObjects; }
        }

    }
}
