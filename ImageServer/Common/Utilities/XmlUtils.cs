using System;
using System.IO;
using System.Security;
using System.Xml;
using System.Xml.Serialization;
using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Common.Utilities
{
    /// <summary>
    /// A Utility class that provides methods related to working with XmlDocuments.
    /// </summary>
    public static class XmlUtils
    {
        /// <summary>
        /// A method for converting and XmlDocument to it's string representation, with the option
        /// of escaping the special characters if desired.
        /// </summary>
        public static string GetXmlDocumentAsString(XmlDocument doc, bool escapeChars)
        {
            StringWriter sw = new StringWriter();
            XmlTextWriter xw = new XmlTextWriter(sw);
            doc.WriteTo(xw);

            return escapeChars ? SecurityElement.Escape(sw.ToString()) : sw.ToString();
        }

        /// <summary>
        /// Deserialize an xml node into an object of the specified type.
        /// </summary>
        /// <typeparam name="T">Type of the object to deserialize into</typeparam>
        /// <param name="node">The node to be deserialized</param>
        /// <returns></returns>
        public static T Deserialize<T>(XmlNode node)
        {
            Platform.CheckForNullReference(node, "node");

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            XmlNodeReader reader = new XmlNodeReader(node);
            return (T)serializer.Deserialize(reader);
        }

        /// <summary>
        /// Serializes an object into an XML format.
        /// </summary>
        /// <param name="obj">Object to be serialized</param>
        /// <returns>An XmlNode that contains the serialized object</returns>
        /// <remarks>
        /// To use the returned <see cref="XmlNode"/> in an <see cref="XmlDocument"/>, <see cref="XmlDocument.ImportNode"/> must be used.
        /// </remarks>
        public static XmlNode Serialize(Object obj)
        {
            Platform.CheckForNullReference(obj, "obj");

            StringWriter sw = new StringWriter();
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            serializer.Serialize(sw, obj);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(sw.ToString());

            return doc.DocumentElement;
        }
    
    }
}
