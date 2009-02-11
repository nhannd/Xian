using System;
using System.IO;
using System.Reflection;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

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
            XmlWriterSettings xmlSettings = new XmlWriterSettings();

            xmlSettings.Encoding = Encoding.UTF8;
            xmlSettings.ConformanceLevel = ConformanceLevel.Fragment;
            xmlSettings.Indent = true;
            xmlSettings.NewLineOnAttributes = false;
            xmlSettings.CheckCharacters = true;
            xmlSettings.IndentChars = "  ";

            XmlWriter xw = XmlWriter.Create(sw, xmlSettings);

            doc.WriteTo(xw);

            xw.Close();

            return escapeChars ? SecurityElement.Escape(sw.ToString()) : sw.ToString();
        }

        /// <summary>
        /// Deserialize an xml node into an object of the specified type.
        /// </summary>
        /// <typeparam name="T">Type of the object to deserialize into</typeparam>
        /// <param name="node">The node to be deserialized</param>
        /// <returns></returns>
        public static T Deserialize<T>(XmlNode node)
            where T:class
        {
            Platform.CheckForNullReference(node, "node");

            return Deserialize<T>(new XmlNodeReader(node));
        }


        /// <summary>
        /// Deserialize an xml node into an object of the specified type.
        /// </summary>
        /// <typeparam name="T">Type of the object to deserialize into</typeparam>
        /// <param name="reader">The <see cref="XmlReader"/> to read the xml content</param>
        /// <returns></returns>
        public static T Deserialize<T>(XmlReader reader)
            where T : class
        {
            Platform.CheckForNullReference(reader, "reader");

            XmlSerializer serializer = new XmlSerializer(typeof(T));
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
            return SerializeAsXmlDoc(obj).DocumentElement;
        }


        public static XmlDocument SerializeAsXmlDoc(Object obj)
        {
            Platform.CheckForNullReference(obj, "obj");
            StringWriter sw = new StringWriter();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(sw);

            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            serializer.Serialize(xmlTextWriter, obj);
            
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(sw.ToString());

            return doc;
        }

        public static string SerializeAsString(Object obj)
        {
            Platform.CheckForNullReference(obj, "obj");

            StringWriter sw = new StringWriter();
            XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;
			settings.NewLineOnAttributes = false;
			settings.OmitXmlDeclaration = true;
			settings.Encoding = Encoding.UTF8;


			using(XmlWriter writer = XmlWriter.Create(sw, settings))
			{
                XmlNode node = Serialize(obj);
                node.WriteTo(writer);
                writer.Flush();
			}
            
            return sw.ToString();
        }

        /// <summary>
        /// Ensures the value is Xml compatible.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string EncodeValue(string value)
        {
            string text = SecurityElement.Escape(value);
            
            // Remove escape characters
            string escape = String.Format("{0}", (char)0x1B);
            string replacement = "";
            text = text.Replace(escape, replacement);

            return text;
        }

        /// <summary>
        /// Replaces escaped characters with their ascii equivalent
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DecodeValue(string value)
        {
            // Cleanup the common XML character replacements
            string text = value;
            text= text.Replace("&lt;", "<").
                Replace("&gt;", ">").
                Replace("&quot;", "\"").
                Replace("&apos;", "'").
                Replace("&amp;", "&");
            return text;
        }

    }

    /// <summary>
    /// Helper class to serialize abstract class.
    /// </summary>
    /// <typeparam name="AbstractType"></typeparam>
    /// <remarks>
    /// To serialize a property whose type is an abstract class, use AbstractProperty as the Type 
    /// when adding <see cref="XmlAttribute"/> to the property. For example:
    /// 
    /// [XmlElement(Type=typeof(AbstractProperty<MyAbstractClass>))]
    /// public MyAbstractClass MyProperty { 
    ///     ... 
    /// }
    /// </remarks>
    public class AbstractProperty<AbstractType> : IXmlSerializable
    {
        // Override the Implicit Conversions Since the XmlSerializer
        // Casts to/from the required types implicitly.
        public static implicit operator AbstractType(AbstractProperty<AbstractType> obj)
        {
            return obj.Data;
        }

        public static implicit operator AbstractProperty<AbstractType>(AbstractType obj)
        {
            return obj == null ? null : new AbstractProperty<AbstractType>(obj);
        }

        private AbstractType _data;
        /// <summary>
        /// [Concrete] Data to be stored/is stored as XML.
        /// </summary>
        public AbstractType Data
        {
            get { return _data; }
            set { _data = value; }
        }

        /// <summary>
        /// **DO NOT USE** This is only added to enable XML Serialization.
        /// </summary>
        /// <remarks>DO NOT USE THIS CONSTRUCTOR</remarks>
        public AbstractProperty()
        {
            // Default Ctor (Required for Xml Serialization - DO NOT USE)
        }

        /// <summary>
        /// Initialises the Serializer to work with the given data.
        /// </summary>
        /// <param name="data">Concrete Object of the AbstractType Specified.</param>
        public AbstractProperty(AbstractType data)
        {
            _data = data;
        }

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null; // this is fine as schema is unknown.
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            // Cast the Data back from the Abstract Type.
            string typeAttrib = reader.GetAttribute("type");

            // Ensure the Type was Specified
            if (typeAttrib == null)
                throw new ArgumentNullException("Unable to Read Xml Data for Abstract Type '" + typeof(AbstractType).Name +
                    "' because no 'type' attribute was specified in the XML.");

            Type type = Type.GetType(typeAttrib);

            // Check the Type is Found.
            if (type == null)
                throw new InvalidCastException("Unable to Read Xml Data for Abstract Type '" + typeof(AbstractType).Name +
                    "' because the type specified in the XML was not found.");

            // Check the Type is a Subclass of the AbstractType.
            if (!type.IsSubclassOf(typeof(AbstractType)))
                throw new InvalidCastException("Unable to Read Xml Data for Abstract Type '" + typeof(AbstractType).Name +
                    "' because the Type specified in the XML differs ('" + type.Name + "').");

            // Read the Data, Deserializing based on the (now known) concrete type.
            reader.ReadStartElement();
            this.Data = (AbstractType)new
                XmlSerializer(type).Deserialize(reader);
            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            // Write the Type Name to the XML Element as an Attrib and Serialize
            Type type = _data.GetType();

            // BugFix: Assembly must be FQN since Types can/are external to current.
            writer.WriteAttributeString("type", type.AssemblyQualifiedName);
            new XmlSerializer(type).Serialize(writer, _data);
        }

        #endregion
    }

}
