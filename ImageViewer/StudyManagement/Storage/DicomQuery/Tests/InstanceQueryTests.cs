#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca

// For information about the licensing and copyright of this software please
// contact ClearCanvas, Inc. at info@clearcanvas.ca

#endregion

#if UNIT_TESTS

using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Tests;
using ClearCanvas.Dicom.Utilities.Xml;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.DicomQuery.Tests
{
    [TestFixture]
    public class InstanceQueryTests : TestBase
    {
        [Test]
        public void SelectAllSops()
        {
            Study study = CreateTestStudy1();
            var criteria = new DicomAttributeCollection();
            var filters = new PropertyFilters<Series>(criteria);
            var results = filters.FilterResults(study.GetSeries().Cast<Series>());
            Assert.AreEqual(4, results.Count());
        }

    }
}

#endif