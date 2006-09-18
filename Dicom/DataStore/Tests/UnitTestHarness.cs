#if UNIT_TESTS
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using NMock2;
using Iesi.Collections;

namespace ClearCanvas.Dicom.DataStore.Tests
{
    [TestFixture]
    public class DataStoreTests
    {
        private Mockery _mocks;

        [SetUp]
        public void Setup()
        {
            _mocks = new Mockery();
        }

        [Test]
        public void CreateNewImage()
        {
            ImageSopInstance sop = DataAccessLayer.GetIDataStoreWriter().NewImageSopInstance() as ImageSopInstance;

            sop.BitsAllocated = 16;
            sop.BitsStored = 12;
            sop.Columns = 1000;
            sop.HighBit = 11;
            sop.InstanceNumber = 1;
            sop.LocationUri = new DicomUri(@"c:\temp\temp.dcm");
            sop.PhotometricInterpretation = PhotometricInterpretation.Monochrome1;
            sop.PixelRepresentation = 0;
            sop.Rows = 800;
            sop.SamplesPerPixel = 1;
            sop.SopClassUid = "1.2.840.10008.5.1.4.1.1.1";
            sop.SopInstanceUid = "1.2.840.23432234.1425132452.2134124123432";
            sop.TransferSyntaxUid = "1.2.840.10008.1.2";
            sop.PixelSpacing = new PixelSpacing(1.25, 1.25);
            sop.WindowValues.Add(new Window(1000.5, 800.3));
            sop.WindowValues.Add(new Window(2000, 1500));

            // test state before storing to database
            Assert.IsTrue("1.2.840.10008.5.1.4.1.1.1" == sop.SopClassUid);
            Assert.IsTrue("1.2.840.23432234.1425132452.2134124123432" == sop.SopInstanceUid);
            Assert.IsTrue(1000 == sop.Columns);
            Assert.IsTrue(1000.5 == (sop.WindowValues[0] as Window).Width);
            Assert.IsTrue(1500 == (sop.WindowValues[1] as Window).Center);
  
            DataAccessLayer.GetIDataStoreWriter().StoreSopInstance(sop);
            
            ImageSopInstance sop2 = DataAccessLayer.GetIDataStoreReader().GetSopInstance(new Uid(sop.SopInstanceUid)) as ImageSopInstance;

            // test state after storing to database
            Assert.IsTrue("1.2.840.10008.5.1.4.1.1.1" == sop2.SopClassUid);
            Assert.IsTrue("1.2.840.23432234.1425132452.2134124123432" == sop2.SopInstanceUid);
            Assert.IsTrue(1000 == sop2.Columns);
            Assert.IsTrue(1000.5 == (sop2.WindowValues[0] as Window).Width);
            Assert.IsTrue(1500 == (sop2.WindowValues[1] as Window).Center);
            DataAccessLayer.GetIDataStoreWriter().RemoveSopInstance(sop2);
        }

        [Test]
        public void CheckForTheExistenceOfSopInstance()
        {
            Uid sopInstanceReference = new Uid("1.2.840.23432234.1425132452.2134124123432");

            // check for existence before a new sop is created
            Assert.IsFalse(DataAccessLayer.GetIDataStoreReader().SopInstanceExists(sopInstanceReference));

            ImageSopInstance sop = new ImageSopInstance();
            sop.SopInstanceUid = sopInstanceReference.ToString();

            DataAccessLayer.GetIDataStoreWriter().StoreSopInstance(sop);

            Assert.IsTrue(DataAccessLayer.GetIDataStoreReader().SopInstanceExists(sopInstanceReference));
            DataAccessLayer.GetIDataStoreWriter().RemoveSopInstance(sop);
        }

        [Test]
        public void SeriesCreation()
        {
            Series series = new Series();
            series.SeriesDescription = "Test Series";
            series.SeriesInstanceUid = "1.2.3.4.5.6";

            ImageSopInstance sop1 = new ImageSopInstance();
            sop1.SopInstanceUid = "9.9.9.1";

            ImageSopInstance sop2 = new ImageSopInstance();
            sop2.SopInstanceUid = "9.9.9.2";

            series.AddSopInstance(sop1);
            series.AddSopInstance(sop2);

            DataAccessLayer.GetIDataStoreWriter().StoreSeries(series);
            
            Series series2 = DataAccessLayer.GetIDataStoreReader().GetSeries(new Uid("1.2.3.4.5.6")) as Series;

            int count = 0;
            foreach (ISopInstance sop in series2.GetSopInstances())
            {
                ++count;
            }

            Assert.IsTrue(2 == count);
            DataAccessLayer.GetIDataStoreWriter().RemoveSeries(series2);
        }

