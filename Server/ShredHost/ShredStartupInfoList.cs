using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Server.ShredHost
{
    internal class ShredStartupInfoList : MarshallableList<ShredStartupInfo>
    {
        public ShredStartupInfoList()
        {

        }

        public ReadOnlyCollection<ShredStartupInfo> AllShredStartupInfo
        {
            get { return this.ContainedObjects; }
        }
    }
}
