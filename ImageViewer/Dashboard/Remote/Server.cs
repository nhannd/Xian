namespace ClearCanvas.Workstation.Dashboard.Remote
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Text;
    using ClearCanvas.Dicom;
    using ClearCanvas.Dicom.Network;

    [SerializableAttribute]
    public class Server : ApplicationEntity
    {
        public Server(String name, String description, String hostname, String aeTitle, Int32 listenPort)
            : base(new HostName(hostname), new AETitle(aeTitle), new ListeningPort(listenPort))
        {
            _name = name;
            _description = description;
        }

        public String Name
        {
            get { return _name; }
        }

        public String Description
        {
            get { return _description; }
        }

        public new String Host
        {
            get { return base.Host; }
        }

        public new String AE
        {
            get { return base.AE; }
        }

        public Int32 ListeningPort
        {
            get { return base.Port; }
        }

        public override string ToString()
        {
            StringBuilder me = new StringBuilder();
            me.AppendFormat("{0} ({1}) - {2}", Name, Description, base.ToString());
            return me.ToString();
        }

        private String _name;
        private String _description;
    }

    /// <summary>
    /// A read-only encapsulation of a List&lt;ApplicationEntity&gt;
    /// </summary>
    public class ReadOnlyServerCollection : ReadOnlyCollection<Server>
    {
        public ReadOnlyServerCollection(List<Server> serverList)
            : base(serverList)
        {
        }
    }
}

