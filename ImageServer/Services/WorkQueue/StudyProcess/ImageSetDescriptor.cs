using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common.Utilities;

namespace ClearCanvas.ImageServer.Services.WorkQueue.StudyProcess
{

    public class ImageSetField : IEquatable<ImageSetField>
    {
        private DicomTag _tag;
        private string _value;
        
        public DicomTag DicomTag
        {
            get { return _tag; }
            set
            {
                Debug.Assert(value!=null);
                _tag = value;
            }
        }

        public string Tag
        {
            get { return _tag.HexString; }
            set {
                Debug.Assert(!String.IsNullOrEmpty(value));
                _tag = DicomTagDictionary.GetDicomTag(uint.Parse(value, NumberStyles.HexNumber)); 
            }
        }

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public ImageSetField() { }

        public ImageSetField(DicomAttribute attr)
        {
            _tag = attr.Tag;
            if (attr.IsEmpty || attr.IsNull)
                _value = String.Empty;
            else
            {
                // Make sure the value is Xml compatible.
                _value = XmlUtils.EncodeValue(attr.ToString());
            }
        }

        #region IEquatable<ImageSetField> Members

        public bool Equals(ImageSetField other)
        {
            return DicomTag.Equals(other.DicomTag) && _value.Equals(other.Value);
        }

        #endregion
    }

    [Serializable]
    [XmlRoot("ImageSetDescriptor")]
    public class ImageSetDescriptor : IEquatable<ImageSetDescriptor> , IXmlSerializable
    {

        private Dictionary<DicomTag, ImageSetField> _fields = new Dictionary<DicomTag, ImageSetField>();

        public ImageSetField[] Fields
        {
            get
            {
                ImageSetField[] array = new ImageSetField[_fields.Count];
                _fields.Values.CopyTo(array, 0);
                return array;
            }
            set
            {
                _fields = new Dictionary<DicomTag, ImageSetField>();
                foreach (ImageSetField field in value)
                {
                    _fields.Add(field.DicomTag, field);
                }
            }
        }

        public ImageSetField this[DicomTag tag]
        {
            get
            {
                return _fields[tag];
            }
        }

        public ImageSetField this[uint tag]
        {
            get
            {
                return _fields[DicomTagDictionary.GetDicomTag(tag)];
            }
        }

        protected void AddField(ImageSetField field)
        {
            _fields.Add(field.DicomTag, field);
        }

        static public ImageSetDescriptor Parse(DicomMessageBase message)
        {
            ImageSetDescriptor desc = new ImageSetDescriptor();
            desc.AddField(new ImageSetField(message.DataSet[DicomTags.PatientId]));
            desc.AddField(new ImageSetField(message.DataSet[DicomTags.IssuerOfPatientId]));
            desc.AddField(new ImageSetField(message.DataSet[DicomTags.PatientsName]));
            desc.AddField(new ImageSetField(message.DataSet[DicomTags.PatientsBirthDate]));
            desc.AddField(new ImageSetField(message.DataSet[DicomTags.PatientsSex]));
            desc.AddField(new ImageSetField(message.DataSet[DicomTags.AccessionNumber]));
            desc.AddField(new ImageSetField(message.DataSet[DicomTags.StudyDate]));
            return desc;
        }

        static public ImageSetDescriptor Parse(XmlElement element)
        {
            return XmlUtils.Deserialize<ImageSetDescriptor>(element);
        }


        #region IEquatable<ImageSetDescriptor> Members

        public bool Equals(ImageSetDescriptor other)
        {
            if (this == other)
                return true;

            if (Fields.Length == 0 && other.Fields.Length > 0)
                return false;

            foreach (ImageSetField field in Fields)
            {
                if (other[field.DicomTag] == null || !field.Equals(other[field.DicomTag]))
                    return false;
            }

            return true;
        }

        #endregion

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void ReadXml(XmlReader reader)
        {
            if (reader.ReadToFollowing("Field"))
            {
                do
                {
                    ImageSetField field = new ImageSetField();
                    field.Tag = reader["Tag"];
                    field.Value = String.IsNullOrEmpty(reader["Value"])? String.Empty:reader["Value"];
                    AddField(field);
                } while (reader.ReadToNextSibling("Field"));
            }

        }

        public void WriteXml(XmlWriter writer)
        {
            foreach(ImageSetField field in _fields.Values)
            {
                writer.WriteStartElement("Field");
                writer.WriteAttributeString("Tag", field.Tag);
                writer.WriteAttributeString("Value", field.Value);
                writer.WriteEndElement();
            }
        }

        #endregion
    }
}
