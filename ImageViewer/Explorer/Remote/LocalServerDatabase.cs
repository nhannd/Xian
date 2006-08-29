namespace ClearCanvas.ImageViewer.Dashboard.Remote
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using ClearCanvas.Dicom;
    using ClearCanvas.Dicom.Network;                                                                  

    public class LocalServerDatabase : ServerPool
    {
        public LocalServerDatabase()
        {

        }

        ~LocalServerDatabase()
        {

        }

        public override ReadOnlyServerCollection ServerList
        {
            get 
            {
                if (!IsDatabaseLoaded)
                {
                    LoadDatabase();
                    IsDatabaseLoaded = true;
                }

                return new ReadOnlyServerCollection(_serverList); 
            }
        }

        public override void SaveNewDatabase(List<Server> serverList)
        {
            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(_databaseFilename, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                formatter.Serialize(stream, serverList);
                stream.Close();
            }

            // reload the database and have our reference point to a new object so
            // that our copy of the server list is not dependent on a writable 
            // list that the client has a reference to
            LoadDatabase();
        }

        protected override void LoadDatabase()
        {

            if (!File.Exists(_databaseFilename))
                GenerateDefaultDatabase();

            ReadDatabaseIntoMemory();
        }

        protected Boolean IsDatabaseLoaded
        {
            get { return _isDatabaseLoaded; }
            set { _isDatabaseLoaded = value; }
        }

        private void GenerateDefaultDatabase()
        {
            List<Server> list = new List<Server>();

            list.Add(new Server("Default", "Default Server #1",
                "192.168.0.100", "AETITLE1", 1234));

            list.Add(new Server("Default B", "Default Server #2",
                "10.0.0.100", "AETITLE2", 5678));

            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(_databaseFilename, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                formatter.Serialize(stream, list);
                stream.Close();
            }
        }

        private void ReadDatabaseIntoMemory()
        {
            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(_databaseFilename, FileMode.Open))
            {
                _serverList = (List<Server>)formatter.Deserialize(stream);
                stream.Close();
            }
        }

        private readonly String _databaseFilename = "ServerDatabase.bin";
        private List<Server> _serverList;
        private Boolean _isDatabaseLoaded = false;
    }
}
