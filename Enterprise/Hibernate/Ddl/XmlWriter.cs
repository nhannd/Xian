using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using NHibernate.Mapping;
using NHibernate.Cfg;
using Iesi.Collections;
using ClearCanvas.Common.Utilities;
using System.IO;
using NHibernate.Dialect;
using ClearCanvas.Enterprise.Hibernate.Ddl.Model;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
	public class XmlWriter
	{
		/// <summary>
		/// Defines the root XML tag under which data is exported.
		/// </summary>
		private readonly Configuration _config;
		private readonly Dialect _dialect;

		public XmlWriter(Configuration config, Dialect dialect)
		{
			_config = config;
			_dialect = dialect;
		}

		public void WriteModel(TextWriter tw)
		{
			using (XmlTextWriter writer = new XmlTextWriter(tw))
			{
				writer.Formatting = Formatting.Indented;

				DatabaseSchemaInfo schemaInfo = new DatabaseSchemaInfo(_config, _dialect);

				Write(writer, schemaInfo);
			}
 		}

		public DatabaseSchemaInfo ReadModel(TextReader tr)
		{
			using (XmlTextReader reader = new XmlTextReader(tr))
			{
				reader.WhitespaceHandling = WhitespaceHandling.None;
				return (DatabaseSchemaInfo) Read(reader, typeof(DatabaseSchemaInfo));
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
