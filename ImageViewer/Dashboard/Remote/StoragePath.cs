
namespace ClearCanvas.Workstation.Dashboard.Remote
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.IO;

    public class StoragePath
    {
        public StoragePath()
        {
            Load();
        }

        public static implicit operator String(StoragePath sp)
        {
            return sp._storagePath;
        }

        public override string ToString()
        {
            return _storagePath;
        }

        public void Set(String storagePath)
        {
            _storagePath = storagePath;
        }

        public void Save()
        {
            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(_persistentFilename, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                formatter.Serialize(stream, _storagePath);
                stream.Close();
            }
        }

        public void Load()
        {
            if (!File.Exists(_persistentFilename))
            {
                _storagePath = @"";
                Save();
            }

            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(_persistentFilename, FileMode.Open))
            {
                _storagePath = (String)formatter.Deserialize(stream);
                stream.Close();
            }
        }

        private String _storagePath;
        private String _persistentFilename = @"StoragePath.bin";
    }
}
