#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using NHibernate;
using NHibernate.Expression;

namespace ClearCanvas.Dicom.DataStore
{
	public sealed partial class DataAccessLayer
	{
		private class DicomDictionary : IDicomDictionary
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

				try
				{
					sessionManager.BeginReadTransaction();

					containers = sessionManager.Session.CreateCriteria(typeof(DicomDictionaryContainer))
						.Add(Expression.Eq("DictionaryName", _dictionaryName))
						.List();

					sessionManager.Commit();
				}
				catch (Exception e)
				{
					throw new DataStoreException(String.Format(SR.FormatDicomDictionaryFailedToLoad, _dictionaryName), e);
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