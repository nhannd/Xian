using System;
using System.Xml;
using ClearCanvas.Enterprise.Common;
using NHibernate.Cfg;
using System.IO;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
	/// <summary>
	/// Serializes/de-serializes a <see cref="RelationalModelInfo"/> object to XML.
	/// </summary>
	public class RelationalModelSerializer
	{
		/// <summary>
		/// Writes the specified model.
		/// </summary>
		public void WriteModel(RelationalModelInfo model, TextWriter tw)
		{
			using (XmlTextWriter writer = new XmlTextWriter(tw))
			{
				writer.Formatting = Formatting.Indented;
				Write(writer, model);
			}
 		}

		/// <summary>
		/// Reads a model from the specified reader.
		/// </summary>
		/// <param name="tr"></param>
		/// <returns></returns>
		public RelationalModelInfo ReadModel(TextReader tr)
		{
			using (XmlTextReader reader = new XmlTextReader(tr))
			{
				reader.WhitespaceHandling = WhitespaceHandling.None;
				return (RelationalModelInfo) Read(reader, typeof(RelationalModelInfo));
			}
		}


		/// <summary>
		/// Writes the specified data to the specified xml writer.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="data"></param>
		private static void Write(System.Xml.XmlWriter writer, object data)
		{
			JsmlSerializer.Serialize(writer, data, data.GetType().Name, false);
		}

		/// <summary>
		/// Reads an object of the specified class from the xml reader.
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="dataContractClass"></param>
		/// <returns></returns>
		private static object Read(XmlReader reader, Type dataContractClass)
		{
			return JsmlSerializer.Deserialize(reader, dataContractClass);
		}
	}
}
