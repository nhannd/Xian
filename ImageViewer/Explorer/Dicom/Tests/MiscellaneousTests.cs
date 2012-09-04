#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Dicom.ServiceModel.Query;
using NUnit.Framework;

#if UNIT_TESTS

namespace ClearCanvas.ImageViewer.Explorer.Dicom.Tests
{
    [TestFixture]
    public class MiscellaneousTests
    {
        [Test]
        public void TestIsOpenQuery()
        {
            var identifier = new StudyRootStudyIdentifier();
            Assert.IsTrue(identifier.IsOpenQuery());

            identifier.RetrieveAeTitle = "Test";
            identifier.InstanceAvailability = "Test";
            identifier.SpecificCharacterSet = "Test";

            Assert.IsTrue(identifier.IsOpenQuery());

            identifier.PatientId = "Test";
            Assert.IsFalse(identifier.IsOpenQuery());

            identifier.PatientId = "";
            identifier.PatientsName = "Test";
            Assert.IsFalse(identifier.IsOpenQuery());

            identifier.PatientId = "T";
            identifier.PatientsName = "T";
            Assert.IsFalse(identifier.IsOpenQuery());
        }
    }
}

#endif