#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using NUnit.Framework;

namespace ClearCanvas.ImageServer.Common.Utilities.Test
{
    [TestFixture]
    public class DicomNameUtilsTest
    {
        [Test]
        public void Test_TrimSpacesOption()
        {
            Assert.AreEqual(DicomNameUtils.Normalize("Ab C", DicomNameUtils.NormalizeOptions.TrimSpaces), "Ab C");
            Assert.AreEqual(DicomNameUtils.Normalize("Ab C ", DicomNameUtils.NormalizeOptions.TrimSpaces), "Ab C");
            Assert.AreEqual(DicomNameUtils.Normalize("Ab C  ", DicomNameUtils.NormalizeOptions.TrimSpaces), "Ab C");
            Assert.AreEqual(DicomNameUtils.Normalize(" Ab C  ", DicomNameUtils.NormalizeOptions.TrimSpaces), "Ab C");
            Assert.AreEqual(DicomNameUtils.Normalize("  Ab       C  ", DicomNameUtils.NormalizeOptions.TrimSpaces), "Ab C");
            

            Assert.AreEqual(DicomNameUtils.Normalize("Ab C^Ab C", DicomNameUtils.NormalizeOptions.TrimSpaces), "Ab C^Ab C");
            Assert.AreEqual(DicomNameUtils.Normalize("Ab C ^Ab C ", DicomNameUtils.NormalizeOptions.TrimSpaces), "Ab C^Ab C");
            Assert.AreEqual(DicomNameUtils.Normalize("Ab C  ^Ab C  ", DicomNameUtils.NormalizeOptions.TrimSpaces), "Ab C^Ab C");
            Assert.AreEqual(DicomNameUtils.Normalize(" Ab C  ^ Ab C  ", DicomNameUtils.NormalizeOptions.TrimSpaces), "Ab C^Ab C");
            Assert.AreEqual(DicomNameUtils.Normalize("  Ab C  ^  Ab C  ", DicomNameUtils.NormalizeOptions.TrimSpaces), "Ab C^Ab C");
            Assert.AreEqual(DicomNameUtils.Normalize("  Ab       C  ^  Ab  C  ", DicomNameUtils.NormalizeOptions.TrimSpaces), "Ab C^Ab C");

            Assert.AreEqual(DicomNameUtils.Normalize("Ab C^Ab C ^ ^ ^ ", DicomNameUtils.NormalizeOptions.TrimSpaces), "Ab C^Ab C^^^");
            Assert.AreEqual(DicomNameUtils.Normalize("Ab   C^Ab   C ^ ^ ^ ", DicomNameUtils.NormalizeOptions.TrimSpaces), "Ab C^Ab C^^^");
            
        }

        [Test]
        public void Test_TrimEmptyEndingComponentsOption()
        {
            Assert.AreEqual(DicomNameUtils.Normalize("Ab C", DicomNameUtils.NormalizeOptions.TrimEmptyEndingComponents), "Ab C");
            Assert.AreEqual(DicomNameUtils.Normalize("Ab C^", DicomNameUtils.NormalizeOptions.TrimEmptyEndingComponents), "Ab C");
            Assert.AreEqual(DicomNameUtils.Normalize("Ab C^ABC", DicomNameUtils.NormalizeOptions.TrimEmptyEndingComponents), "Ab C^ABC");
            Assert.AreEqual(DicomNameUtils.Normalize("Ab C^ABC^^", DicomNameUtils.NormalizeOptions.TrimEmptyEndingComponents), "Ab C^ABC");
            Assert.AreEqual(DicomNameUtils.Normalize("Ab C^ABC^ ^ ^ ", DicomNameUtils.NormalizeOptions.TrimEmptyEndingComponents), "Ab C^ABC");
            
        }

        [Test]
        public void Test_CombinedOptions()
        {
            Assert.AreEqual(DicomNameUtils.Normalize("   Ab    C           ^    ABC     ^         ^     ^     ", DicomNameUtils.NormalizeOptions.TrimEmptyEndingComponents | DicomNameUtils.NormalizeOptions.TrimSpaces), "Ab C^ABC");

        }
    }
}
