#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion


#if UNIT_TESTS

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using NUnit.Framework;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare.Tests
{
	[TestFixture]
	public class OrderTests
	{
		private OrderCancelReasonEnum _unmergedCancelReason;

		public OrderTests()
		{
			_unmergedCancelReason = new OrderCancelReasonEnum("UM", "Unmerged", "The order was merged and then unmerged.");

			// set the extension factory to special test factory
			Platform.SetExtensionFactory(new TestExtensionFactory());
		}

		[Test]
		public void Test_merge_order()
		{
			var patient = TestPatientFactory.CreatePatient();
			var visit = TestVisitFactory.CreateVisit(patient);
			var facility = TestFacilityFactory.CreateFacility();
			var order1 = TestOrderFactory.CreateOrder(patient, visit, facility, "101", 2, 1, true, true);
			var order2 = TestOrderFactory.CreateOrder(patient, visit, facility, "102", 2, 1, true, true);
			var staff = TestStaffFactory.CreateStaff();

			// merge order1 into order2
			order1.Merge(new OrderMergeInfo(staff, order2));

			// order1 post conditions
			Assert.AreEqual(OrderStatus.MG, order1.Status);
			Assert.IsNotNull(order1.MergeInfo);
			Assert.AreEqual(order2, order1.MergeInfo.MergeDestinationOrder);
			Assert.AreEqual(staff, order1.MergeInfo.MergedBy);
			Assert.AreEqual(2, order1.Procedures.Count);
			Assert.IsTrue(CollectionUtils.TrueForAll(order1.Procedures, p => p.Status == ProcedureStatus.GH));

			// order2 post conditions
			Assert.AreEqual(OrderStatus.SC, order2.Status);
			Assert.IsNull(order2.MergeInfo);
			Assert.AreEqual(4, order2.Procedures.Count);
			Assert.IsTrue(CollectionUtils.TrueForAll(order1.Procedures, p => p.Status == ProcedureStatus.SC));
		}
	}
}

#endif
