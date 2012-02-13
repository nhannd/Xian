#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if	UNIT_TESTS

using ClearCanvas.Dicom.Iod;
using NUnit.Framework;

namespace ClearCanvas.Dicom.Iod.Tests
{
	[TestFixture]
	public class ImageOrientationPatientTests
	{
		public ImageOrientationPatientTests()
		{ 
		}

		[Test]
		public void TestCosines()
		{
			ImageOrientationPatient iop = new ImageOrientationPatient(0.1, 0.2, 0.7, 0.2, 0.3, 0.5);
			Assert.AreEqual(iop.RowX, 0.1);
			Assert.AreEqual(iop.RowY, 0.2);
			Assert.AreEqual(iop.RowZ, 0.7);
			Assert.AreEqual(iop.ColumnX, 0.2);
			Assert.AreEqual(iop.ColumnY, 0.3);
			Assert.AreEqual(iop.ColumnZ, 0.5);
		}
				
		[Test]
		public void TestEdges()
		{
			// See ImageOrientationPatient class for an explanation of the logic in the tests.

			//the fact that the row/column cosines aren't orthogonal doesn't matter here, we're just testing the algorithm.
			ImageOrientationPatient iop = new ImageOrientationPatient(0, 0, 0, 0, 0, 0);
			Assert.IsTrue(iop.GetPrimaryRowDirection(false) == ImageOrientationPatient.Directions.None);
			Assert.IsTrue(iop.GetSecondaryRowDirection(false) == ImageOrientationPatient.Directions.None);

			Assert.IsTrue(iop.GetPrimaryColumnDirection(false) == ImageOrientationPatient.Directions.None);
			Assert.IsTrue(iop.GetSecondaryColumnDirection(false) == ImageOrientationPatient.Directions.None);


			iop = new ImageOrientationPatient(0.1, 0.2, 0.7, 0.2, 0.3, 0.5);

			Assert.IsTrue(iop.GetPrimaryRowDirection(false) == ImageOrientationPatient.Directions.Head);
			Assert.IsTrue(iop.GetSecondaryRowDirection(false) == ImageOrientationPatient.Directions.Posterior);
			Assert.IsTrue(iop.GetPrimaryRowDirection(true) == ImageOrientationPatient.Directions.Foot);
			Assert.IsTrue(iop.GetSecondaryRowDirection(true) == ImageOrientationPatient.Directions.Anterior);

			Assert.IsTrue(iop.GetPrimaryColumnDirection(false) == ImageOrientationPatient.Directions.Head);
			Assert.IsTrue(iop.GetSecondaryColumnDirection(false) == ImageOrientationPatient.Directions.Posterior);
			Assert.IsTrue(iop.GetPrimaryColumnDirection(true) == ImageOrientationPatient.Directions.Foot);
			Assert.IsTrue(iop.GetSecondaryColumnDirection(true) == ImageOrientationPatient.Directions.Anterior);

			iop = new ImageOrientationPatient(0.1, -0.2, -0.7, 0.2, -0.3, -0.5);

			Assert.IsTrue(iop.GetPrimaryRowDirection(false) == ImageOrientationPatient.Directions.Foot);
			Assert.IsTrue(iop.GetSecondaryRowDirection(false) == ImageOrientationPatient.Directions.Anterior);
			Assert.IsTrue(iop.GetPrimaryRowDirection(true) == ImageOrientationPatient.Directions.Head);
			Assert.IsTrue(iop.GetSecondaryRowDirection(true) == ImageOrientationPatient.Directions.Posterior);

			Assert.IsTrue(iop.GetPrimaryColumnDirection(false) == ImageOrientationPatient.Directions.Foot);
			Assert.IsTrue(iop.GetSecondaryColumnDirection(false) == ImageOrientationPatient.Directions.Anterior);
			Assert.IsTrue(iop.GetPrimaryColumnDirection(true) == ImageOrientationPatient.Directions.Head);
			Assert.IsTrue(iop.GetSecondaryColumnDirection(true) == ImageOrientationPatient.Directions.Posterior);

			iop = new ImageOrientationPatient(0.7, -0.1, -0.2, 0.5, -0.2, -0.3);

			Assert.IsTrue(iop.GetPrimaryRowDirection(false) == ImageOrientationPatient.Directions.Left);
			Assert.IsTrue(iop.GetSecondaryRowDirection(false) == ImageOrientationPatient.Directions.Foot);
			Assert.IsTrue(iop.GetPrimaryRowDirection(true) == ImageOrientationPatient.Directions.Right);
			Assert.IsTrue(iop.GetSecondaryRowDirection(true) == ImageOrientationPatient.Directions.Head);

			Assert.IsTrue(iop.GetPrimaryColumnDirection(false) == ImageOrientationPatient.Directions.Left);
			Assert.IsTrue(iop.GetSecondaryColumnDirection(false) == ImageOrientationPatient.Directions.Foot);
			Assert.IsTrue(iop.GetPrimaryColumnDirection(true) == ImageOrientationPatient.Directions.Right);
			Assert.IsTrue(iop.GetSecondaryColumnDirection(true) == ImageOrientationPatient.Directions.Head);

			iop = new ImageOrientationPatient(-0.2, -0.7, 0.1, -0.3, -0.5, -0.2);

			Assert.IsTrue(iop.GetPrimaryRowDirection(false) == ImageOrientationPatient.Directions.Anterior);
			Assert.IsTrue(iop.GetSecondaryRowDirection(false) == ImageOrientationPatient.Directions.Right);
			Assert.IsTrue(iop.GetPrimaryRowDirection(true) == ImageOrientationPatient.Directions.Posterior);
			Assert.IsTrue(iop.GetSecondaryRowDirection(true) == ImageOrientationPatient.Directions.Left);

			Assert.IsTrue(iop.GetPrimaryColumnDirection(false) == ImageOrientationPatient.Directions.Anterior);
			Assert.IsTrue(iop.GetSecondaryColumnDirection(false) == ImageOrientationPatient.Directions.Right);
			Assert.IsTrue(iop.GetPrimaryColumnDirection(true) == ImageOrientationPatient.Directions.Posterior);
			Assert.IsTrue(iop.GetSecondaryColumnDirection(true) == ImageOrientationPatient.Directions.Left);

			iop = new ImageOrientationPatient(-0.2, -0.7, 0.1, -0.3, -0.5, -0.2);

			Assert.IsTrue(iop.GetPrimaryRowDirection(false) == ImageOrientationPatient.Directions.Anterior);
			Assert.IsTrue(iop.GetSecondaryRowDirection(false) == ImageOrientationPatient.Directions.Right);
			Assert.IsTrue(iop.GetPrimaryRowDirection(true) == ImageOrientationPatient.Directions.Posterior);
			Assert.IsTrue(iop.GetSecondaryRowDirection(true) == ImageOrientationPatient.Directions.Left);

			Assert.IsTrue(iop.GetPrimaryColumnDirection(false) == ImageOrientationPatient.Directions.Anterior);
			Assert.IsTrue(iop.GetSecondaryColumnDirection(false) == ImageOrientationPatient.Directions.Right);
			Assert.IsTrue(iop.GetPrimaryColumnDirection(true) == ImageOrientationPatient.Directions.Posterior);
			Assert.IsTrue(iop.GetSecondaryColumnDirection(true) == ImageOrientationPatient.Directions.Left);

			iop = new ImageOrientationPatient(-0.2, -0.6, 0.2, -0.4, -0.3, -0.3);

			Assert.IsTrue(iop.GetPrimaryRowDirection(false) == ImageOrientationPatient.Directions.Anterior);
			Assert.IsTrue(iop.GetSecondaryRowDirection(false) == ImageOrientationPatient.Directions.Right);
			Assert.IsTrue(iop.GetPrimaryRowDirection(true) == ImageOrientationPatient.Directions.Posterior);
			Assert.IsTrue(iop.GetSecondaryRowDirection(true) == ImageOrientationPatient.Directions.Left);

			Assert.IsTrue(iop.GetPrimaryColumnDirection(false) == ImageOrientationPatient.Directions.Right);
			Assert.IsTrue(iop.GetSecondaryColumnDirection(false) == ImageOrientationPatient.Directions.Anterior);
			Assert.IsTrue(iop.GetPrimaryColumnDirection(true) == ImageOrientationPatient.Directions.Left);
			Assert.IsTrue(iop.GetSecondaryColumnDirection(true) == ImageOrientationPatient.Directions.Posterior);
		}
	}
}

#endif