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
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Externals.Config;

namespace ClearCanvas.ImageViewer.Externals
{
	public class ExternalCollection : ICollection<IExternal>, IXmlSerializable
	{
		private const string _concreteTypeElement = "Concrete-Type";
		private readonly List<IExternal> _definitions;

		public ExternalCollection()
		{
			_definitions = new List<IExternal>();
		}

		public ExternalCollection(IEnumerable<IExternal> defintions)
		{
			_definitions = new List<IExternal>(defintions);
		}

		#region ICollection<IExternalLauncher> Members

		public void Add(IExternal item)
		{
			_definitions.Add(item);
		}

		public void Clear()
		{
			_definitions.Clear();
		}

		public bool Contains(IExternal item)
		{
			return _definitions.Contains(item);
		}

		public void CopyTo(IExternal[] array, int arrayIndex)
		{
			_definitions.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return _definitions.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(IExternal item)
		{
			return _definitions.Remove(item);
		}

		#endregion

		#region IEnumerable<IExternalLauncher> Members

		public IEnumerator<IExternal> GetEnumerator()
		{
			return _definitions.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion

		public void Sort()
		{
			_definitions.Sort((x, y) => string.Compare(x.Label, y.Label, StringComparison.CurrentCultureIgnoreCase));
		}

		public string Serialize()
		{
			return SerializeXml(this);
		}

		public static ExternalCollection Deserialize(string data)
		{
			return DeserializeXml(data, typeof (ExternalCollection)) as ExternalCollection;
		}

		private static ExternalCollection _savedExternals = null;

		public static ExternalCollection SavedExternals
		{
			get
			{
				if (_savedExternals == null)
				{
					try
					{
						ReloadSavedExternals();
					}
					catch(Exception ex)
					{
						Platform.Log(LogLevel.Error, ex, "Error encountered while deserializing External Application definitions. The XML may be corrupt.");
					}
				}
				return _savedExternals;
			}
		}

		public static void ReloadSavedExternals()
		{
			try
			{
				ExternalsConfigurationSettings settings = ExternalsConfigurationSettings.Default;
				_savedExternals = Deserialize(settings.Externals);
				if (_savedExternals == null)
					_savedExternals = new ExternalCollection();
			}
			catch (Exception)
			{
				// make sure that the SavedExternals property is never simply null
				_savedExternals = new ExternalCollection();
				throw;
			}
		}

		#region IXmlSerializable Members

		XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == typeof(ExternalCollection).Name)
			{
				List<IExternal> list = new List<IExternal>();
				if (reader.ReadToDescendant(typeof(IExternal).Name))
				{
					string type = reader.GetAttribute(_concreteTypeElement);
					while (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == typeof(IExternal).Name)
					{
						string s = reader.ReadElementContentAsString();

						Type t = Type.GetType(type, false);
						if (t != null)
						{
							IExternal launcher = DeserializeXml(s, t) as IExternal;
							list.Add(launcher);
						}

						type = reader.GetAttribute(_concreteTypeElement);
					}
				}
				reader.Read();

				_definitions.Clear();
				_definitions.AddRange(list);
			}
		}

		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			foreach (IExternal launcher in this)
			{
				writer.WriteStartElement(typeof(IExternal).Name);
				writer.WriteAttributeString(_concreteTypeElement, launcher.GetType().FullName);
				string s = SerializeXml(launcher);
				writer.WriteCData(s);
				writer.WriteEndElement();
			}
		}

		private static string SerializeXml(object obj)
		{
			if (obj == null)
				return string.Empty;

			try
			{
				using (StringWriter writer = new StringWriter())
				{
					XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());
					xmlSerializer.Serialize(writer, obj);
					return writer.ToString();
				}
			}
			catch (Exception)
			{
				Platform.Log(LogLevel.Debug, "Failed to serialize an object of type {0} to the XML stream.", obj.GetType());
				throw;
			}
		}

		private static object DeserializeXml(string xml, Type type)
		{
			if (string.IsNullOrEmpty(xml))
				return null;

			try
			{
				using (StringReader reader = new StringReader(xml))
				{
					XmlSerializer xmlSerializer = new XmlSerializer(type);
					return xmlSerializer.Deserialize(reader);
				}
			}
			catch (Exception)
			{
				Platform.Log(LogLevel.Debug, "Failed to deserialize an object of type {0} off the XML stream.", type);
				throw;
			}
		}

		#endregion
	}
}