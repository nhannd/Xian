#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Reflection;

namespace ClearCanvas.Dicom.DataStore
{
	internal class QueryablePropertyInfo
	{
		public QueryablePropertyInfo(PropertyInfo property, DicomTagPath path, 
			bool isComputed, bool returnAlways, bool returnOnly, PropertyInfo returnProperty)
		{
			Property = property;
			ColumnName = String.Format("{0}_", property.Name);
			Path = path;
			IsComputed = isComputed;
			ReturnAlways = returnAlways;
			ReturnProperty = returnProperty;
			ReturnOnly = returnOnly;

			//minor hack to allow 'list' queries for ModalitiesInStudy.
			AllowListMatching = path.ValueRepresentation == DicomVr.UIvr || property.Name == "ModalitiesInStudy";
		}

		public readonly DicomTagPath Path;

		public readonly PropertyInfo Property;
		public readonly string ColumnName;

		public readonly bool AllowListMatching;

		//if ReturnOnly is true, then no querying can be performed on the property.
		public readonly bool ReturnOnly;
		public readonly bool IsComputed;
		public readonly bool ReturnAlways;
		public readonly PropertyInfo ReturnProperty;

		public override string ToString()
		{
			return Path.ToString();
		}
	}

	internal class QueryablePropertyAttribute : Attribute
	{
		public QueryablePropertyAttribute(params DicomTag[] tags)
		{
			Path = new DicomTagPath(tags);
		}

		public QueryablePropertyAttribute(params uint[] tags)
		{
			Path = new DicomTagPath(tags);
		}

		public DicomTagPath Path;
		public bool ReturnOnly = false;
		public bool ReturnAlways = false;
		public bool IsComputed = false;
		public string ReturnProperty = null;
	}

	internal class QueryablePropertyCollection<T> : IEnumerable<QueryablePropertyInfo>
	{
		private static readonly Dictionary<DicomTagPath, QueryablePropertyInfo> _dictionary;

		public QueryablePropertyCollection()
		{
		}

		static QueryablePropertyCollection()
		{
			_dictionary = CreateDictionary(typeof(T));
		}

		private static Dictionary<DicomTagPath, QueryablePropertyInfo> CreateDictionary(Type type)
		{
			Dictionary<DicomTagPath, QueryablePropertyInfo> dictionary = new Dictionary<DicomTagPath, QueryablePropertyInfo>();

			foreach (PropertyInfo property in type.GetProperties())
			{
				foreach (QueryablePropertyAttribute attribute in property.GetCustomAttributes(typeof(QueryablePropertyAttribute), false))
				{
					PropertyInfo returnProperty = property;
					if (attribute.ReturnProperty != null)
						returnProperty = type.GetProperty(attribute.ReturnProperty);

					dictionary[attribute.Path] = 
						new QueryablePropertyInfo(property, attribute.Path, attribute.IsComputed, 
													attribute.ReturnAlways, attribute.ReturnOnly, returnProperty);
				}
			}

			return dictionary;
		}

		public QueryablePropertyInfo this[DicomTagPath path]
		{
			get
			{
				if (!_dictionary.ContainsKey(path))
					return null;

				return _dictionary[path];
			}
		}

		#region IEnumerable<DictionaryEntry> Members

		public IEnumerator<QueryablePropertyInfo> GetEnumerator()
		{
			return _dictionary.Values.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _dictionary.Values.GetEnumerator();
		}

		#endregion
	}
}