using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NHibernate;

namespace ClearCanvas.Dicom.DataStore
{
    public class DicomDictionaryContainer
    {
		private readonly IList _dictionaryEntries;
		private string _dictionaryName;
		private Guid _entryOid;
		
		public DicomDictionaryContainer()
        {
            _dictionaryEntries = new ArrayList();
        }

        protected virtual Guid EntryOid
        {
            get { return _entryOid; }
            set { _entryOid = value; }
        }

		public virtual string DictionaryName
		{
			get { return _dictionaryName; }
			set { _dictionaryName = value; }
		}

		public virtual IList DictionaryEntries
        {
            get { return _dictionaryEntries; }
        }
    }
}
