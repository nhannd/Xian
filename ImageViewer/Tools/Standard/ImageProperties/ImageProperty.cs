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
		string Category { get; }
		string Name { get; }
		string Description { get; }
		bool IsEmpty { get; }
		object Value { get; }
		Type ValueType { get; }
	}

	public class ImageProperty : IImageProperty
	{
		private string _category;
		private string _name;
		private string _description;
		private object _value;

		public ImageProperty()
		{
		}

		public ImageProperty(string category, string name, string description, object value)
		{
			_category = category;
			_name = name;
			_description = description;
			_value = value;
		}

		#region IImageProperty Members

		public string Category
		{
			get { return _category; }
			set { _category = value; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		public virtual bool IsEmpty
		{
			get 
			{
				if (_value == null)
					return true;
				if (_value is string)
					return ((string) _value).Length == 0;

				return false;
			}	
		}

		public Type ValueType
		{
			get 
			{
				if (_value == null)
					return typeof(string);
				else
					return _value.GetType();
			}	
		}

		public object Value
		{
			get { return _value; }
			set { _value = value; }
		}

		#endregion

		public static ImageProperty Create(DicomTag tag, IDicomAttributeProvider provider, string category, string name, string description, string separator)
		{
			return Create(provider[tag], category, name, description, separator);
		}

		public static ImageProperty Create(DicomAttribute attribute, string category, string name, string description, string separator)
		{
			if (attribute.IsNull || attribute.IsEmpty)
				return new ImageProperty(category, name, description, "");

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

			return new ImageProperty(category, name, description, value);
		}
	}
}