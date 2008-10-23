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
using System.Runtime.Serialization;
using System.Xml;
using System.IO;

using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Common
{
    public static class JsmlSerializer
    {
        /// <summary>
        /// Serializes the specified object to JSML format, using the specified objectName as the outermost tag name.
        /// </summary>
        /// <param name="dataObject"></param>
        /// <param name="objectName"></param>
        /// <returns></returns>
        public static string Serialize(object dataObject, string objectName)
        {
            return Serialize(dataObject, objectName, false);
        }

        /// <summary>
        /// Serializes the specified object to JSML format, using the specified objectName as the outermost tag name.
        /// </summary>
        /// <param name="dataObject"></param>
        /// <param name="objectName"></param>
        /// <param name="includeEmptyTags">Specifies whether or not to serialize null-valued properties.
        ///   If there are many null-valued properties, this will significantly affect the size of the JSML document.</param>
        /// <returns></returns>
        public static string Serialize(object dataObject, string objectName, bool includeEmptyTags)
        {
            if (dataObject == null)
                return "";

            string jsml = "";

            using (StringWriter sw = new StringWriter())
            {
                XmlTextWriter writer = new XmlTextWriter(sw);
                writer.Formatting = Formatting.Indented;
                SerializeHelper(dataObject, objectName, writer, includeEmptyTags);
                writer.Close();
                jsml = sw.ToString();
            }

            return jsml;
        }

        public static void Serialize(XmlWriter writer, object obj, string objectName, bool includeEmptyTags)
        {
            if (obj != null)
            {
                SerializeHelper(obj, objectName, writer, includeEmptyTags);
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

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(jsml);

            return DeserializeHelper(dataContract, xmlDoc.DocumentElement);
        }

        public static object Deserialize<TDataContract>(XmlReader reader)
        {
            return Deserialize(reader, typeof(TDataContract));
        }

        public static object Deserialize(XmlReader reader, Type dataContract)
        {
            XmlDocument xmlDoc = new XmlDocument();
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
        /// <param name="includeEmptyTags"></param>
        private static void SerializeHelper(object dataObject, string objectName, XmlWriter writer, bool includeEmptyTags)
        {
            if (dataObject == null)
            {
                if (includeEmptyTags)
                    writer.WriteElementString(objectName, String.Empty);
            }
            else if (dataObject is EntityRef)
            {
                writer.WriteElementString(objectName, SerializeEntityRef((EntityRef)dataObject));
            }
            else if (IsDataContract(dataObject.GetType()))
            {
                List<IObjectMemberContext> dataMemberFields = new List<IObjectMemberContext>(GetDataMemberFields(dataObject));
                if (dataMemberFields.Count > 0)
                {
                    writer.WriteStartElement(objectName);
					writer.WriteAttributeString("hash", "true");
					foreach (IObjectMemberContext context in dataMemberFields)
                    {
						SerializeHelper(context.MemberValue, context.Member.Name, writer, includeEmptyTags);
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
                IDictionary dic = (IDictionary) dataObject;

				writer.WriteStartElement(objectName);
				writer.WriteAttributeString("hash", "true");
				foreach (DictionaryEntry entry in dic)
                {
                    SerializeHelper(entry.Value, entry.Key.ToString(), writer, includeEmptyTags);
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

                foreach (object item in (IList)dataObject)
                {
                    SerializeHelper(item, "item", writer, includeEmptyTags);
                }

                writer.WriteEndElement();
            }
            else if(dataObject is XmlDocument)
            {
                // this clause supports serialization of an embedded JSML document inline with the
                // output of the serializer
				writer.WriteStartElement(objectName);
				writer.WriteAttributeString("hash", "true");
				XmlDocument xmlDoc = (XmlDocument)dataObject;
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
                dataObject = DeserializeEntityRef(xmlElement.InnerText);
            }
            else if (IsDataContract(dataType))
            {
                dataObject = Activator.CreateInstance(dataType);

				foreach (IObjectMemberContext context in GetDataMemberFields(dataObject))
                {
					XmlElement memberElement = GetFirstElementWithTagName(xmlElement, context.Member.Name);
                    if (memberElement != null)
                    {
                        object memberObject = DeserializeHelper(context.MemberType, memberElement);
                        context.MemberValue = memberObject;
                    	xmlElement.RemoveChild(memberElement);
                    }
                }

				// If there are more child nodes left, it means the Xml describe a larger data contract 
				// than the current data type.  Hence this is not the right data type.
				if (xmlElement.HasChildNodes)
					return null;
            }
            else if (typeof(IDictionary).IsAssignableFrom(dataType))
            {
                // this clause was added mainly to support de-serialization of ExtendedProperties
                // note that only strongly-typed dictionaries are supported, and the key type *must* be "string",
                // and the value type must be JSML-serializable
                dataObject = Activator.CreateInstance(dataType);
                Type[] genericTypes = dataType.GetGenericArguments();
                Type keyType = genericTypes[0];
                if(keyType != typeof(string))
                    throw new NotSupportedException("Only IDictionary<string, T>, where T is a JSML-serializable type, is supported.");
                Type valueType = genericTypes[1];

                foreach (XmlNode node in xmlElement.ChildNodes)
                {
                    if(node is XmlElement)
                    {
                        object value = DeserializeHelper(valueType, (XmlElement)node);
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
                Type[] genericTypes = dataType.GetGenericArguments();

                XmlNodeList nodeList = xmlElement.SelectNodes("item");
                foreach (XmlNode node in nodeList)
                {
                    object iteratorObject = DeserializeHelper(genericTypes[0], (XmlElement)node);
                    ((IList)dataObject).Add(iteratorObject);
                }
            }
            else if(dataType == typeof(XmlDocument))
            {
                // this clause supports deserialization of an embedded JSML document
                string xml = xmlElement.InnerXml;
                if(!string.IsNullOrEmpty(xml))
                {
                    XmlDocument doc = new XmlDocument();
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
        private static IEnumerable<IObjectMemberContext> GetDataMemberFields(object dataObject)
        {
			ObjectWalker walker = new ObjectWalker(
				delegate(MemberInfo member) { return AttributeUtils.HasAttribute<DataMemberAttribute>(member, true); });
        	walker.IncludeNonPublicFields = true;
        	walker.IncludeNonPublicProperties = true;

        	return walker.Walk(dataObject);
        }

        private static XmlElement GetFirstElementWithTagName(XmlElement xmlElement, string tagName)
        {
            return (XmlElement)CollectionUtils.FirstElement(xmlElement.SelectNodes(tagName));
        }

        private static bool IsDataContract(Type t)
        {
            return t.GetCustomAttributes(typeof(DataContractAttribute), false).Length > 0;
        }
        
        private static string SerializeEntityRef(EntityRef entityRef)
        {
            return string.Format("{0}:{1}:{2}:{3}",
                EntityRefUtils.GetClassName(entityRef),
                EntityRefUtils.GetOID(entityRef).GetType().AssemblyQualifiedName,
                EntityRefUtils.GetOID(entityRef),
                EntityRefUtils.GetVersion(entityRef));
        }

        private static EntityRef DeserializeEntityRef(string value)
        {
            Platform.CheckForNullReference(value, "value");

            string[] parts = value.Split(':');
            if (parts.Length != 4)
                throw new SerializationException("Invalid EntityRef string");

            string entityClassName = parts[0];
            Type oidType = Type.GetType(parts[1], true);
            string oidValue = parts[2];
            int version = int.Parse(parts[3]);

            object oid = null;
            if(oidType == typeof(int))
            {
                oid = int.Parse(oidValue);
            }
            else if(oidType == typeof(long))
            {
                oid = long.Parse(oidValue);
            }
            else if(oidType == typeof(string))
            {
                oid = oidValue;
            }
            else if(oidType == typeof(Guid))
            {
                oid = new Guid(oidValue);
            }
            else
                throw new SerializationException("Invalid EntityRef string");

            return new EntityRef(entityClassName, oid, version);
        }

        #endregion
    }
}
