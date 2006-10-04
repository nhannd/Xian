using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using Iesi.Collections;

namespace ClearCanvas.Dicom.DataStore
{
    public class DicomDictionary : IDicomDictionary
    {
        #region Handcoded Members

        public DicomDictionary(ISession session)
        {
            _session = session;
            _pathToColumnDictionary = new Dictionary<string, DictionaryEntry>();
            _tagNameToColumnDictionary = new Dictionary<string, DictionaryEntry>();

            // load the dictionary entries from the database
            Load();

            _session.Clear();
            _session.Close();
        }

        #region Private Members
        private DicomDictionary()
        {

        }

        private void Load()
        {

            IList containers = null;
            try
            {
                containers = this.Session.CreateCriteria(typeof(DicomDictionaryContainer))
                    .SetFetchMode("DictionaryEntries_", FetchMode.Eager)
                    .List();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (null == containers || containers.Count < 1)
            {
                throw new Exception("DicomDictionary could not be loaded");
            }

            DicomDictionaryContainer dictionaryContainer = containers[0] as DicomDictionaryContainer;

            for (int i = 0; i < dictionaryContainer.DictionaryEntries.Count; ++i)
            {
                DictionaryEntry entry = dictionaryContainer.DictionaryEntries[i] as DictionaryEntry;
                _tagNameToColumnDictionary.Add(entry.TagName, entry);
                _pathToColumnDictionary.Add(entry.Path, entry);
            }
        }

        private ISession _session;
        private ISession Session
        {
            get { return _session; }
        }
        private Dictionary<string, DictionaryEntry> _tagNameToColumnDictionary = null;
        private Dictionary<string, DictionaryEntry> _pathToColumnDictionary = null;
        #endregion
        #endregion

        #region IDicomDictionary Members

        public bool Contains(TagName tagName)
        {
            return _tagNameToColumnDictionary.ContainsKey(tagName);
        }

        public bool Contains(Path path)
        {
            return _pathToColumnDictionary.ContainsKey(path);
        }

        public DictionaryEntry GetColumn(TagName tagName)
        {
            if (_tagNameToColumnDictionary.ContainsKey(tagName))
                return _tagNameToColumnDictionary[tagName];
            else
                throw new Exception("Fix this!");
        }

        public DictionaryEntry GetColumn(Path path)
        {
            if (_pathToColumnDictionary.ContainsKey(path))
                return _pathToColumnDictionary[path];
            else
                throw new Exception("Fix this!");
        }

        #endregion
    }
}
