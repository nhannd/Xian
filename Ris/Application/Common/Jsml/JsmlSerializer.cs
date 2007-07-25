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

        public static DateTime? ParseIsoDateTime(string isoDateString)
        {
            if (String.IsNullOrEmpty(isoDateString))
                return null;

            int y = int.Parse(isoDateString.Substring(0, 4));
            int m = int.Parse(isoDateString.Substring(5, 2));
            int d = int.Parse(isoDateString.Substring(8, 2));
            int h = int.Parse(isoDateString.Substring(11, 2));
            int n = int.Parse(isoDateString.Substring(14, 2));
            int s = int.Parse(isoDateString.Substring(17, 2));

            return new DateTime(y, m, d, h, n, s);
        }

        public static string GetIsoDateTime(DateTime dt)
        {
            string isoDateTime = String.Format("{0}-{1}-{2}T{3}:{4}:{5}",
                dt.Year,
                PrefixWithZero(dt.Month),
                PrefixWithZero(dt.Day),
                PrefixWithZero(dt.Hour),
                PrefixWithZero(dt.Minute),
                PrefixWithZero(dt.Second));

            return isoDateTime;
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
            else if (dataObject is string)
            {
                writer.WriteElementString(objectName, dataObject.ToString());
            }
            else if (dataObject is DateTime)
            {
                writer.WriteElementString(objectName, GetIsoDateTime((DateTime)dataObject));
            }
            else if (dataObject is DateTime?)
            {
                writer.WriteElementString(objectName, GetIsoDateTime(((DateTime?)dataObject).Value));
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
            else if (dataType == typeof(DateTime) || dataType == typeof(DateTime?))
            {
                dataObject = ParseIsoDateTime(xmlElement.InnerText);
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
