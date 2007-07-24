using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using System.IO;

using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Jsml
{
    public static class JsmlSerializer
    {
        /// <summary>
        /// Take an object of type DataContractBase and serialize all members with DataMemberAttribute to Jsml format.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataObject"></param>
        /// <returns></returns>
        public static string Serialize<T>(T dataObject)
            where T : DataContractBase
        {
            return Serialize<T>(dataObject, false);
        }

        /// <summary>
        /// Take an object of type DataContractBase and serialize all members with DataMemberAttribute to Jsml format.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataObject"></param>
        /// <param name="includeEmptyTags"></param>
        /// <returns></returns>
        public static string Serialize<T>(T dataObject, bool includeEmptyTags)
            where T : DataContractBase
        {
            return Serialize((DataContractBase)dataObject, includeEmptyTags);
        }

        public static string Serialize(DataContractBase dataObject, bool includeEmptyTags)
        {
            string jsml = "";

            using (StringWriter sw = new StringWriter())
            {
                XmlTextWriter writer = new XmlTextWriter(sw);
                writer.Formatting = System.Xml.Formatting.Indented;
                SerializeHelper(dataObject, dataObject.GetType().Name, writer, false);
                writer.Close();
                jsml = sw.ToString();
            }

            return jsml;
        }


        /// <summary>
        /// Take a jsml string and deserialize into a DataContractBase object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsml"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string jsml)
            where T : DataContractBase, new()
        {
            return (T)Deserialize(typeof(T), jsml);
        }

        public static DataContractBase Deserialize(Type dataContract, string jsml)
        {
            if (String.IsNullOrEmpty(jsml))
                return null;

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(jsml);

            return (DataContractBase)DeserializeHelper(dataContract, xmlDoc.DocumentElement);
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
            if (dataObject is DataContractBase)
            {
                List<FieldInfo> dataMemberFields = GetDataMemberFields((DataContractBase)dataObject);
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
            else
            {
                if (dataObject == null)
                {
                    if (includeEmptyTags)
                        writer.WriteElementString(objectName, String.Empty);
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

            if (dataType.BaseType == typeof(DataContractBase))
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
            else
            {
                if (dataType == typeof(string))
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
            }

            return dataObject;
        }

        /// <summary>
        /// Get a list of properties and fields from a data contract object with DataMemberAttribute
        /// </summary>
        /// <param name="dataContract"></param>
        /// <returns></returns>
        private static List<FieldInfo> GetDataMemberFields(DataContractBase dataContract)
        {
            List<FieldInfo> dataMemberFields = CollectionUtils.Select<FieldInfo, List<FieldInfo>>(
                dataContract.GetType().GetFields(),
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

        #endregion
    }
}
