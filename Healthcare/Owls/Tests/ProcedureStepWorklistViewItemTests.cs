#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if UNIT_TESTS

using NUnit.Framework;
using ClearCanvas.Healthcare.Tests;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare.Owls.Tests
{
    [TestFixture]
	public class ProcedureStepWorklistViewItemTests
	{
		class ConcreteViewItem : ProcedureStepWorklistViewItem
		{
			
		}

		[Test]
		public void Test_ViewItemUpdate_when_Move_ProcedureStep_to_new_Procedure()
		{
			var p1 = TestPatientFactory.CreatePatient("1");
			var pp1 = CollectionUtils.FirstElement(p1.Profiles);
			var v1 = TestVisitFactory.CreateVisit(p1, "1");
			var o1 = TestOrderFactory.CreateOrder(p1, v1, "1", 1, 1, true, true);
			var rp1 = CollectionUtils.FirstElement(o1.Procedures);
			var ps1 = CollectionUtils.FirstElement(rp1.ProcedureSteps);

			// initialize view item
			var viewItem = new ConcreteViewItem();
			viewItem.SetProcedureStepInfo(ps1, true);

			// verify that all other parts are initialized too
			Assert.AreEqual(ps1, viewItem.ProcedureStep.Instance);
			Assert.AreEqual(rp1, viewItem.Procedure.Instance);
			Assert.AreEqual(o1, viewItem.Order.Instance);
			Assert.AreEqual(pp1, viewItem.PatientProfile.Instance);
			Assert.AreEqual(v1, viewItem.Visit.Instance);
		}
	}
}

#endif