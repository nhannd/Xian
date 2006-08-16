using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NHibernate;

namespace ClearCanvas.Dicom.DataStore
{
    public class DicomDictionaryContainer
    {
        public DicomDictionaryContainer()
        {
            _dictionaryEntries = new ArrayList();
        }

        protected virtual long EntryOid
        {
            get { return _entryOid; }
            set { _entryOid = value; }
        }

        public virtual IList DictionaryEntries
        {
            get { return _dictionaryEntries; }
        }

        private IList _dictionaryEntries;
        private long _entryOid;

    }
}
