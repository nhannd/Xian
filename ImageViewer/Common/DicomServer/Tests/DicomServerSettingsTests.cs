#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if UNIT_TESTS

using System.IO;
using System.Xml.Serialization;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Common.DicomServer.Tests
{
	[TestFixture]
	internal class DicomServerSettingsTests
	{
		[Test]
		public void TestImageSopClassCollectionRoundtripSerialization()
		{
			TestConfigurationElementCollectionRoundtripSerialization<ImageSopClassConfigurationElementCollection, SopClassConfigurationElement>(
				new SopClassConfigurationElement("1", "A"),
				new SopClassConfigurationElement("2", "B"),
				new SopClassConfigurationElement("3", "C"));
		}

		[Test]
		public void TestNonImageSopClassCollectionRoundtripSerialization()
		{
			TestConfigurationElementCollectionRoundtripSerialization<NonImageSopClassConfigurationElementCollection, SopClassConfigurationElement>(
				new SopClassConfigurationElement("1", "A"),
				new SopClassConfigurationElement("2", "B"),
				new SopClassConfigurationElement("3", "C"));
		}

		[Test]
		public void TestTransferSyntaxCollectionRoundtripSerialization()
		{
			TestConfigurationElementCollectionRoundtripSerialization<TransferSyntaxConfigurationElementCollection, TransferSyntaxConfigurationElement>(
				new TransferSyntaxConfigurationElement("1", "A"),
				new TransferSyntaxConfigurationElement("2", "B"),
				new TransferSyntaxConfigurationElement("3", "C"));
		}

		private static void TestConfigurationElementCollectionRoundtripSerialization<TCollection, TElement>(params TElement[] seedElements)
			where TCollection : ConfigurationElementCollection<TElement>, new()
		{
			var original = new TCollection();
			foreach (var element in seedElements)
				original.Add(element);

			string xml;
			TCollection deserialized;

			var xmlSerializer = new XmlSerializer(typeof (TCollection));
			using (var stringWriter = new StringWriter())
			{
				xmlSerializer.Serialize(stringWriter, original);
				xml = stringWriter.GetStringBuilder().ToString();
			}

			using (var stringReader = new StringReader(xml))
			{
				deserialized = (TCollection) xmlSerializer.Deserialize(stringReader);
			}

			Assert.AreEqual(original.Count, deserialized.Count, "Collection element counts differ.");
			for (var n = 0; n < original.Count; ++n)
				Assert.AreEqual(original[n], deserialized[n], "Collections differ at index {0}", n);
		}
	}
}

#endif