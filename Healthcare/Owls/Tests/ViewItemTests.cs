#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if UNIT_TESTS

using ClearCanvas.Enterprise.Core.Modelling;
using NUnit.Framework;
using ClearCanvas.Healthcare.Tests;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare.Owls.Tests
{
    [TestFixture]
	public class ViewItemTests
	{
		class ConcreteViewItem : WorklistViewItemBase
		{
			
		}

		[Test]
		public void Test_validation_disabled()
		{
			Assert.IsFalse(DomainObjectValidator.IsValidationEnabled(typeof(WorklistViewItemBase)));
			Assert.IsFalse(DomainObjectValidator.IsValidationEnabled(typeof(ConcreteViewItem)));
		}


        [Test]
        public void Test_ViewItemUpdate_when_Move_Order_to_new_Visit_and_Patient()
		{
        	var p1 = TestPatientFactory.CreatePatient("1");
        	var pp1 = CollectionUtils.FirstElement(p1.Profiles);
        	var v1 = TestVisitFactory.CreateVisit(p1, "1");
        	var o1 = TestOrderFactory.CreateOrder(p1, v1, "1", 1, 1, true, true);

			// initialize view item
        	var viewItem = new ConcreteViewItem();
			viewItem.SetOrderInfo(o1, true);

			Assert.AreEqual(o1, viewItem.Order.Instance);
			Assert.AreEqual(pp1, viewItem.PatientProfile.Instance);
			Assert.AreEqual(v1, viewItem.Visit.Instance);

			// move the order to another visit and patient
			var p2 = TestPatientFactory.CreatePatient("2");
			var pp2 = CollectionUtils.FirstElement(p2.Profiles);
			var v2 = TestVisitFactory.CreateVisit(p2, "2");
        	o1.Patient = p2;
        	o1.Visit = v2;

			// update the order part of the view item
			viewItem.SetOrderInfo(o1, true);

			// verify that the visit and patient parts were updated too
			Assert.AreEqual(o1, viewItem.Order.Instance);
			Assert.AreEqual(pp2, viewItem.PatientProfile.Instance);
			Assert.AreEqual(v2, viewItem.Visit.Instance);
		}

		[Test]
		public void Test_ViewItemUpdate_when_Move_Procedure_to_new_Order()
		{
			var p1 = TestPatientFactory.CreatePatient("1");
			var pp1 = CollectionUtils.FirstElement(p1.Profiles);
			var v1 = TestVisitFactory.CreateVisit(p1, "1");
			var o1 = TestOrderFactory.CreateOrder(p1, v1, "1", 1, 1, true, true);
			var rp1 = CollectionUtils.FirstElement(o1.Procedures);

			// initialize view item
			var viewItem = new ConcreteViewItem();
			viewItem.SetProcedureInfo(rp1, true);

			Assert.AreEqual(rp1, viewItem.Procedure.Instance);
			Assert.AreEqual(o1, viewItem.Order.Instance);
			Assert.AreEqual(pp1, viewItem.PatientProfile.Instance);
			Assert.AreEqual(v1, viewItem.Visit.Instance);

			// move the procedure to another order
			var p2 = TestPatientFactory.CreatePatient("2");
			var pp2 = CollectionUtils.FirstElement(p2.Profiles);
			var v2 = TestVisitFactory.CreateVisit(p2, "2");
			var o2 = TestOrderFactory.CreateOrder(p2, v2, "2", 1, 1, true, true);
			o1.RemoveProcedure(rp1);
			o2.AddProcedure(rp1);

			// update the procedure part of the view item and direct references
			viewItem.SetProcedureInfo(rp1, true);

			// verify that the order, visit and patient parts were updated too
			Assert.AreEqual(rp1, viewItem.Procedure.Instance);
			Assert.AreEqual(o2, viewItem.Order.Instance);
			Assert.AreEqual(pp2, viewItem.PatientProfile.Instance);
			Assert.AreEqual(v2, viewItem.Visit.Instance);
		}
	}
}

#endif