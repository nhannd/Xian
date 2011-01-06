#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.ImageServer.Common.Utilities;
using NUnit.Framework;

namespace ClearCanvas.ImageServer.Core.Reconcile.Test
{
    [TestFixture]
    public class ImageReconcilerTest
    {
        [Test]
        public void Test_LookLikeSameNames()
        {


            Assert.IsTrue(DicomNameUtils.LookLikeSameNames("Selva^Siva", "Selva^Siva"), "These are different. Should return false.");

            // Both names don't have ^
            Assert.IsTrue(DicomNameUtils.LookLikeSameNames("Selva Siva", "Selva Siva"), "Identical");
            Assert.IsTrue(DicomNameUtils.LookLikeSameNames("Selva Siva", "Selva SIVA"), "letter case");
            Assert.IsTrue(DicomNameUtils.LookLikeSameNames("Selva SIVA", "Selva Siva"), "letter case");
            Assert.IsTrue(DicomNameUtils.LookLikeSameNames("Selva Siva", "   Selva  Siva  "), "Trailing/Leading Spaces");
            Assert.IsTrue(DicomNameUtils.LookLikeSameNames("   Selva  Siva  ", "Selva Siva"), "Trailing/Leading Spaces");
            
            // Only one of the names has ^
            Assert.IsTrue(DicomNameUtils.LookLikeSameNames("Selva Siva", "Selva^Siva"), "One has ^");
            Assert.IsTrue(DicomNameUtils.LookLikeSameNames("Selva^Siva", "Selva Siva"), "One has ^");

            Assert.IsTrue(DicomNameUtils.LookLikeSameNames("Selva Siva", "Selva^SIVA"), "letter case");
            Assert.IsTrue(DicomNameUtils.LookLikeSameNames("Selva^SIVA", "Selva Siva"), "letter case");

            Assert.IsTrue(DicomNameUtils.LookLikeSameNames("Selva Siva", " Selva ^ Siva "), "Trailing/Leading Spaces");
            Assert.IsTrue(DicomNameUtils.LookLikeSameNames(" Selva ^ Siva ", "Selva Siva"), "Trailing/Leading Spaces");

            Assert.IsTrue(DicomNameUtils.LookLikeSameNames("Selva Siva", " Selva ^ Siva ^ ^ ^"), "Trailing Empty Components");
            Assert.IsTrue(DicomNameUtils.LookLikeSameNames(" Selva ^ Siva ^ ^ ^", "Selva Siva"), "Trailing Empty Components");

            // Both names have ^
            Assert.IsTrue(DicomNameUtils.LookLikeSameNames("Selva^Siva", "Selva^SIVA"), "letter case");
            Assert.IsTrue(DicomNameUtils.LookLikeSameNames("Selva^SIVA", "Selva^Siva"), "letter case");

            Assert.IsTrue(DicomNameUtils.LookLikeSameNames("Selva^Siva", " Selva ^ Siva "), "Trailing/Leading Spaces");
            Assert.IsTrue(DicomNameUtils.LookLikeSameNames(" Selva ^ Siva ", "Selva^Siva"), "Trailing/Leading Spaces");

            Assert.IsTrue(DicomNameUtils.LookLikeSameNames("Selva^Siva", " Selva ^ Siva ^ ^ ^"), "Trailing Empty Components");
            Assert.IsTrue(DicomNameUtils.LookLikeSameNames(" Selva ^ Siva ^ ^ ^", "Selva^Siva"), "Trailing Empty Components");

            // Spaces in between... names are not the same
            Assert.IsFalse(DicomNameUtils.LookLikeSameNames("Selva Siva", "Selva Si va"));
            Assert.IsFalse(DicomNameUtils.LookLikeSameNames("Selva Siv a", "Selva Siva"));

            Assert.IsFalse(DicomNameUtils.LookLikeSameNames("Selva^Siva", "Selva Si va"));
            Assert.IsFalse(DicomNameUtils.LookLikeSameNames("Selva Si va", "Selva^Siva"));

            Assert.IsFalse(DicomNameUtils.LookLikeSameNames("Selva^Siva", "Selva^Si va"));
            Assert.IsFalse(DicomNameUtils.LookLikeSameNames("Selva^Si va", "Selva^Siva"));

            Assert.IsFalse(DicomNameUtils.LookLikeSameNames("Selva^Siva", "Selva^^Siva"));

            Assert.IsFalse(DicomNameUtils.LookLikeSameNames("Selva^Siva", "Selva^Siva^Jr"));
            
        }

    }
}
