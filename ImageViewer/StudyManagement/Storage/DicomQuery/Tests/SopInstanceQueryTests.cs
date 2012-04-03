#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca

// For information about the licensing and copyright of this software please
// contact ClearCanvas, Inc. at info@clearcanvas.ca

#endregion

#if UNIT_TESTS

using System.Linq;
using ClearCanvas.Dicom;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.DicomQuery.Tests
{
    [TestFixture]
    public class SopInstanceQueryTests : TestBase
    {
        [Test]
        public void SelectAllSops()
        {
            Study study = CreateTestStudy1();
            var sops = study.GetSeries().First().GetSopInstances().Cast<SopInstance>().ToList();

            var criteria = new DicomAttributeCollection();
            var filters = new PropertyFilters<SopInstance>(criteria);
            var results = filters.Query(sops);
            Assert.AreEqual(5, results.Count());
        }

        [Test]
        public void SelectByInstanceNumber()
        {
            Study study = CreateTestStudy1();
            var sops = study.GetSeries().First().GetSopInstances().Cast<SopInstance>().ToList();

            var criteria = new DicomAttributeCollection();
            criteria[DicomTags.InstanceNumber].SetInt32(0, 102);

            var filters = new PropertyFilters<SopInstance>(criteria);
            var results = filters.Query(sops);
            Assert.AreEqual(1, results.Count());

            criteria[DicomTags.InstanceNumber].SetInt32(0, 106);
            filters = new PropertyFilters<SopInstance>(criteria);

            results = filters.Query(sops);
            Assert.AreEqual(0, results.Count());
        }

        [Test]
        public void AssertUniqueKeys()
        {
            Study study = CreateTestStudy1();
            var sops = study.GetSeries().First().GetSopInstances().Cast<SopInstance>().ToList();

            var criteria = new DicomAttributeCollection();
            var filters = new PropertyFilters<SopInstance>(criteria);
            var results = filters.Query(sops);
            var converted = filters.ConvertResults(results);
            foreach (var result in converted)
            {
                Assert.AreEqual(3, result.Count);
                Assert.IsNotEmpty(result[DicomTags.StudyInstanceUid]);
                Assert.IsNotEmpty(result[DicomTags.SeriesInstanceUid]);
                Assert.IsNotEmpty(result[DicomTags.SopInstanceUid]);
            }
        }
    }
}

#endif