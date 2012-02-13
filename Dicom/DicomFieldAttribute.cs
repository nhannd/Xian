#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Dicom
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
    	private DicomTag _parentTag;
        private DicomFieldDefault _default;
        private bool _defltOnZL;
        private bool _createEmpty;
		private bool _setNullValueIfEmpty;

        public DicomFieldAttribute(uint tag)
        {
            _tag = DicomTagDictionary.GetDicomTag(tag);
            if (_tag == null)
                _tag = new DicomTag(tag, "Unknown Tag", "UnknownTag", DicomVr.UNvr, false, 1, uint.MaxValue, false);

            _default = DicomFieldDefault.None;
            _defltOnZL = false;
            _createEmpty = false;
        }

		public DicomFieldAttribute(uint tag, uint parentTag)
			: this(tag)
		{
			_parentTag = DicomTagDictionary.GetDicomTag(parentTag);
			if (_parentTag == null)
				_parentTag = new DicomTag(parentTag, "Unknown Tag", "UnknownTag", DicomVr.UNvr, false, 1, uint.MaxValue, false);
		}

        public DicomTag Tag
        {
            get { return _tag; }
        }

    	public DicomTag ParentTag
    	{
    		get { return _parentTag; }
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
		
    	public bool SetNullValueIfEmpty
    	{
			get { return _setNullValueIfEmpty; }
			set { _setNullValueIfEmpty = value; }
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
