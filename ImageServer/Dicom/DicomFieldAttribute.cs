using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageServer.Dicom
{
    public enum DicomFieldDefault
    {
        None,
        Null,
        Default,
        MinValue,
        MaxValue,
        DateTimeNow,
        StringEmpty,
        DBNull,
        EmptyArray
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class DicomFieldAttribute : Attribute
    {
        private DicomTag _tag;
        private DicomFieldDefault _default;
        private bool _defltOnZL;
        private bool _createEmpty;

        public DicomFieldAttribute(uint tag)
        {
            _tag = DicomTagDictionary.Instance[tag];
            if (_tag == null)
                _tag = new DicomTag(tag, "Unknown Tag", DicomVr.UNvr, false, 1, uint.MaxValue, false);

            _default = DicomFieldDefault.None;
            _defltOnZL = false;
            _createEmpty = false;
        }

        public DicomTag Tag
        {
            get { return _tag; }
        }

        public DicomFieldDefault DefaultValue
        {
            get { return _default; }
            set { _default = value; }
        }

        public bool UseDefaultForZeroLength
        {
            get { return _defltOnZL; }
            set { _defltOnZL = value; }
        }

        public bool CreateEmptyElement
        {
            get { return _createEmpty; }
            set { _createEmpty = value; }
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DicomClassAttribute : Attribute
    {
        private bool _defltOnZL;
        private bool _createEmpty;

        public DicomClassAttribute()
        {
        }

        public bool UseDefaultForZeroLength
        {
            get { return _defltOnZL; }
            set { _defltOnZL = value; }
        }

        public bool CreateEmptyElement
        {
            get { return _createEmpty; }
            set { _createEmpty = value; }
        }
    }
}
