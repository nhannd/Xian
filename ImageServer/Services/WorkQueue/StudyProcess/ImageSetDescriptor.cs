using System;
using System.Collections.Generic;
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

        

        [XmlIgnore]
        public DicomTag DicomTag
        {
            get { return _tag; }
            set { _tag = value; }
        }

        [XmlAttribute("Tag")]
        public string Tag
        {
            get { return _tag.HexString; }
            set { _tag = DicomTagDictionary.GetDicomTag(uint.Parse(value, NumberStyles.HexNumber)); }
        }

        [XmlText]
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
                _value = null;
            else
            {
                _value = attr.ToString();
            }
        }

        #region IEquatable<ImageSetField> Members

        public bool Equals(ImageSetField other)
        {
            return _tag.Equals(other.Tag) && _value.Equals(other.Value);
        }

        #endregion
    }

    [Serializable]
    [XmlRoot("ImageSetDescriptor")]
    public class ImageSetDescriptor : IEquatable<ImageSetDescriptor>
    {

        private Dictionary<DicomTag, ImageSetField> _fields = new Dictionary<DicomTag, ImageSetField>();

        [XmlElement("Field")]
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
                if (other[field.DicomTag] == null || field.Value != other[field.DicomTag].Value)
                    return false;
            }

            return true;
        }

        #endregion
    }
}
