using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom.DataStore
{
    public class DictionaryEntry
    {
        #region Handcoded Members
        public DictionaryEntry()
        {
        }

        public virtual TagName TagName
        {
            get { return _tagName; }
            set { _tagName = value; }
        }

        public virtual Path Path
        {
            get { return _path; }
            set { _path = value; }
        }

        public virtual bool IsComputed
        {
            get { return _isComputed; }
            set { _isComputed = value; }
        }

		public virtual string ValueRepresentation
		{
			get { return _valueRepresentation; }
			set { _valueRepresentation = value; }
		}

        private TagName _tagName;
        private Path _path;
        private bool _isComputed;
		private string _valueRepresentation;
        #endregion
    }
}
