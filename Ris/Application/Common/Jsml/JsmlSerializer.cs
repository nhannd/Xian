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

namespace ClearCanvas.Ris.Application.Common.Jsml
{
    public static class JsmlSerializer
    {
        /// <summary>
        /// Take an object and serialize all members with DataMemberAttribute to Jsml format.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataObject"></param>
        /// <returns></returns>
        public static string Serialize(object dataObject, string objectName)
        {
            return Serialize(dataObject, objectName, false);
        }

        public static string Serialize(object dataObject, string objectName, bool includeEmptyTags)
        {
            if (dataObject == null)
                return "";

            string jsml = "";

            using (StringWriter sw = new StringWriter())
            {
                XmlTextWriter writer = new XmlTextWriter(sw);
                writer.Formatting = System.Xml.Formatting.Indented;
                SerializeHelper(dataObject, objectName, writer, false);
                writer.Close();
                jsml = sw.ToString();
            }

            return jsml;
        }


        /// <summary>
        /// Take a jsml string and deserialize into an object of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsml"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string jsml)
            where T : new()
        {
            return (T)Deserialize(typeof(T), jsml);
        }

        public static object Deserialize(Type dataContract, string jsml)
        {
            if (String.IsNullOrEmpty(jsml))
                return null;

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(jsml);

            return (object)DeserializeHelper(dataContract, xmlDoc.DocumentElement);
        }


        #region Private Helpers

        /// <summary>
        /// Serialize an object to Jsml format.  Recurse if the object is a DataContractBase or IList
        /// </summary>
        /// <param name="dataObject"></param>
        /// <param name="objectName"></param>
        /// <param name="writer"></param>
        /// <param name="includeEmptyTags"></param>
        private static void SerializeHelper(object dataObject, string objectName, XmlTextWriter writer, bool includeEmptyTags)
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
                List<FieldInfo> dataMemberFields = GetDataMemberFields(dataObject);
                if (dataMemberFields.Count > 0)
                {
                    writer.WriteStartElement(objectName);
                    foreach (FieldInfo info in dataMemberFields)
                    {
                        SerializeHelper(info.GetValue(dataObject), info.Name, writer, includeEmptyTags);
                    }
                    writer.WriteEndElement();
                }
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

                List<FieldInfo> dataMemberFields = GetDataMemberFields((DataContractBase)dataObject);
                foreach (FieldInfo info in dataMemberFields)
                {
                    XmlElement memberElement = GetFirstElementWithTagName(xmlElement, info.Name);
                    if (memberElement != null)
                    {
                        object memberObject = DeserializeHelper(info.FieldType, memberElement);
                        info.SetValue(dataObject, memberObject);
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

                XmlNodeList nodeList = xmlElement.GetElementsByTagName("item");
                foreach (XmlNode node in nodeList)
                {
                    object iteratorObject = DeserializeHelper(genericTypes[0], (XmlElement)node);
                    ((IList)dataObject).Add(iteratorObject);
                }
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
        /// <param name="dataContract"></param>
        /// <returns></returns>
        private static List<FieldInfo> GetDataMemberFields(object dataObject)
        {
            List<FieldInfo> dataMemberFields = CollectionUtils.Select<FieldInfo, List<FieldInfo>>(
                dataObject.GetType().GetFields(),
                delegate(FieldInfo info)
                {
                    object[] attribs = info.GetCustomAttributes(typeof(DataMemberAttribute), true);
                    return attribs.Length > 0;
                });

            return dataMemberFields;
        }

        private static XmlElement GetFirstElementWithTagName(XmlElement xmlElement, string tagName)
        {
            return (XmlElement)CollectionUtils.FirstElement<XmlNode>(xmlElement.GetElementsByTagName(tagName));
        }

        private static string PrefixWithZero(int n) 
        {
            // Format integers to have at least two digits.
            return n < 10 ? '0' + n.ToString() : n.ToString();
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
