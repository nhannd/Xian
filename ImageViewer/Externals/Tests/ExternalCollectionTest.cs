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

#if	UNIT_TESTS
#pragma warning disable 1591,0419,1574,1587

using System;
using System.Diagnostics;
using System.Xml;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Externals.General;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Externals.Tests
{
	[TestFixture]
	public class ExternalCollectionTest
	{
		[Test]
		public void TestNullXmlSerialization()
		{
			var xmlData = ExternalCollection.Serialize(null);
			try
			{
				Assert.IsEmpty(xmlData, "Serialization of null should produce empty string output");
			}
			catch (Exception)
			{
				Trace.WriteLine("XML Data Dump");
				Trace.WriteLine(xmlData);
				throw;
			}
		}

		[Test]
		public void TestEmptyXmlSerialization()
		{
			var xmlData = ExternalCollection.Serialize(new ExternalCollection());
			try
			{
				var xmlDoc = LoadXml(xmlData);
				AssertXmlNodeEmpty(xmlDoc, "/ExternalCollection", "Serialization of empty collection should produce empty element output");
			}
			catch (Exception)
			{
				Trace.WriteLine("XML Data Dump");
				Trace.WriteLine(xmlData);
				throw;
			}
		}

		[Test]
		public void TestXmlSerialization()
		{
			var external1 = new CommandLineExternal {Name = "external1", Label = "Label1", Enabled = true, WindowStyle = WindowStyle.Normal, Command = @"C:\Temp\CommandA.cmd", WorkingDirectory = "$DIRECTORY$", Arguments = "", Username = "", Domain = "", AllowMultiValueFields = false};
			var external2 = new CommandLineExternal {Name = "external2", Label = "Label2", Enabled = false, WindowStyle = WindowStyle.Hidden, Command = @"\ComputerA\ShareB\CommandC.cmd", WorkingDirectory = "$DIRECTORY$", Arguments = "\"$FILENAMEONLY$\"", Username = "\u5305\u9752\u5929", Domain = "\u958B\u5C01\u5E9C", AllowMultiValueFields = true, MultiValueFieldSeparator = "\" \""};
			var external3 = new CommandLineExternal {Name = "external3"};
			var xmlData = ExternalCollection.Serialize(new ExternalCollection {external1, external2, external3});
			try
			{
				var xmlDoc = LoadXml(xmlData);
				AssertXmlNodeValue(typeof (CommandLineExternal).AssemblyQualifiedName, xmlDoc, "/ExternalCollection/IExternal[1]/@Concrete-Type", "Concrete-Type should be assembly qualified type name");

				var xmlExternal1 = LoadXml(xmlDoc.SelectSingleNode("/ExternalCollection/IExternal[1]").InnerText);
				AssertXmlNodeValue("external1", xmlExternal1, "/CommandLineExternal/Name", "external1");
				AssertXmlNodeValue("Label1", xmlExternal1, "/CommandLineExternal/Label", "external1");
				AssertXmlNodeValue("true", xmlExternal1, "/CommandLineExternal/Enabled", "external1");
				AssertXmlNodeValue("Normal", xmlExternal1, "/CommandLineExternal/WindowStyle", "external1");
				AssertXmlNodeValue("C:\\Temp\\CommandA.cmd", xmlExternal1, "/CommandLineExternal/Command", "external1");
				AssertXmlNodeValue("$DIRECTORY$", xmlExternal1, "/CommandLineExternal/WorkingDirectory", "external1");
				AssertXmlNodeEmpty(xmlExternal1, "/CommandLineExternal/Arguments", "external1");
				AssertXmlNodeEmpty(xmlExternal1, "/CommandLineExternal/Username", "external1");
				AssertXmlNodeEmpty(xmlExternal1, "/CommandLineExternal/Domain", "external1");
				AssertXmlNodeValue("false", xmlExternal1, "/CommandLineExternal/AllowMultiValueFields", "external1");
				AssertXmlNodeEmpty(xmlExternal1, "/CommandLineExternal/MultiValueFieldSeparator", "external1");

				var xmlExternal2 = LoadXml(xmlDoc.SelectSingleNode("/ExternalCollection/IExternal[2]").InnerText);
				AssertXmlNodeValue("external2", xmlExternal2, "/CommandLineExternal/Name", "external2");
				AssertXmlNodeValue("Label2", xmlExternal2, "/CommandLineExternal/Label", "external2");
				AssertXmlNodeValue("false", xmlExternal2, "/CommandLineExternal/Enabled", "external2");
				AssertXmlNodeValue("Hidden", xmlExternal2, "/CommandLineExternal/WindowStyle", "external2");
				AssertXmlNodeValue("\\ComputerA\\ShareB\\CommandC.cmd", xmlExternal2, "/CommandLineExternal/Command", "external2");
				AssertXmlNodeValue("$DIRECTORY$", xmlExternal2, "/CommandLineExternal/WorkingDirectory", "external2");
				AssertXmlNodeValue("\"$FILENAMEONLY$\"", xmlExternal2, "/CommandLineExternal/Arguments", "external2");
				AssertXmlNodeValue("\u5305\u9752\u5929", xmlExternal2, "/CommandLineExternal/Username", "external2");
				AssertXmlNodeValue("\u958B\u5C01\u5E9C", xmlExternal2, "/CommandLineExternal/Domain", "external2");
				AssertXmlNodeValue("true", xmlExternal2, "/CommandLineExternal/AllowMultiValueFields", "external2");
				AssertXmlNodeValue("\" \"", xmlExternal2, "/CommandLineExternal/MultiValueFieldSeparator", "external2");

				var xmlExternal3 = LoadXml(xmlDoc.SelectSingleNode("/ExternalCollection/IExternal[3]").InnerText);
				AssertXmlNodeValue("external3", xmlExternal3, "/CommandLineExternal/Name", "external3");
				AssertXmlNodeEmpty(xmlExternal3, "/CommandLineExternal/Label", "external3");
				AssertXmlNodeValue("true", xmlExternal3, "/CommandLineExternal/Enabled", "external3");
				AssertXmlNodeValue("Normal", xmlExternal3, "/CommandLineExternal/WindowStyle", "external3");
				AssertXmlNodeEmpty(xmlExternal3, "/CommandLineExternal/Command", "external3");
				AssertXmlNodeEmpty(xmlExternal3, "/CommandLineExternal/WorkingDirectory", "external3");
				AssertXmlNodeEmpty(xmlExternal3, "/CommandLineExternal/Arguments", "external3");
				AssertXmlNodeEmpty(xmlExternal3, "/CommandLineExternal/Username", "external3");
				AssertXmlNodeEmpty(xmlExternal3, "/CommandLineExternal/Domain", "external3");
				AssertXmlNodeValue("true", xmlExternal3, "/CommandLineExternal/AllowMultiValueFields", "external3");
				AssertXmlNodeEmpty(xmlExternal3, "/CommandLineExternal/MultiValueFieldSeparator", "external3");
			}
			catch (Exception)
			{
				Trace.WriteLine("XML Data Dump");
				Trace.WriteLine(xmlData);
				throw;
			}
		}

		[Test]
		public void TestNullXmlDeserialization()
		{
			var collection = ExternalCollection.Deserialize(string.Empty);
			Assert.IsNull(collection, "Deserialization of null should produce null output");
		}

		[Test]
		public void TestEmptyElementXmlDeserialization()
		{
			const string testXml = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n"
			                       + "<ExternalCollection />";

			var collection = ExternalCollection.Deserialize(testXml);
			Assert.IsNotNull(collection, "Deserialization of empty element tag should produce empty collection");
			Assert.AreEqual(0, collection.Count, "Deserialization of empty element tag should produce empty collection");
		}

		[Test]
		public void TestEmptyCollectionXmlDeserialization()
		{
			const string testXml = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n"
			                       + "<ExternalCollection>\r\n"
			                       + "</ExternalCollection>";

			var collection2 = ExternalCollection.Deserialize(testXml);
			Assert.IsNotNull(collection2, "Deserialization of full element tag with no descendants should produce empty collection");
			Assert.AreEqual(0, collection2.Count, "Deserialization of full element tag with no descendants should produce empty collection");
		}

		[Test]
		public void TestXmlDeserialization()
		{
			// if you encounter "unspecified errors" during compile, reformat the test data - csc has a maximum line/statement length (particularly important during pdb generation)
			const string testXml = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n"
			                       + "<ExternalCollection>\r\n"
			                       + "  <IExternal Concrete-Type=\"ClearCanvas.ImageViewer.Externals.General.CommandLineExternal, ClearCanvas.ImageViewer.Externals, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\"><![CDATA[<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n"
			                       + "<CommandLineExternal xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n"
			                       + "  <Name>external1</Name>\r\n"
			                       + "  <Label>Label1</Label>\r\n"
			                       + "  <Enabled>true</Enabled>\r\n"
			                       + "  <WindowStyle>Normal</WindowStyle>\r\n"
			                       + "  <Command>C:\\Temp\\CommandA.cmd</Command>\r\n"
			                       + "  <WorkingDirectory>$DIRECTORY$</WorkingDirectory>\r\n"
			                       + "  <Arguments />\r\n"
			                       + "  <Username />\r\n"
			                       + "  <Domain />\r\n"
			                       + "  <AllowMultiValueFields>false</AllowMultiValueFields>\r\n"
			                       + "</CommandLineExternal>]]></IExternal>\r\n"
			                       + "  <IExternal Concrete-Type=\"ClearCanvas.ImageViewer.Externals.General.CommandLineExternal, ClearCanvas.ImageViewer.Externals, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\"><![CDATA[<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n"
			                       + "<CommandLineExternal xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n"
			                       + "  <Name>external2</Name>\r\n"
			                       + "  <Label>Label2</Label>\r\n"
			                       + "  <Enabled>false</Enabled>\r\n"
			                       + "  <WindowStyle>Hidden</WindowStyle>\r\n"
			                       + "  <Command>\\ComputerA\\ShareB\\CommandC.cmd</Command>\r\n"
			                       + "  <WorkingDirectory>$DIRECTORY$</WorkingDirectory>"
			                       + "  <Arguments>\"$FILENAMEONLY$\"</Arguments>"
			                       + "  <Username>\u5305\u9752\u5929</Username>"
			                       + "  <Domain>\u958B\u5C01\u5E9C</Domain>"
			                       + "  <AllowMultiValueFields>true</AllowMultiValueFields>\r\n"
			                       + "  <MultiValueFieldSeparator>\" \"</MultiValueFieldSeparator>\r\n"
			                       + "</CommandLineExternal>]]></IExternal>\r\n"
			                       + "  <IExternal Concrete-Type=\"ClearCanvas.ImageViewer.Externals.General.CommandLineExternal, ClearCanvas.ImageViewer.Externals, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\"><![CDATA[<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n"
			                       + "<CommandLineExternal xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n"
			                       + "  <Name>external3</Name>\r\n"
			                       + "</CommandLineExternal>]]></IExternal>\r\n"
			                       + "</ExternalCollection>";

			var expectedExternal1 = new CommandLineExternal {Name = "external1", Label = "Label1", Enabled = true, WindowStyle = WindowStyle.Normal, Command = @"C:\Temp\CommandA.cmd", WorkingDirectory = "$DIRECTORY$", Arguments = "", Username = "", Domain = "", AllowMultiValueFields = false};
			var expectedExternal2 = new CommandLineExternal {Name = "external2", Label = "Label2", Enabled = false, WindowStyle = WindowStyle.Hidden, Command = @"\ComputerA\ShareB\CommandC.cmd", WorkingDirectory = "$DIRECTORY$", Arguments = "\"$FILENAMEONLY$\"", Username = "\u5305\u9752\u5929", Domain = "\u958B\u5C01\u5E9C", AllowMultiValueFields = true, MultiValueFieldSeparator = "\" \""};
			var expectedExternal3 = new CommandLineExternal {Name = "external3"};

			var collection = ExternalCollection.Deserialize(testXml);
			Assert.IsNotNull(collection, "Deserialization returned null");
			Assert.AreEqual(3, collection.Count, "Deserialization returned collection with wrong number of entries");

			var external1 = CollectionUtils.SelectFirst(collection, e => e.Name == "external1");
			Assert.IsNotNull(collection, "Failed to deserialize external1");
			Assert.IsInstanceOfType(typeof (CommandLineExternal), external1, "external1: Wrong concrete implementation of IExternal");
			AssertCommandLineExternal(expectedExternal1, (CommandLineExternal) external1, "external1");

			var external2 = CollectionUtils.SelectFirst(collection, e => e.Name == "external2");
			Assert.IsNotNull(collection, "Failed to deserialize external2");
			Assert.IsInstanceOfType(typeof (CommandLineExternal), external2, "external2: Wrong concrete implementation of IExternal");
			AssertCommandLineExternal(expectedExternal2, (CommandLineExternal) external2, "external2");

			var external3 = CollectionUtils.SelectFirst(collection, e => e.Name == "external3");
			Assert.IsNotNull(collection, "Failed to deserialize external3");
			Assert.IsInstanceOfType(typeof (CommandLineExternal), external3, "external3: Wrong concrete implementation of IExternal");
			AssertCommandLineExternal(expectedExternal3, (CommandLineExternal) external3, "external3");
		}

		private static void AssertCommandLineExternal(CommandLineExternal expectedExternal, CommandLineExternal actualExternal, string message)
		{
			Assert.AreEqual(expectedExternal.Label, actualExternal.Label, "{0} (Wrong Label)", message);
			Assert.AreEqual(expectedExternal.Enabled, actualExternal.Enabled, "{0} (Wrong Enabled)", message);
			Assert.AreEqual(expectedExternal.WindowStyle, actualExternal.WindowStyle, "{0} (Wrong WindowStyle)", message);
			Assert.AreEqual(expectedExternal.Command, actualExternal.Command, "{0} (Wrong Command)", message);
			Assert.AreEqual(expectedExternal.WorkingDirectory, actualExternal.WorkingDirectory, "{0} (Wrong WorkingDirectory)", message);
			Assert.AreEqual(expectedExternal.Arguments, actualExternal.Arguments, "{0} (Wrong Arguments)", message);
			Assert.AreEqual(expectedExternal.Username, actualExternal.Username, "{0} (Wrong Username)", message);
			Assert.AreEqual(expectedExternal.Domain, actualExternal.Domain, "{0} (Wrong Domain)", message);
			Assert.AreEqual(expectedExternal.AllowMultiValueFields, actualExternal.AllowMultiValueFields, "{0} (Wrong AllowMultiValueFields)", message);
			Assert.AreEqual(expectedExternal.MultiValueFieldSeparator, actualExternal.MultiValueFieldSeparator, "{0} (Wrong MultiValueFieldSeparator)", message);
		}

		private static void AssertXmlNodeValue(string expected, XmlNode root, string xPath, string message)
		{
			var node = root.SelectSingleNode(xPath);
			Assert.IsNotNull(node, "{0} ({1} Not Found)", message, xPath);
			Assert.AreEqual(expected, node.InnerText, "{0} ({1} Wrong Value)", message, xPath);
		}

		private static void AssertXmlNodeEmpty(XmlNode root, string xPath, string message)
		{
			var node = root.SelectSingleNode(xPath);
			if (node != null && !string.IsNullOrEmpty(node.InnerText))
				Assert.AreEqual(string.Empty, node.InnerText, "{0} ({1} Should Be Empty)", message, xPath);
		}

		private static XmlDocument LoadXml(string xml)
		{
			var xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(xml);
			return xmlDocument;
		}
	}
}

#endif