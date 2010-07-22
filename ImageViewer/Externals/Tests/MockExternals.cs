#region License

// Copyright (c) 2010, ClearCanvas Inc.
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

#if UNIT_TESTS

using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using ClearCanvas.ImageViewer.Externals.General;

namespace ClearCanvas.ImageViewer.Externals.Tests
{
	internal interface IMockExternal : IExternal
	{
		string Data { get; set; }
	}

	// This class must be public because of XmlSerializer
	public sealed class MockExternal : ExternalBase, IMockExternal
	{
		public string Data { get; set; }

		protected override bool CanLaunch(IArgumentHintResolver hintResolver)
		{
			throw new NotSupportedException();
		}

		protected override bool PerformLaunch(IArgumentHintResolver hintResolver)
		{
			throw new NotSupportedException();
		}

		internal sealed class ExternalFactory : ExternalFactoryBase<MockExternal>
		{
			public ExternalFactory() : base("Mock External Type") {}

			public override IExternalPropertiesComponent CreatePropertiesComponent()
			{
				throw new NotImplementedException();
			}
		}
	}

	// This class must be public because of XmlSerializer
	public sealed class MockXmlSerializableExternal : ExternalBase, IMockExternal, IXmlSerializable
	{
		public string Data { get; set; }

		public void ReadXml(XmlReader reader)
		{
			reader.MoveToContent();
			Name = reader.GetAttribute("Name");
			Label = reader.GetAttribute("Label");
			Enabled = bool.Parse(reader.GetAttribute("Enabled"));
			WindowStyle = (WindowStyle) Enum.Parse(typeof (WindowStyle), reader.GetAttribute("WindowStyle"));

			var isEmptyElement = reader.IsEmptyElement;
			reader.ReadStartElement();
			if (!isEmptyElement)
			{
				Data = reader.ReadElementString("Data");
				reader.ReadEndElement();
			}
		}

		public void WriteXml(XmlWriter writer)
		{
			writer.WriteAttributeString("Name", Name);
			writer.WriteAttributeString("Label", Label);
			writer.WriteAttributeString("Enabled", Enabled.ToString());
			writer.WriteAttributeString("WindowStyle", WindowStyle.ToString());
			writer.WriteElementString("Data", Data);
		}

		XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		protected override bool CanLaunch(IArgumentHintResolver hintResolver)
		{
			throw new NotSupportedException();
		}

		protected override bool PerformLaunch(IArgumentHintResolver hintResolver)
		{
			throw new NotSupportedException();
		}

		internal sealed class ExternalFactory : ExternalFactoryBase<MockXmlSerializableExternal>
		{
			public ExternalFactory() : base("Mock External Type With Custom XML Serialization") {}

			public override IExternalPropertiesComponent CreatePropertiesComponent()
			{
				throw new NotImplementedException();
			}
		}
	}

	// This class must be public because of XmlSerializer
	public sealed class MockBrokenExternal : ExternalBase, IMockExternal, IXmlSerializable
	{
		public string Data { get; set; }

		public void ReadXml(XmlReader reader)
		{
			try
			{
				reader.MoveToContent();
				reader.Read(); // simulate bad XML: put the reader in a broken state by advancing asymmetrically
				throw new Exception();
			}
			catch (Exception ex)
			{
				throw new XmlException("Generic XML parse exception", ex);
			}
		}

		public void WriteXml(XmlWriter writer)
		{
			// we don't have any provision in place to mitigate broken serialization code
			// it's easy to break XML accidentally, but IExternal implementations should be safer
		}

		XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		protected override bool CanLaunch(IArgumentHintResolver hintResolver)
		{
			throw new NotSupportedException();
		}

		protected override bool PerformLaunch(IArgumentHintResolver hintResolver)
		{
			throw new NotSupportedException();
		}

		internal sealed class ExternalFactory : ExternalFactoryBase<MockBrokenExternal>
		{
			public ExternalFactory() : base("Mock External Type That Simulates XML Serialization Errors") {}

			public override IExternalPropertiesComponent CreatePropertiesComponent()
			{
				throw new NotImplementedException();
			}
		}
	}
}

#endif