        [Test]
        public void AddSopsToExistingSeries()
        {          
            Series series = new Series();
            series.SeriesDescription = "Test Series";
            series.SeriesInstanceUid = "1.2.3.4.5.6";

            ImageSopInstance sop1 = new ImageSopInstance();
            sop1.SopInstanceUid = "9.9.9.1";

            ImageSopInstance sop2 = new ImageSopInstance();
            sop2.SopInstanceUid = "9.9.9.2";

            series.AddSopInstance(sop1);
            series.AddSopInstance(sop2);

            DataAccessLayer.GetIDataStoreWriter().StoreSeries(series);

            
            ISeries seriesLoaded = DataAccessLayer.GetIDataStoreReader().GetSeriesAndSopInstances(new Uid("1.2.3.4.5.6"));

            ImageSopInstance sop3 = new ImageSopInstance();
            sop3.SopInstanceUid = "9.9.9.3";

            ImageSopInstance sop4 = new ImageSopInstance();
            sop4.SopInstanceUid = "9.9.9.4";

            seriesLoaded.AddSopInstance(sop3);
            seriesLoaded.AddSopInstance(sop4);

            DataAccessLayer.GetIDataStoreWriter().StoreSeries(seriesLoaded);

            
            ISeries seriesCheck = DataAccessLayer.GetIDataStoreReader().GetSeriesAndSopInstances(new Uid("1.2.3.4.5.6"));

            int count = 0;
            foreach (ISopInstance sop in seriesCheck.GetSopInstances())
            {
                ++count;
            }

            Assert.IsTrue(4 == count);
            
            // remove the two
            seriesCheck.RemoveSopInstance(sop3);
            seriesCheck.RemoveSopInstance(sop4);
            DataAccessLayer.GetIDataStoreWriter().StoreSeries(seriesCheck);
            
            ISeries seriesRecheck = DataAccessLayer.GetIDataStoreReader().GetSeriesAndSopInstances(new Uid("1.2.3.4.5.6"));

            count = 0;
            foreach (ISopInstance sop in seriesRecheck.GetSopInstances())
            {
                ++count;
            }

            Assert.IsTrue(2 == count);
            DataAccessLayer.GetIDataStoreWriter().RemoveSeries(seriesRecheck);
        }

        [Test]
        public void StudyCreation()
        {
            Study study = new Study();
            study.StudyDescription = "Test Study";
            study.StudyInstanceUid = "3.1.4.1.5.9.2";

            Series series = new Series();
            series.SeriesDescription = "Test Series for Test Study #1";
            series.SeriesInstanceUid = "1.2.3.4.5.6.7.8";

            ImageSopInstance sop1 = new ImageSopInstance();
            sop1.SopInstanceUid = "9.9.9.1";

            ImageSopInstance sop2 = new ImageSopInstance();
            sop2.SopInstanceUid = "9.9.9.2";

            series.AddSopInstance(sop1);
            series.AddSopInstance(sop2);

            Series series2 = new Series();
            series2.SeriesDescription = "Test Series for Test Study #2";
            series2.SeriesInstanceUid = "1.2.3.4.5.6.7.8.9";

            ImageSopInstance sop3 = new ImageSopInstance();
            sop3.SopInstanceUid = "9.9.9.3";

            ImageSopInstance sop4 = new ImageSopInstance();
            sop4.SopInstanceUid = "9.9.9.4";

            ImageSopInstance sop5 = new ImageSopInstance();
            sop5.SopInstanceUid = "9.9.9.5";

            series2.AddSopInstance(sop3);
            series2.AddSopInstance(sop4);
            series2.AddSopInstance(sop5);

            study.AddSeries(series);
            study.AddSeries(series2);

            DataAccessLayer.GetIDataStoreWriter().StoreStudy(study);
            
            IStudy studyFound = DataAccessLayer.GetIDataStoreReader().GetStudy(new Uid("3.1.4.1.5.9.2"));

            int count = 0;
            foreach (ISeries seriesFound in studyFound.GetSeries())
            {
                if (seriesFound.GetSeriesInstanceUid() == "1.2.3.4.5.6.7.8")
                {
                    int count2 = 0;
                    foreach (ISopInstance sopFound in seriesFound.GetSopInstances())
                    {
                        ++count2;
                    }
                    Assert.IsTrue(2 == count2);
                }
                if (seriesFound.GetSeriesInstanceUid() == "1.2.3.4.5.6.7.8.9")
                {
                    int count2 = 0;
                    foreach (ISopInstance sopFound in seriesFound.GetSopInstances())
                    {
                        ++count2;
                    }
                    Assert.IsTrue(3 == count2);
                }
                ++count;
            }

            Assert.IsTrue(2 == count);
            DataAccessLayer.GetIDataStoreWriter().RemoveStudy(studyFound);
        }

        [Ignore]
        [Test]
        public void TestDictionary()
        {
            DicomDictionaryContainer container = new DicomDictionaryContainer();
            DictionaryEntry entry1 = new DictionaryEntry();
            entry1.TagName = new TagName("TestA");
            entry1.Path = new Path("Path/Path/Path1");
            entry1.IsComputed = false;
            DictionaryEntry entry2 = new DictionaryEntry();
            entry2.TagName = new TagName("TestB");
            entry2.Path = new Path("Path/Path/Path2");
            entry2.IsComputed = false;
            container.DictionaryEntries.Add(entry1);
            container.DictionaryEntries.Add(entry2);

            DataAccessLayer.GetIDataStoreWriter().StoreDictionary(container);
        }
    }
}
#endif