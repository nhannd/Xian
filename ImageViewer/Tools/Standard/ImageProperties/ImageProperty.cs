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