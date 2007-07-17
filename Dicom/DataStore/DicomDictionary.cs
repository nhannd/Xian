using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using Iesi.Collections;
using ClearCanvas.Common;

namespace ClearCanvas.Dicom.DataStore
{
    internal class DicomDictionary : IDicomDictionary
    {
		internal static readonly string DefaultDictionaryName = "default-unicode";
		internal static readonly string DefaultQueryDictionaryName = "study-query-unicode";
		internal static readonly string DefaultResultsDictionaryName = "study-query-results-unicode";

        #region Handcoded Members

		internal DicomDictionary(ISession session)
			: this(session, DefaultDictionaryName)
		{ 
		}

		public DicomDictionary(ISession session, string dictionaryName)
        {
			Platform.CheckForEmptyString(dictionaryName, "dictionaryName");

			_dictionaryName = dictionaryName;
			_pathToColumnDictionary = new Dictionary<string, DictionaryEntry>();
            _tagNameToColumnDictionary = new Dictionary<string, DictionaryEntry>();

            // load the dictionary entries from the database
            Load(session);
        }

		public string DictionaryName
		{
			get { return _dictionaryName; }
		}
	
        #region Private Members

		private DicomDictionary()
        {
        }

        private void Load(ISession session)
        {
            IList containers = null;
			try
			{
				containers = session.CreateCriteria(typeof(DicomDictionaryContainer))
					.SetFetchMode("DictionaryEntries_", FetchMode.Eager)
					.List();
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				session.Clear();
				session.Close();
			}	

            if (null == containers || containers.Count < 1)
            {
				throw new Exception(SR.ExceptionDicomDictionaryFailedToLoad);
            }

			foreach (DicomDictionaryContainer container in containers)
			{
				if (container.DictionaryName == _dictionaryName)
				{
					for (int i = 0; i < container.DictionaryEntries.Count; ++i)
					{
						DictionaryEntry entry = container.DictionaryEntries[i] as DictionaryEntry;
						_tagNameToColumnDictionary.Add(entry.TagName, entry);
						_pathToColumnDictionary.Add(entry.Path, entry);
					}

					return;
				}
			}

			throw new Exception(String.Format(SR.FormatDicomDictionaryFailedToLoad, _dictionaryName));
        }

		private string _dictionaryName;
		private Dictionary<string, DictionaryEntry> _tagNameToColumnDictionary = null;
        private Dictionary<string, DictionaryEntry> _pathToColumnDictionary = null;

		#endregion
        #endregion

        #region IDicomDictionary Members

        public bool Contains(TagName tagName)
        {
            return _tagNameToColumnDictionary.ContainsKey(tagName);
        }

		public bool Contains(DicomTagPath path)
        {
            return _pathToColumnDictionary.ContainsKey(path);
        }

        public DictionaryEntry GetColumn(TagName tagName)
        {
            if (_tagNameToColumnDictionary.ContainsKey(tagName))
                return _tagNameToColumnDictionary[tagName];
            else
                throw new Exception(String.Format(SR.FormatSpecifiedColumnDoesNotExistForTag, tagName.ToString()));
        }

		public DictionaryEntry GetColumn(DicomTagPath path)
        {
            if (_pathToColumnDictionary.ContainsKey(path))
                return _pathToColumnDictionary[path];
            else
				throw new Exception(String.Format(SR.FormatSpecifiedColumnDoesNotExistForPath, path.ToString()));
        }

		#endregion
	}
}
