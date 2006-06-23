namespace ClearCanvas.Workstation.Dashboard.Remote
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Text;
    using ClearCanvas.Dicom;
    using ClearCanvas.Dicom.Network;

    abstract public class ServerPool
    {
        public ServerPool()
        {
        }

        ~ServerPool()
        {

        }

        abstract public ReadOnlyServerCollection ServerList
        {
            get;
        }

        abstract protected void LoadDatabase();
        abstract public void SaveNewDatabase(List<Server> serverList);
    }
}
