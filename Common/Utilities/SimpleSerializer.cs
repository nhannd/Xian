using System;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;

using ClearCanvas.Common;

namespace ClearCanvas.Common.Utilities
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public sealed class SimpleSerializedAttribute : Attribute
	{
		public SimpleSerializedAttribute()
		{
		}
	}

	public sealed class SimpleSerializer
	{
		[Serializable]
		public sealed class SimpleSerializerException : Exception
		{
			internal SimpleSerializerException(string message, Exception innerException)
				: base(message, innerException)
			{
			}
		}

		private SimpleSerializer()
		{
		}

		public static void Serialize(object destinationObject, IDictionary<string, string> sourceValues)
		{
			Platform.CheckForNullReference(destinationObject, "destinationObject");
			Platform.CheckForNullReference(sourceValues, "sourceValues");

			try
			{
				PropertyInfo[] properties = destinationObject.GetType().GetProperties();

				foreach (PropertyInfo property in properties)
				{
					if (!property.IsDefined(typeof(SimpleSerializedAttribute), false))
						continue;

					Type propertyType = property.PropertyType;
					if (sourceValues.ContainsKey(property.Name))
					{
						string value = sourceValues[property.Name];

						TypeConverter converter = TypeDescriptor.GetConverter(propertyType);
						if (converter.CanConvertFrom(typeof(string)))
								property.SetValue(destinationObject, converter.ConvertFromString(value), null);
						else
							throw new InvalidOperationException(String.Format("Unable to convert from String to {0}", propertyType.FullName));
					}
				}
			}
			catch (Exception e)
			{
				throw new SimpleSerializerException(String.Format("Serialization failed for object type '{0}'", destinationObject.GetType().FullName), e);
			}
		}

		public static IDictionary<string, string> Deserialize(object sourceObject)
		{
			Platform.CheckForNullReference(sourceObject, "sourceObject");

			Dictionary<string, string> dictionary = new Dictionary<string, string>();

			try
			{
				PropertyInfo[] properties = sourceObject.GetType().GetProperties();
				foreach (PropertyInfo property in properties)
				{
					if (!property.IsDefined(typeof(SimpleSerializedAttribute), false))
						continue;

					Type propertyType = property.PropertyType;
					object value = property.GetValue(sourceObject, null);

					if (value == null)
						continue;

					TypeConverter converter = TypeDescriptor.GetConverter(propertyType);
					if (converter.CanConvertTo((typeof(string))))
					{
						string stringValue = converter.ConvertToString(value);
						if (!String.IsNullOrEmpty(stringValue))
							dictionary[property.Name] = stringValue;
					}
					else
						throw new InvalidOperationException(String.Format("Unable to convert from {0} to String", propertyType.FullName));
				}

				return dictionary;
			}
			catch (Exception e)
			{
				throw new SimpleSerializerException(String.Format("Deserialization failed for object type '{0}'", sourceObject.GetType().FullName), e);
			}
		}
	}
}
