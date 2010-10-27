#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if	UNIT_TESTS

#pragma warning disable 1591

using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NUnit.Framework;

namespace ClearCanvas.Utilities.Manifest.Tests
{
	[TestFixture]
    public class SerializationTest
	{
	    [TestFixtureSetUp]
		public void Init()
		{
		}

		[TestFixtureTearDown]
		public void Cleanup()
		{
		}

		[Test]
		public void ProductTest()
		{
            Product theProduct = new Product
                                     {
                                         Manifest = "Manifest.xml",
                                         Name = "ClearCanvas Test",
                                         Suffix = "SP1",
                                         Version = "1.2.12011.33333"
                                     };

		    ProductManifest theManfest = new ProductManifest
		                                     {
		                                         Product = theProduct
		                                     };

		    ManifestFile theFile = new ManifestFile
		                               {
		                                   Checksum = "111",
		                                   Filename = "Test.dll",
		                                   Timestamp = DateTime.Now
		                               };

		    theManfest.Files.Add(theFile);

            ClearCanvasManifest manifest = new ClearCanvasManifest
                                               {
                                                   ProductManifest = theManfest
                                               };

		    XmlSerializer theSerializer = new XmlSerializer(typeof(ClearCanvasManifest));

            using (FileStream fs = new FileStream("ProductTest.xml", FileMode.Create))
            {
                XmlWriter writer = XmlWriter.Create(fs);
                if (writer != null)
                    theSerializer.Serialize(writer, manifest);
                fs.Flush();
                fs.Close();
            }
		}

        [Test]
        public void PackageTest()
        {
            Package package = new Package
            {
                Manifest = "PackageManifest.xml",
                Name = "ClearCanvas Test",
                Product = new Product
                               {
                                   Name = "ClearCanvas Test",
                                   Suffix = "SP1",
                                   Version = "1.2.12011.33333"
                               },
            };

            PackageManifest packageManifest = new PackageManifest
            {
                Package = package
            };

            ManifestFile theFile = new ManifestFile
            {
                Checksum = "111",
                Filename = "Test.dll",
                Timestamp = DateTime.Now
            };

            packageManifest.Files.Add(theFile);

            ClearCanvasManifest manifest = new ClearCanvasManifest
                                               {
                                                   PackageManifest = packageManifest
                                               };

            XmlSerializer theSerializer = new XmlSerializer(typeof(ClearCanvasManifest));

            using (FileStream fs = new FileStream("PackageTest.xml", FileMode.Create))
            {
                XmlWriter writer = XmlWriter.Create(fs);
                if (writer != null)
                    theSerializer.Serialize(writer, manifest);
                fs.Flush();
                fs.Close();
            }
        }
    }
}
#endif