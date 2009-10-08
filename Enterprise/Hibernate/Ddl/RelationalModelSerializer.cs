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
using System.Xml;
using ClearCanvas.Enterprise.Common;
using NHibernate.Cfg;
using System.IO;
using System.Reflection;

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
				var model = (RelationalModelInfo) Read(reader, typeof(RelationalModelInfo));

				// bug #5300: need to convert any unique flags to explicit unique constraints
				MakeUniqueConstraintsExplicit(model);
				return model;
			}
		}

		/// <summary>
		/// Writes the specified data to the specified xml writer.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="data"></param>
		private static void Write(System.Xml.XmlWriter writer, object data)
		{
			// bug #5300: do not write out the "Unique" flag anymore
			var options = new JsmlSerializer.SerializeOptions { MemberFilter = (m => m.Name != "Unique") };
			JsmlSerializer.Serialize(writer, data, data.GetType().Name, options);
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

		/// <summary>
		/// Adds an explicit unique constraint for each column that is marked as unique.
		/// </summary>
		/// <remarks>
		/// This is to support backwards compatability with prior versions, where
		/// the Unique flag was set to indicate that a column had a unique constraint.
		/// </remarks>
		/// <param name="model"></param>
		private static void MakeUniqueConstraintsExplicit(RelationalModelInfo model)
		{
			foreach (var table in model.Tables)
			{
				// explicitly model any unique columns as unique constraints
				foreach (var col in table.Columns)
				{
					if (col.Unique)
					{
						table.UniqueKeys.Add(new ConstraintInfo(table, col));
					}
				}
			}
		}

	}
}
