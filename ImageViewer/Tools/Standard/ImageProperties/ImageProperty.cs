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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.ImageViewer.Tools.Standard.ImageProperties
{
	public interface IImageProperty
	{
		string Identifier { get; }

		string Category { get; }
		string Name { get; }
		string Description { get; }
		bool IsEmpty { get; }
		object Value { get; }
		Type ValueType { get; }
	}

	public class ImageProperty : IImageProperty
	{
		public ImageProperty()
		{}

		public ImageProperty(string identifier, string category, string name, string description, object value)
		{
			Identifier = identifier;
			Category = category;
			Name = name;
			Description = description;
			Value = value;
		}

		#region IImageProperty Members

		public string Identifier { get; set; }

		public string Category { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public object Value { get; set; }

		public Type ValueType
		{
			get
			{
				if (Value == null)
					return typeof(string);
				else
					return Value.GetType();
			}
		}

		public virtual bool IsEmpty
		{
			get 
			{
				if (Value == null)
					return true;
				if (Value is string)
					return ((string) Value).Length == 0;

				return false;
			}	
		}

		#endregion

		public static ImageProperty Create(DicomAttribute attribute, string category, string name, string description, string separator)
		{
			string identifier = attribute.Tag.VariableName;
			if (String.IsNullOrEmpty(identifier))
				identifier = attribute.Tag.HexString;

			if (attribute.IsNull || attribute.IsEmpty)
				return new ImageProperty(identifier, category, name, description, "");

			if (String.IsNullOrEmpty(separator))
				separator = ", ";

			string value;
			if (attribute.Tag.VR.Name == DicomVr.DAvr.Name)
			{
				value = StringUtilities.Combine(attribute.Values as string[], separator,
				                                delegate(string dateString)
				                                	{
				                                		DateTime? date = DateParser.Parse(dateString);
				                                		if (!date.HasValue)
				                                			return null;
				                                		else
				                                			return Format.Date(date.Value);
				                                	}, true);
			}
			else if (attribute.Tag.VR.Name == DicomVr.TMvr.Name)
			{
				value = StringUtilities.Combine(attribute.Values as string[], separator,
				                                delegate(string timeString)
				                                	{
				                                		DateTime? time = TimeParser.Parse(timeString);
				                                		if (!time.HasValue)
				                                			return null;
				                                		else
				                                			return Format.Time(time.Value);
				                                	}, true);
			}
			else if (attribute.Tag.VR.Name == DicomVr.DTvr.Name)
			{
				value = StringUtilities.Combine(attribute.Values as string[], separator,
				                                delegate(string dateTimeString)
				                                	{
				                                		DateTime? dateTime = DateTimeParser.Parse(dateTimeString);
				                                		if (!dateTime.HasValue)
				                                			return null;
				                                		else
				                                			return Format.Time(dateTime.Value);
				                                	}, true);
			}
			else if (attribute.Tag.VR.Name == DicomVr.PNvr.Name)
			{
				value = StringUtilities.Combine(attribute.Values as string[], separator,
				                                delegate(string nameString)
				                                	{
				                                		PersonName personName = new PersonName(nameString ?? "");
				                                		return personName.FormattedName;
				                                	}, true);
			}
			else if (attribute.Tag.VR.IsTextVR && attribute.GetValueType() == typeof(string[]))
			{
				value = StringUtilities.Combine(attribute.Values as string[], separator, true);
			}
			else
			{
				value = attribute.ToString();
			}

			return new ImageProperty(identifier, category, name, description, value);
		}
	}
}