#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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