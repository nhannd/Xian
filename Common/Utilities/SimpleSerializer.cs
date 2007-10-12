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
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;

using ClearCanvas.Common;

namespace ClearCanvas.Common.Utilities
{
	/// <summary>
	/// This attribute class is used to decorate properties of other classes for use with the <see cref="SimpleSerializer"/>.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public sealed class SimpleSerializedAttribute : Attribute
	{
		/// <summary>
		/// Default Constructor.
		/// </summary>
		public SimpleSerializedAttribute()
		{
		}
	}

	/// <summary>
	/// The <see cref="SimpleSerializer"/> class can be used to serialize to and from a string dictionary (Property/Value pairs).
	/// The resulting dictionary can then be stored to a file or setting so that objects can be easily persisted and restored.
	/// </summary>
	public sealed class SimpleSerializer
	{
		/// <summary>
		/// Thrown by <see cref="SimpleSerializer"/>'s public methods: <see cref="SimpleSerializer.Deserialize"/> and <see cref="SimpleSerializer.Serialize"/>.
		/// </summary>
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

		/// <summary>
		/// Populates the destinationObject's properties that are marked with a <see cref="SimpleSerializedAttribute"/> 
		/// attribute using the Property/Value pairs from the input dictionary (sourceValues).
		/// </summary>
		/// <param name="destinationObject">The object whose properties are to be initialized using the input dictionary's Property/Value pairs.</param>
		/// <param name="sourceValues">The input dictionary of Property/Value pairs.</param>
		/// <exception cref="ArgumentNullException">thrown when either of the input values are null</exception>
		/// <exception cref="SimpleSerializerException">thrown when an error occurs during serialization</exception>
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
							throw new InvalidOperationException(String.Format(SR.ExceptionFormatCannotConvertFromStringToType, propertyType.FullName));
					}
				}
			}
			catch (Exception e)
			{
				throw new SimpleSerializerException(String.Format(SR.ExceptionFormatSerializationFailedForType, destinationObject.GetType().FullName), e);
			}
		}

		/// <summary>
		/// Constructs and returns a dictionary of Property/Value pairs from the input sourceObject.
		/// Those properties decorated with the <see cref="SimpleSerializedAttribute"/> attribute will have their
		/// values extracted and inserted into the resulting dictionary.
		/// </summary>
		/// <param name="sourceObject">the object whose properties are to be extracted</param>
		/// <returns>a dictionary of Property/Value pairs</returns>
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
						throw new InvalidOperationException(String.Format(SR.ExceptionFormatCannotConvertFromTypeToString, propertyType.FullName));
				}

				return dictionary;
			}
			catch (Exception e)
			{
				throw new SimpleSerializerException(String.Format(SR.ExceptionFormatDeserializationFailedForType, sourceObject.GetType().FullName), e);
			}
		}
	}
}
