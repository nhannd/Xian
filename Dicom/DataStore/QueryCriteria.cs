#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.Dicom.DataStore
{
	internal class QueryCriteria : IEnumerable<KeyValuePair<DicomTagPath, string>>
	{
		private readonly IDictionary<DicomTagPath, string> _internalDictionary;

		public QueryCriteria(DicomAttributeCollection query)
		{
			_internalDictionary = new Dictionary<DicomTagPath, string>();
			BuildQueryDictionary(query, null, _internalDictionary);
		}

		public string this[DicomTagPath path]
		{
			get
			{
				if (!_internalDictionary.ContainsKey(path))
					return null;

				return _internalDictionary[path];
			}
		}

		private static void BuildQueryDictionary(IEnumerable<DicomAttribute> collection, DicomTagPath currentPath, IDictionary<DicomTagPath, string> queryDictionary)
		{
			foreach (DicomAttribute attribute in collection)
			{
				DicomTagPath path;
				if (currentPath == null)
					path = new DicomTagPath(attribute.Tag);
				else
					path = currentPath + attribute.Tag;

				if (attribute is DicomAttributeSQ && attribute.Values != null)
				{
					foreach (DicomAttributeCollection sequenceItem in (object[])attribute.Values)
						BuildQueryDictionary(sequenceItem, path, queryDictionary);
				}

				if (!attribute.IsEmpty)
					queryDictionary[path] = attribute.ToString() ?? "";
			}
		}

		#region IEnumerable<KeyValuePair<DicomTagPath,string>> Members

		public IEnumerator<KeyValuePair<DicomTagPath, string>> GetEnumerator()
		{
			return _internalDictionary.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _internalDictionary.GetEnumerator();
		}

		#endregion
	}
}
