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

using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Externals.General;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Externals.Tests
{
	[TestFixture]
	public class ExternalCollectionTest
	{
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

		private static void AssertCommandLineExternal(CommandLineExternal expectedExternal, CommandLineExternal actualExternal, string id)
		{
			Assert.AreEqual(expectedExternal.Label, actualExternal.Label, "{0}: Wrong value for Label", id);
			Assert.AreEqual(expectedExternal.Enabled, actualExternal.Enabled, "{0}: Wrong value for Enabled", id);
			Assert.AreEqual(expectedExternal.WindowStyle, actualExternal.WindowStyle, "{0}: Wrong value for WindowStyle", id);
			Assert.AreEqual(expectedExternal.Command, actualExternal.Command, "{0}: Wrong value for Command", id);
			Assert.AreEqual(expectedExternal.WorkingDirectory, actualExternal.WorkingDirectory, "{0}: Wrong value for WorkingDirectory", id);
			Assert.AreEqual(expectedExternal.Arguments, actualExternal.Arguments, "{0}: Wrong value for Arguments", id);
			Assert.AreEqual(expectedExternal.Username, actualExternal.Username, "{0}: Wrong value for Username", id);
			Assert.AreEqual(expectedExternal.Domain, actualExternal.Domain, "{0}: Wrong value for Domain", id);
			Assert.AreEqual(expectedExternal.AllowMultiValueFields, actualExternal.AllowMultiValueFields, "{0}: Wrong value for AllowMultiValueFields", id);
			Assert.AreEqual(expectedExternal.MultiValueFieldSeparator, actualExternal.MultiValueFieldSeparator, "{0}: Wrong value for MultiValueFieldSeparator", id);
		}
	}
}

#endif