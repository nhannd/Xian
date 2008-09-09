using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ClearCanvas.ImageServer.Model
{
    [Serializable]
    public class ReconcileStudyWorkQueueData : IXmlSerializable
    {
        private string _storagePath;

        public string StoragePath
        {
            get { return _storagePath; }
            set { _storagePath = value; }
        }

        #region ISerializable Members


        #endregion

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            if (reader.ReadToDescendant("StoragePath"))
            {
                StoragePath = reader.ReadElementContentAsString();
            }

        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString("StoragePath", StoragePath);
        }

        #endregion
    }
}
