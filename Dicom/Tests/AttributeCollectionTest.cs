#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#pragma warning disable 1591, 0168

#if UNIT_TESTS

using NUnit.Framework;


namespace ClearCanvas.Dicom.Tests
{
	[TestFixture]
	public class AttributeCollectionTest : AbstractTest
	{
        [Test]
        public void TestFile()
        {
            string filename = "OutOfRange.dcm";

            DicomFile file = new DicomFile(filename, new DicomAttributeCollection(), DicomAttributeCollection.ToProvider(new DicomAttributeCollection()));
            SetupMR(file.DataSet);
            SetupMetaInfo(file);

            DicomTag tag = new DicomTag(0x00030010, "Test", "TestBad", DicomVr.LOvr, false, 1, 1, false);

            file.DataSet[tag].SetStringValue("Test");

            file.Save(filename);

            file = new DicomFile(filename);

            file.DataSet.IgnoreOutOfRangeTags = true;

            file.Load();

            Assert.IsNotNull(file.DataSet.GetAttribute(tag));

            file = new DicomFile(filename);

            file.DataSet.IgnoreOutOfRangeTags = false;

            try
            {
                file.Load();
                Assert.Fail("file.Load failed");
            }
            catch (DicomException)
            {
            }
        }

	    [Test]
		public void TestIgnoreOutOfRangeTags()
		{
			DicomFile file = new DicomFile();
			DicomAttributeCollection collection = file.DataSet;

			SetupMR(collection);
			collection.IgnoreOutOfRangeTags = true;

			DicomTag tag = new DicomTag(0x00030010, "Test", "TestBad", DicomVr.STvr, false, 1, 1, false);

			Assert.IsNotNull(collection.GetAttribute(tag));
			Assert.IsNotNull(collection[tag]);
	
			try
			{
				DicomAttribute attrib = collection[tag.TagValue];
				Assert.Fail("DicomAttributeCollection.GetAttribute failed");
			}
			catch (DicomException)
			{
			}

			collection.IgnoreOutOfRangeTags = false;

			try
			{
				collection.GetAttribute(tag);
				Assert.Fail("DicomAttributeCollection.GetAttribute failed");
			}
			catch (DicomException)
			{
			}

			try
			{
				DicomAttribute attrib = collection[tag];
				Assert.Fail("DicomAttributeCollection.GetAttribute failed");
			}
			catch (DicomException)
			{
			}

			try
			{
				DicomAttribute attrib = collection[tag.TagValue];
				Assert.Fail("DicomAttributeCollection.GetAttribute failed");
			}
			catch (DicomException)
			{
			}
		}
	}
}

#endif