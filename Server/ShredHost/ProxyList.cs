using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Server.ShredHost
{
    public class ProxyList : MarshallableList<IShredCommunication>
    {
        public ProxyList()
        {
        }

        public ReadOnlyCollection<IShredCommunication> Proxies
        {
            get { return this.ContainedObjects; }
        }
    }
}
