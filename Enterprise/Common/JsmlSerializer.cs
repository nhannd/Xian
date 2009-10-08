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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using System.IO;

using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Common
{
    public static class JsmlSerializer
    {
		public class SerializeOptions
		{
			public static readonly SerializeOptions Default = new SerializeOptions();

			public SerializeOptions()
			{
				// default to no filter
				this.MemberFilter = delegate { return true; };
			}


			/// <summary>
			/// Specifies whether or not to serialize null-valued properties.
			/// If there are many null-valued properties, this will significantly affect the size of the JSML document.
			/// </summary>
			public bool IncludeEmptyTags { get; set; }

			public Predicate<MemberInfo> MemberFilter { get; set; }
		}



        /// <summary>
        /// Serializes the specified object to JSML format, using the specified objectName as the outermost tag name.
        /// </summary>
        /// <param name="dataObject"></param>
        /// <param name="objectName"></param>
        /// <returns></returns>
        public static string Serialize(object dataObject, string objectName)
        {
            return Serialize(dataObject, objectName, SerializeOptions.Default);
        }

        /// <summary>
        /// Serializes the specified object to JSML format, using the specified objectName as the outermost tag name.
        /// </summary>
        /// <param name="dataObject"></param>
        /// <param name="objectName"></param>
		/// <param name="includeEmptyTags"></param>
        /// <returns></returns>
		public static string Serialize(object dataObject, string objectName, bool includeEmptyTags)
        {
        	return Serialize(dataObject, objectName, new SerializeOptions {IncludeEmptyTags = includeEmptyTags});
        }

        /// <summary>
        /// Serializes the specified object to JSML format, using the specified objectName as the outermost tag name.
        /// </summary>
        /// <param name="dataObject"></param>
        /// <param name="objectName"></param>
		/// <param name="options"></param>
        /// <returns></returns>
		public static string Serialize(object dataObject, string objectName, SerializeOptions options)
        {
            if (dataObject == null)
                return "";

            using (var sw = new StringWriter())
            {
                var writer = new XmlTextWriter(sw) {Formatting = Formatting.Indented};
            	SerializeHelper(dataObject, objectName, writer, options);
                writer.Close();
                return sw.ToString();
            }
        }

		public static void Serialize(XmlWriter writer, object obj, string objectName, bool includeEmptyTags)
        {
			Serialize(writer, obj, objectName, new SerializeOptions { IncludeEmptyTags = includeEmptyTags });
        }

        public static void Serialize(XmlWriter writer, object obj, string objectName, SerializeOptions options)
        {
            if (obj != null)
            {
				SerializeHelper(obj, objectName, writer, options);
            }
        }


        /// <summary>
        /// Deserializes the specified JSML text into an object of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsml"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string jsml)
            where T : new()
        {
            return (T)Deserialize(typeof(T), jsml);
        }

        /// <summary>
        /// Deserializes the specified JSML text into an object of the specified type.
        /// </summary>
        /// <param name="dataContract"></param>
        /// <param name="jsml"></param>
        /// <returns></returns>
        public static object Deserialize(Type dataContract, string jsml)
        {
            if (String.IsNullOrEmpty(jsml))
                return null;

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(jsml);

            return DeserializeHelper(dataContract, xmlDoc.DocumentElement);
        }

        public static object Deserialize<TDataContract>(XmlReader reader)
        {
            return Deserialize(reader, typeof(TDataContract));
        }

        public static object Deserialize(XmlReader reader, Type dataContract)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(reader);

            return DeserializeHelper(dataContract, xmlDoc.DocumentElement);
        }


        #region Private Helpers

        /// <summary>
        /// Serialize an object to Jsml format.  Recurse if the object is a DataContractBase or IList
        /// </summary>
        /// <param name="dataObject"></param>
        /// <param name="objectName"></param>
        /// <param name="writer"></param>
        /// <param name="options"></param>
        private static void SerializeHelper(object dataObject, string objectName, XmlWriter writer, SerializeOptions options)
        {
            if (dataObject == null)
            {
                if (options.IncludeEmptyTags)
                    writer.WriteElementString(objectName, String.Empty);
            }
            else if (dataObject is EntityRef)
            {
                writer.WriteElementString(objectName, ((EntityRef)dataObject).Serialize());
            }
            else if (IsDataContract(dataObject.GetType()))
            {
                var dataMemberFields = new List<IObjectMemberContext>(GetDataMemberFields(dataObject, options.MemberFilter));
                if (dataMemberFields.Count > 0)
                {
                    writer.WriteStartElement(objectName);
					writer.WriteAttributeString("hash", "true");
					foreach (var context in dataMemberFields)
                    {
						SerializeHelper(context.MemberValue, context.Member.Name, writer, options);
                    }
                    writer.WriteEndElement();
                }
            }
            else if (dataObject is IDictionary)
            {
                // this clause was added mainly to support serialization of ExtendedProperties, which 
                // is always a string-keyed dictionary
                // the dictionary is serialized as if it were an object and each key is a property on the object
                // jscript will not be able to distinguish that it was originally a dictionary
                // note that if the dictionary contains non-string keys, unpredictable behaviour may result
                var dic = (IDictionary) dataObject;

				writer.WriteStartElement(objectName);
				writer.WriteAttributeString("hash", "true");
				foreach (DictionaryEntry entry in dic)
                {
                    SerializeHelper(entry.Value, entry.Key.ToString(), writer, options);
                }
                writer.WriteEndElement();
            }
            else if (dataObject is Enum)
            {
                writer.WriteElementString(objectName, dataObject.ToString());
            }
            else if (dataObject is string)
            {
                writer.WriteElementString(objectName, dataObject.ToString());
            }
            else if (dataObject is DateTime)
            {
                writer.WriteElementString(objectName, DateTimeUtils.FormatISO((DateTime)dataObject));
            }
            else if (dataObject is DateTime?)
            {
                writer.WriteElementString(objectName, DateTimeUtils.FormatISO(((DateTime?)dataObject).Value));
            }
            else if (dataObject is bool)
            {
                writer.WriteElementString(objectName, (bool)dataObject ? "true" : "false");
            }
            else if (dataObject is IList)
            {
                writer.WriteStartElement(objectName);
                writer.WriteAttributeString("array", "true");

                foreach (var item in (IList)dataObject)
                {
                    SerializeHelper(item, "item", writer, options);
                }

                writer.WriteEndElement();
            }
            else if(dataObject is XmlDocument)
            {
                // this clause supports serialization of an embedded JSML document inline with the
                // output of the serializer
				writer.WriteStartElement(objectName);
				writer.WriteAttributeString("hash", "true");
				var xmlDoc = (XmlDocument)dataObject;
                if(xmlDoc.DocumentElement != null)
                {
 					xmlDoc.DocumentElement.WriteTo(writer);
                }
				writer.WriteEndElement();
			}
            else
            {
                writer.WriteElementString(objectName, dataObject.ToString());
            }
        }

        /// <summary>
        /// Create an object of type 'dataType' from the xmlElement.  Recurse if the object is a DataContractBase or IList
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="xmlElement"></param>
        /// <returns></returns>
        private static object DeserializeHelper(Type dataType, XmlElement xmlElement)
        {
            object dataObject = null;

            if (dataType == typeof(EntityRef))
            {
                dataObject = new EntityRef(xmlElement.InnerText);
            }
            else if (IsDataContract(dataType))
            {
                dataObject = Activator.CreateInstance(dataType);

				foreach (var context in GetDataMemberFields(dataObject, delegate { return true; }))
                {
					var memberElement = GetFirstElementWithTagName(xmlElement, context.Member.Name);
                    if (memberElement != null)
                    {
                        var memberObject = DeserializeHelper(context.MemberType, memberElement);
                        context.MemberValue = memberObject;
                    }
                }
            }
            else if (typeof(IDictionary).IsAssignableFrom(dataType))
            {
                // this clause was added mainly to support de-serialization of ExtendedProperties
                // note that only strongly-typed dictionaries are supported, and the key type *must* be "string",
                // and the value type must be JSML-serializable
                dataObject = Activator.CreateInstance(dataType);
                var genericTypes = dataType.GetGenericArguments();
                var keyType = genericTypes[0];
                if(keyType != typeof(string))
                    throw new NotSupportedException("Only IDictionary<string, T>, where T is a JSML-serializable type, is supported.");
                var valueType = genericTypes[1];

                foreach (XmlNode node in xmlElement.ChildNodes)
                {
                    if(node is XmlElement)
                    {
                        var value = DeserializeHelper(valueType, (XmlElement)node);
                        ((IDictionary)dataObject).Add(node.Name, value);
                    }
                }
            }
            else if (dataType == typeof(string))
            {
                dataObject = xmlElement.InnerText;
            }
            else if (dataType.IsEnum)
            {
                dataObject = Enum.Parse(dataType, xmlElement.InnerText);
            }
            else if (dataType == typeof(DateTime) || dataType == typeof(DateTime?))
            {
                dataObject = DateTimeUtils.ParseISO(xmlElement.InnerText);
            }
            else if (dataType == typeof(bool))
            {
                dataObject = xmlElement.InnerText.Equals("true") ? true : false;
            }
            else if (dataType.GetInterface("IList") == typeof(IList))
            {
                dataObject = Activator.CreateInstance(dataType);
                var genericTypes = dataType.GetGenericArguments();

                var nodeList = xmlElement.SelectNodes("item");
                foreach (XmlNode node in nodeList)
                {
                    var iteratorObject = DeserializeHelper(genericTypes[0], (XmlElement)node);
                    ((IList)dataObject).Add(iteratorObject);
                }
            }
            else if(dataType == typeof(XmlDocument))
            {
                // this clause supports deserialization of an embedded JSML document
                var xml = xmlElement.InnerXml;
                if(!string.IsNullOrEmpty(xml))
                {
                    var doc = new XmlDocument();
                    doc.LoadXml(xml);
                    dataObject = doc;
                }
            }
            else if (dataType.GetInterface("IConvertible") == typeof(IConvertible))
            {
                dataObject = Convert.ChangeType(xmlElement.InnerText, dataType);
            }
            else
            {
                dataObject = xmlElement.InnerText;
            }

            return dataObject;
        }

        /// <summary>
        /// Get a list of properties and fields from a data contract object with DataMemberAttribute
        /// </summary>
        private static IEnumerable<IObjectMemberContext> GetDataMemberFields(object dataObject, Predicate<MemberInfo> memberFilter)
        {
			var walker = new ObjectWalker(member => AttributeUtils.HasAttribute<DataMemberAttribute>(member, true) && memberFilter(member))
			             	{
			             		IncludeNonPublicFields = true,
			             		IncludeNonPublicProperties = true
			             	};

        	return walker.Walk(dataObject);
        }

        private static XmlElement GetFirstElementWithTagName(XmlNode xmlElement, string tagName)
        {
            return (XmlElement)CollectionUtils.FirstElement(xmlElement.SelectNodes(tagName));
        }

        private static bool IsDataContract(Type t)
        {
            return t.GetCustomAttributes(typeof(DataContractAttribute), false).Length > 0;
        }

        #endregion
    }
}
