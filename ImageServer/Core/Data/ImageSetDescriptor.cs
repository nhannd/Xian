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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common.Utilities;

namespace ClearCanvas.ImageServer.Core.Data
{
	public class ImageSetField : IEquatable<ImageSetField>
	{
		private DicomTag _tag;
		private string _value;

		public DicomTag DicomTag
		{
			get { return _tag; }
			set
			{
				Debug.Assert(value != null);
				_tag = value;
			}
		}

		public string Tag
		{
			get { return _tag.HexString; }
			set
			{
				Debug.Assert(!String.IsNullOrEmpty(value));
				_tag = DicomTagDictionary.GetDicomTag(uint.Parse(value, NumberStyles.HexNumber));
			}
		}

		public string Value
		{
			get { return _value; }
			set { _value = value; }
		}

		public ImageSetField() { }

		public ImageSetField(DicomAttribute attr)
		{
			_tag = attr.Tag;
			if (attr.IsEmpty || attr.IsNull)
				_value = String.Empty;
			else
			{
				_value = RemoveEscapeChars(attr.ToString());
			}
		}

		#region IEquatable<ImageSetField> Members

		public bool Equals(ImageSetField other)
		{
			return DicomTag.Equals(other.DicomTag) && _value.Equals(other.Value);
		}

		#endregion

		#region Private Methods
		private string RemoveEscapeChars(string text)
		{
			if (String.IsNullOrEmpty(text))
				return text;

			// Remove escape characters
			string escape = String.Format("{0}", (char)0x1B);
			string replacement = "";
			text = text.Replace(escape, replacement);
			return text;
		}

		#endregion
	}

	/// <summary>
	/// Represents a serializable descriptor of an image set.
	/// </summary>
	[Serializable]
	[XmlRoot("ImageSetDescriptor")]
	public class ImageSetDescriptor : IEquatable<ImageSetDescriptor>, IXmlSerializable
	{
		#region Private Method
		private Dictionary<DicomTag, ImageSetField> _fields = new Dictionary<DicomTag, ImageSetField>();
		#endregion

		#region Constructors
		public ImageSetDescriptor()
		{
		}

		public ImageSetDescriptor(IDicomAttributeProvider attributeProvider)
		{
			PopulateField(DicomTags.PatientId, attributeProvider);
			PopulateField(DicomTags.IssuerOfPatientId, attributeProvider);
			PopulateField(DicomTags.PatientsName, attributeProvider);
			PopulateField(DicomTags.PatientsBirthDate, attributeProvider);
			PopulateField(DicomTags.PatientsSex, attributeProvider);
			PopulateField(DicomTags.AccessionNumber, attributeProvider);
			PopulateField(DicomTags.StudyDate, attributeProvider);
		}
		#endregion

		#region Private Methods
		private void PopulateField(uint tag, IDicomAttributeProvider attributeProvider)
		{
			DicomAttribute attr = null;
			if (attributeProvider.TryGetAttribute(tag, out attr))
			{
				AddField(new ImageSetField(attr));
			}
			else
			{
				// add default value
				AddField(new ImageSetField(DicomTagDictionary.GetDicomTag(tag).CreateDicomAttribute()));    
			}

		}

		#endregion

		#region Public Properties
		public ImageSetField[] Fields
		{
			get
			{
				ImageSetField[] array = new ImageSetField[_fields.Count];
				_fields.Values.CopyTo(array, 0);
				return array;
			}
			set
			{
				_fields = new Dictionary<DicomTag, ImageSetField>();
				foreach (ImageSetField field in value)
				{
					_fields.Add(field.DicomTag, field);
				}
			}
		}
		#endregion

		#region Indexers
		public ImageSetField this[DicomTag tag]
		{
			get
			{
				if (_fields.ContainsKey(tag))
					return _fields[tag];
				else
					return null;
			}
		}

		public ImageSetField this[uint tag]
		{
			get
			{
                DicomTag theTag = DicomTagDictionary.GetDicomTag(tag);
                return this[theTag];
			}
		}
		#endregion

		#region Protected Methods
		protected void AddField(ImageSetField field)
		{
			_fields.Add(field.DicomTag, field);
		}
		#endregion

		static public ImageSetDescriptor Parse(XmlElement element)
		{
			return XmlUtils.Deserialize<ImageSetDescriptor>(element);
		}

		#region IEquatable<ImageSetDescriptor> Members

		public bool Equals(ImageSetDescriptor other)
		{
			if (this == other)
				return true;

			if (Fields.Length == 0 && other.Fields.Length > 0)
				return false;

			foreach (ImageSetField field in Fields)
			{
				if (other[field.DicomTag] == null || !field.Equals(other[field.DicomTag]))
					return false;
			}

			return true;
		}

		#endregion

		#region IXmlSerializable Members

		public System.Xml.Schema.XmlSchema GetSchema()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void ReadXml(XmlReader reader)
		{
			// skip <ImageSetDescriptor>
			reader.Read();
            
			while (reader.NodeType == XmlNodeType.Element && reader.LocalName == "Field")
			{
				ImageSetField field = new ImageSetField();
				field.Tag = reader["Tag"];
				field.Value = String.IsNullOrEmpty(reader["Value"]) ? String.Empty : reader["Value"];
				AddField(field);
				reader.Read();
			}
        

			if (reader.NodeType == XmlNodeType.Element && reader.LocalName == "Details")
			{
				//Details = XmlUtils.Deserialize<ImageSetDetails>(reader);
			}

			reader.Read();
		}

		public void WriteXml(XmlWriter writer)
		{
			foreach (ImageSetField field in _fields.Values)
			{
				writer.WriteStartElement("Field");
				writer.WriteAttributeString("Tag", field.Tag);
				writer.WriteAttributeString("Value", field.Value);
				writer.WriteEndElement();
			}

			//XmlUtils.Serialize(Details, writer);
		}

		#endregion



	}
}