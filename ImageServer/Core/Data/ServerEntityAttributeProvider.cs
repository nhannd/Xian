#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

using System.Collections.Generic;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Core.Edit;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Core.Data
{
	/// <summary>
	/// Wrapper class implementing IDicomAttributeProvider for a ServerEntity object.
	/// </summary>
	public class ServerEntityAttributeProvider:IDicomAttributeProvider
	{
		#region Private Fields
		private readonly ServerEntity _entity;
		private readonly EntityDicomMap _fieldMap;
		private readonly Dictionary<DicomTag, DicomAttribute> _attributes = new Dictionary<DicomTag, DicomAttribute>();
		#endregion

		#region Constructors
		public ServerEntityAttributeProvider(ServerEntity entity)
		{
			_entity = entity;
			_fieldMap = EntityDicomMapManager.Get(entity.GetType());

		}
		#endregion

		#region IDicomAttributeProvider Members

		public DicomAttribute this[DicomTag tag]
		{
			get
			{
				if (_attributes.ContainsKey(tag))
					return _attributes[tag];

				if (!_fieldMap.ContainsKey(tag))
				{
					return null;
				}

				DicomAttribute attr = tag.CreateDicomAttribute();
				object value = _fieldMap[tag].GetValue(_entity, null);
				if (value!=null)
					attr.SetStringValue(value.ToString());
				_attributes.Add(tag, attr);
				return attr;
			}
			set
			{
				if (_fieldMap[tag]!=null)
				{
					_fieldMap[tag].SetValue(_entity, value.ToString(), null);
				}
			}
		}

		public DicomAttribute this[uint tag]
		{
			get
			{
				return this[DicomTagDictionary.GetDicomTag(tag)];
			}
			set
			{
				this[DicomTagDictionary.GetDicomTag(tag)] = value;
			}
		}

		public bool TryGetAttribute(uint tag, out DicomAttribute attribute)
		{
			if (this[tag] == null)
			{
				attribute = null;
				return false;
			}
			attribute = this[tag];
			return true;
		}

		public bool TryGetAttribute(DicomTag tag, out DicomAttribute attribute)
		{
			return TryGetAttribute(tag.TagValue, out attribute);
		}

		#endregion
	}
}