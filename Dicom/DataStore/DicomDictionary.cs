using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using Iesi.Collections;
using ClearCanvas.Common;
using NHibernate.Expression;

namespace ClearCanvas.Dicom.DataStore
{
	public sealed partial class DataAccessLayer
	{
		internal class DicomDictionary : IDicomDictionary
		{
			public static readonly string DefaultDictionaryName = "default-unicode";
			public static readonly string DefaultQueryDictionaryName = "study-query-unicode";
			public static readonly string DefaultResultsDictionaryName = "study-query-results-unicode";

			private readonly string _dictionaryName;
			private readonly Dictionary<string, DictionaryEntry> _tagNameToColumnDictionary;
			private readonly Dictionary<string, DictionaryEntry> _pathToColumnDictionary;

			public DicomDictionary(ISessionManager sessionManager, string dictionaryName)
			{
				try
				{
					Platform.CheckForNullReference(sessionManager, "sessionManager");
					Platform.CheckForEmptyString(dictionaryName, "dictionaryName");

					_dictionaryName = dictionaryName;
					_pathToColumnDictionary = new Dictionary<string, DictionaryEntry>();
					_tagNameToColumnDictionary = new Dictionary<string, DictionaryEntry>();

					LoadInternal(sessionManager);
				}
				finally
				{
					sessionManager.Dispose();
				}
			}

			public string DictionaryName
			{
				get { return _dictionaryName; }
			}

			private void LoadInternal(ISessionManager sessionManager)
			{
				IList containers = null;

				using (sessionManager.GetReadTransaction())
				{
					try
					{
						containers = sessionManager.Session.CreateCriteria(typeof(DicomDictionaryContainer))
							.Add(Expression.Eq("DictionaryName", _dictionaryName))
							.SetFetchMode("DictionaryEntries_", FetchMode.Eager)
							.List();
					}
					catch (Exception e)
					{
						throw new DataStoreException(String.Format(SR.FormatDicomDictionaryFailedToLoad, _dictionaryName));
					}
				}

				if (null == containers || containers.Count < 1)
				{
					throw new DataStoreException(String.Format(SR.FormatDicomDictionaryFailedToLoad, _dictionaryName));
				}

				DicomDictionaryContainer container = (DicomDictionaryContainer) containers[0];

				for (int i = 0; i < container.DictionaryEntries.Count; ++i)
				{
					DictionaryEntry entry = (DictionaryEntry)container.DictionaryEntries[i];
					_tagNameToColumnDictionary[entry.TagName] = entry;
					_pathToColumnDictionary[entry.Path] = entry;
				}
			}

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
}