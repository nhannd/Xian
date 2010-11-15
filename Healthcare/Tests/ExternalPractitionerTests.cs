#region License

// Copyright (c) 2010, ClearCanvas Inc.
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

using System.Collections.Generic;
using NUnit.Framework;
using Iesi.Collections.Generic;
using ClearCanvas.Workflow;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare.Tests
{
	[TestFixture]
	public class ExternalPractitionerTests
	{
		private static class TestExternalPractitionerHelper
		{
			/// <summary>
			/// Create a simple practitioner with no contact point and properties.
			/// </summary>
			/// <remarks>
			/// The practitioner is not edited, not merged and not verified.
			/// </remarks>
			public static ExternalPractitioner CreatePractitioner(string familyName, string givenName)
			{
				var licenseNumber = familyName;
				var billingNumber = givenName;
				return new ExternalPractitioner(
					new PersonName(familyName, givenName, null, null, null, null),
					licenseNumber, billingNumber,
					false, null,
					null,
					new HashedSet<ExternalPractitionerContactPoint>(),
					new Dictionary<string, string>(),
					null);
			}

			/// <summary>
			/// Add a contact point to a practitioner with the specified name and description.
			/// </summary>
			/// <remarks>
			/// No telephone/address/emails are added to the new contact point
			/// </remarks>
			public static ExternalPractitionerContactPoint AddContactPoint(ExternalPractitioner p, string name, string description)
			{
				var isDefault = p.ContactPoints.Count == 0;
				var cp = new ExternalPractitionerContactPoint(p,
					name, description, ResultCommunicationMode.ANY, isDefault,
					new List<TelephoneNumber>(), 
					new List<Address>(), 
					new List<EmailAddress>(), null);

				p.ContactPoints.Add(cp);

				return cp;
			}

			/// <summary>
			/// Perform a simple merge of two practitioners.
			/// </summary>
			/// <remarks>
			/// P1 is the primary practitioner.  The new practitioner will ahve all info inherit from P1.
			/// No contact points are deactivated or replaced.
			/// </remarks>
			public static ExternalPractitioner SimpleMerge(ExternalPractitioner p1, ExternalPractitioner p2)
			{
				return ExternalPractitioner.MergePractitioners(p1, p2,
					p1.Name, p1.LicenseNumber, p1.BillingNumber, p1.ExtendedProperties, p1.DefaultContactPoint,
					new List<ExternalPractitionerContactPoint>(),
					new Dictionary<ExternalPractitionerContactPoint, ExternalPractitionerContactPoint>());
			}
		}

		#region Test Changing Activate/Deactivate status

		[Test]
		public void Test_Deactivate_Merged_Practitioner()
		{
			var p1 = TestExternalPractitionerHelper.CreatePractitioner("A", "1");
			var p2 = TestExternalPractitionerHelper.CreatePractitioner("B", "2");

			Assert.IsFalse(p1.IsMerged);
			Assert.IsFalse(p1.Deactivated);
			Assert.IsFalse(p2.IsMerged);
			Assert.IsFalse(p2.Deactivated);

			TestExternalPractitionerHelper.SimpleMerge(p1, p2);

			Assert.IsTrue(p1.IsMerged);
			Assert.IsTrue(p1.Deactivated);
			Assert.IsTrue(p2.IsMerged);
			Assert.IsTrue(p2.Deactivated);

			p1.MarkDeactivated(true);
			p2.MarkDeactivated(true);
		}

		[Test]
		[ExpectedException(typeof(WorkflowException))]
		public void Test_Activate_Merged_Practitioner()
		{
			var p1 = TestExternalPractitionerHelper.CreatePractitioner("A", "1");
			var p2 = TestExternalPractitionerHelper.CreatePractitioner("B", "2");

			Assert.IsFalse(p1.IsMerged);
			Assert.IsFalse(p1.Deactivated);
			Assert.IsFalse(p2.IsMerged);
			Assert.IsFalse(p2.Deactivated);

			TestExternalPractitionerHelper.SimpleMerge(p1, p2);

			Assert.IsTrue(p1.IsMerged);
			Assert.IsTrue(p1.Deactivated);
			Assert.IsTrue(p2.IsMerged);
			Assert.IsTrue(p2.Deactivated);

			p1.MarkDeactivated(false);
			p2.MarkDeactivated(false);
		}

		[Test]
		public void Test_Deactivate_NotMerged_Practitioner()
		{
			var p1 = TestExternalPractitionerHelper.CreatePractitioner("A", "1");
			Assert.IsFalse(p1.IsMerged);
			Assert.IsFalse(p1.Deactivated);

			p1.MarkDeactivated(true);
			Assert.IsTrue(p1.Deactivated);
		}

		[Test]
		public void Test_Activate_NotMerged_Practitioner()
		{
			var p1 = TestExternalPractitionerHelper.CreatePractitioner("A", "1");
			p1.MarkDeactivated(true);

			Assert.IsFalse(p1.IsMerged);
			Assert.IsTrue(p1.Deactivated);

			p1.MarkDeactivated(false);
			Assert.IsFalse(p1.Deactivated);
		}

		#endregion

		#region Test Merge Practitioners with Activate/Deactivated status

		[Test]
		public void Test_Merge_Activated_Practitioner()
		{
			var p1 = TestExternalPractitionerHelper.CreatePractitioner("A", "1");
			var p2 = TestExternalPractitionerHelper.CreatePractitioner("B", "2");

			Assert.IsFalse(p1.IsMerged);
			Assert.IsFalse(p1.Deactivated);
			Assert.IsFalse(p2.IsMerged);
			Assert.IsFalse(p2.Deactivated);

			TestExternalPractitionerHelper.SimpleMerge(p1, p2);
		}

		[Test]
		[ExpectedException(typeof(WorkflowException))]
		public void Test_Merge_One_Deactivated_Practitioners()
		{
			var p1 = TestExternalPractitionerHelper.CreatePractitioner("A", "1");
			var p2 = TestExternalPractitionerHelper.CreatePractitioner("B", "2");

			p1.MarkDeactivated(true);
			Assert.IsFalse(p1.IsMerged);
			Assert.IsTrue(p1.Deactivated);

			Assert.IsFalse(p2.IsMerged);
			Assert.IsFalse(p2.Deactivated);

			TestExternalPractitionerHelper.SimpleMerge(p1, p2);
		}

		[Test]
		[ExpectedException(typeof(WorkflowException))]
		public void Test_Merge_Two_Deactivated_Practitioners()
		{
			var p1 = TestExternalPractitionerHelper.CreatePractitioner("A", "1");
			var p2 = TestExternalPractitionerHelper.CreatePractitioner("B", "2");

			p1.MarkDeactivated(true);
			Assert.IsFalse(p1.IsMerged);
			Assert.IsTrue(p1.Deactivated);

			p2.MarkDeactivated(true);
			Assert.IsFalse(p2.IsMerged);
			Assert.IsTrue(p2.Deactivated);

			TestExternalPractitionerHelper.SimpleMerge(p1, p2);
		}

		#endregion

		#region Test Merge Practitioners with Merged status

		[Test]
		[ExpectedException(typeof(WorkflowException))]
		public void Test_Merge_One_Merged_Practitioners()
		{
			var p1 = TestExternalPractitionerHelper.CreatePractitioner("A", "1");
			var p2 = TestExternalPractitionerHelper.CreatePractitioner("B", "2");
			var p3 = TestExternalPractitionerHelper.CreatePractitioner("C", "3");

			p1.SetMergedInto(p3);
			Assert.IsTrue(p1.IsMerged);
			Assert.IsFalse(p2.IsMerged);

			TestExternalPractitionerHelper.SimpleMerge(p1, p2);
		}

		[Test]
		[ExpectedException(typeof(WorkflowException))]
		public void Test_Merge_Two_Merged_Practitioners()
		{
			var p1 = TestExternalPractitionerHelper.CreatePractitioner("A", "1");
			var p2 = TestExternalPractitionerHelper.CreatePractitioner("B", "2");
			var p3 = TestExternalPractitionerHelper.CreatePractitioner("C", "3");

			p1.SetMergedInto(p3);
			p2.SetMergedInto(p3);
			Assert.IsTrue(p1.IsMerged);
			Assert.IsTrue(p2.IsMerged);

			TestExternalPractitionerHelper.SimpleMerge(p1, p2);
		}

		#endregion

		#region Test Practitioner Properties after merging.

		[Test]
		public void Test_Merged_Practitioner_IsMerged_Property()
		{
			var p1 = TestExternalPractitionerHelper.CreatePractitioner("A", "1");
			var p2 = TestExternalPractitionerHelper.CreatePractitioner("B", "2");
			var p3 = TestExternalPractitionerHelper.SimpleMerge(p1, p2);

			Assert.IsTrue(p1.IsMerged);
			Assert.IsTrue(p2.IsMerged);
			Assert.IsFalse(p3.IsMerged);
		}

		[Test]
		public void Test_Merged_Practitioner_IsVerified_Property()
		{
			var p1 = TestExternalPractitionerHelper.CreatePractitioner("A", "1");
			var p2 = TestExternalPractitionerHelper.CreatePractitioner("B", "2");

			// Verify p1 and p2 first
			p1.MarkVerified();
			p2.MarkVerified();
			Assert.IsTrue(p1.IsVerified);
			Assert.IsTrue(p2.IsVerified);

			var p3 = TestExternalPractitionerHelper.SimpleMerge(p1, p2);

			// Now test for verified status
			Assert.IsFalse(p1.IsVerified);
			Assert.IsFalse(p2.IsVerified);
			Assert.IsFalse(p3.IsVerified);
		}

		[Test]
		public void Test_Merged_Practitioner_LastEditedTime_Property()
		{
			var p1 = TestExternalPractitionerHelper.CreatePractitioner("A", "1");
			var p2 = TestExternalPractitionerHelper.CreatePractitioner("B", "2");

			Assert.IsNull(p1.LastEditedTime);
			Assert.IsNull(p2.LastEditedTime);

			var p3 = TestExternalPractitionerHelper.SimpleMerge(p1, p2);

			Assert.IsNotNull(p1.LastEditedTime);
			Assert.IsNotNull(p2.LastEditedTime);
			Assert.IsNotNull(p3.LastEditedTime);
		}

		#endregion

		[Test]
		public void Test_Chain_Merge()
		{
			var p1 = TestExternalPractitionerHelper.CreatePractitioner("A", "1");
			var p2 = TestExternalPractitionerHelper.CreatePractitioner("B", "2");
			var p3 = TestExternalPractitionerHelper.CreatePractitioner("C", "3");
			var p4 = TestExternalPractitionerHelper.CreatePractitioner("D", "4");

			var p12 = TestExternalPractitionerHelper.SimpleMerge(p1, p2);
			var p123 = TestExternalPractitionerHelper.SimpleMerge(p12, p3);
			var ultimateDest = TestExternalPractitionerHelper.SimpleMerge(p123, p4);

			Assert.AreEqual(ultimateDest, p1.GetUltimateMergeDestination());
			Assert.AreEqual(ultimateDest, p2.GetUltimateMergeDestination());
			Assert.AreEqual(ultimateDest, p3.GetUltimateMergeDestination());
			Assert.AreEqual(ultimateDest, p4.GetUltimateMergeDestination());
			Assert.AreEqual(ultimateDest, p12.GetUltimateMergeDestination());
			Assert.AreEqual(ultimateDest, p123.GetUltimateMergeDestination());
		}

		[Test]
		public void Test_Binary_Merge()
		{
			var p1 = TestExternalPractitionerHelper.CreatePractitioner("A", "1");
			var p2 = TestExternalPractitionerHelper.CreatePractitioner("B", "2");
			var p3 = TestExternalPractitionerHelper.CreatePractitioner("C", "3");
			var p4 = TestExternalPractitionerHelper.CreatePractitioner("D", "4");

			var p12 = TestExternalPractitionerHelper.SimpleMerge(p1, p2);
			var p34 = TestExternalPractitionerHelper.SimpleMerge(p3, p4);
			var ultimateDest = TestExternalPractitionerHelper.SimpleMerge(p12, p34);

			Assert.AreEqual(ultimateDest, p1.GetUltimateMergeDestination());
			Assert.AreEqual(ultimateDest, p2.GetUltimateMergeDestination());
			Assert.AreEqual(ultimateDest, p3.GetUltimateMergeDestination());
			Assert.AreEqual(ultimateDest, p4.GetUltimateMergeDestination());
			Assert.AreEqual(ultimateDest, p12.GetUltimateMergeDestination());
			Assert.AreEqual(ultimateDest, p34 .GetUltimateMergeDestination());
		}

		[Test]
		public void Test_Content_After_Merge()
		{
			const string testKey = "TestKey";
			var p1 = TestExternalPractitionerHelper.CreatePractitioner("A", "1");
			var p2 = TestExternalPractitionerHelper.CreatePractitioner("B", "2");
			p1.ExtendedProperties.Add(testKey, "Test Value");

			var dest = TestExternalPractitionerHelper.SimpleMerge(p1, p2);

			Assert.AreEqual(p1.Name, dest.Name);
			Assert.AreEqual(p1.LicenseNumber, dest.LicenseNumber);
			Assert.AreEqual(p1.BillingNumber, dest.BillingNumber);
			Assert.AreEqual(p1.ExtendedProperties.Count, dest.ExtendedProperties.Count);
			Assert.AreEqual(p1.ExtendedProperties[testKey], dest.ExtendedProperties[testKey]);
		}

		[Test]
		public void Test_Topology_After_Merge()
		{
			var pA = TestExternalPractitionerHelper.CreatePractitioner("A", "1");
			var cpA1 = TestExternalPractitionerHelper.AddContactPoint(pA, "cpA1", "cpA1");
			var cpA2 = TestExternalPractitionerHelper.AddContactPoint(pA, "cpA2", "cpA2");

			var pB = TestExternalPractitionerHelper.CreatePractitioner("B", "2");
			var cpB1 = TestExternalPractitionerHelper.AddContactPoint(pB, "cpB1", "cpB1");
			var cpB2 = TestExternalPractitionerHelper.AddContactPoint(pB, "cpB2", "cpB2");

			var dest = TestExternalPractitionerHelper.SimpleMerge(pA, pB);
			var destA1 = CollectionUtils.SelectFirst(dest.ContactPoints, cp => cp.Name == cpA1.Name);
			var destA2 = CollectionUtils.SelectFirst(dest.ContactPoints, cp => cp.Name == cpA2.Name);
			var destB1 = CollectionUtils.SelectFirst(dest.ContactPoints, cp => cp.Name == cpB1.Name);
			var destB2 = CollectionUtils.SelectFirst(dest.ContactPoints, cp => cp.Name == cpB2.Name);

			// Test Practitioner MergedInto
			Assert.AreEqual(dest, pA.MergedInto);
			Assert.AreEqual(dest, pB.MergedInto);

			// Test ContactPoint MergedInto
			Assert.IsNotNull(destA1);
			Assert.IsNotNull(destA2);
			Assert.IsNotNull(destB1);
			Assert.IsNotNull(destB2);
			Assert.AreEqual(destA1, cpA1.MergedInto);
			Assert.AreEqual(destA2, cpA2.MergedInto);
			Assert.AreEqual(destB1, cpB1.MergedInto);
			Assert.AreEqual(destB2, cpB2.MergedInto);
		}


		[Test]
		public void Test_Default_Contact_Point_After_Merge()
		{
			var pA = TestExternalPractitionerHelper.CreatePractitioner("A", "1");
			TestExternalPractitionerHelper.AddContactPoint(pA, "cpA1", "cpA1");
			TestExternalPractitionerHelper.AddContactPoint(pA, "cpA2", "cpA2");

			var pB = TestExternalPractitionerHelper.CreatePractitioner("B", "2");
			TestExternalPractitionerHelper.AddContactPoint(pB, "cpB1", "cpB1");
			var cpB2 = TestExternalPractitionerHelper.AddContactPoint(pB, "cpB2", "cpB2");


			var dest = ExternalPractitioner.MergePractitioners(pA, pB,
				pA.Name, pA.LicenseNumber, pA.BillingNumber, pA.ExtendedProperties, cpB2,
				new List<ExternalPractitionerContactPoint>(),
				new Dictionary<ExternalPractitionerContactPoint, ExternalPractitionerContactPoint>());

			Assert.AreEqual(dest.ContactPoints.Count, pA.ContactPoints.Count + pB.ContactPoints.Count);
			Assert.IsNotNull(dest.DefaultContactPoint);
			Assert.AreEqual(dest.DefaultContactPoint.Name, cpB2.Name);
		}


		/// Contact POint test

		//[Test]
		//public void Test_Circular_Merge()
		//{
		//    var p1 = TestExternalPractitionerHelper.CreatePractitioner("A", "1");
		//    var p2 = TestExternalPractitionerHelper.CreatePractitioner("B", "2");

		//    var p12 = TestExternalPractitionerHelper.SimpleMerge(p1, p2);
		//    TestExternalPractitionerHelper.SimpleMerge(p12, p3);
		//}

	}
}
