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

		#region Test Edit Merged Practitioner

		[Test]
		[ExpectedException(typeof(WorkflowException))]
		public void Test_Edit_Merged_Practitioner()
		{
			var p1 = TestExternalPractitionerHelper.CreatePractitioner("A", "1");
			var p2 = TestExternalPractitionerHelper.CreatePractitioner("B", "2");
			TestExternalPractitionerHelper.SimpleMerge(p1, p2);

			// Changing the property won't change _lastEditedTime.  Must call MarkEdited
			p1.LicenseNumber = "Modified value";
			p1.MarkEdited();
		}

		#endregion

		#region Test Activate/Deactivate Merged/NotMerged Practitioner

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

		#region Test Merge Activated/Deactivated Practitioners

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

		#region Test Merge Merged Practitioners

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

		#region Test Merged Practitioner Properties

		[Test]
		public void Test_Merged_Practitioner_Basic_Properties()
		{
			var p1 = TestExternalPractitionerHelper.CreatePractitioner("A", "1");
			var p2 = TestExternalPractitionerHelper.CreatePractitioner("B", "2");
			var dest = TestExternalPractitionerHelper.SimpleMerge(p1, p2);

			Assert.AreEqual(p1.Name, dest.Name);
			Assert.AreEqual(p1.LicenseNumber, dest.LicenseNumber);
			Assert.AreEqual(p1.BillingNumber, dest.BillingNumber);
		}

		[Test]
		public void Test_Merged_Practitioner_Extended_Properties()
		{
			const string testKey = "TestKey";
			var p1 = TestExternalPractitionerHelper.CreatePractitioner("A", "1");
			var p2 = TestExternalPractitionerHelper.CreatePractitioner("B", "2");
			p1.ExtendedProperties.Add(testKey, "Test Value");

			var dest = TestExternalPractitionerHelper.SimpleMerge(p1, p2);

			Assert.AreEqual(p1.ExtendedProperties.Count, dest.ExtendedProperties.Count);
			Assert.AreEqual(p1.ExtendedProperties[testKey], dest.ExtendedProperties[testKey]);
		}

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

		[Test]
		public void Test_Merged_Default_Contact_Point()
		{
			var pA = TestExternalPractitionerHelper.CreatePractitioner("A", "1");
			var cpPA1 = TestExternalPractitionerHelper.AddContactPoint(pA, "cpPA1", "cpPA1");
			var cpPA2 = TestExternalPractitionerHelper.AddContactPoint(pA, "cpPA2", "cpPA2");
			Assert.IsTrue(cpPA1.IsDefaultContactPoint);
			Assert.IsFalse(cpPA2.IsDefaultContactPoint);

			var pB = TestExternalPractitionerHelper.CreatePractitioner("B", "2");
			var cpPB1 = TestExternalPractitionerHelper.AddContactPoint(pB, "cpPB1", "cpPB1");
			var cpPB2 = TestExternalPractitionerHelper.AddContactPoint(pB, "cpPB2", "cpPB2");
			Assert.IsTrue(cpPB1.IsDefaultContactPoint);
			Assert.IsFalse(cpPB2.IsDefaultContactPoint);

			var defaultContactPointAfterMerge = cpPB2;
			var dest = ExternalPractitioner.MergePractitioners(pA, pB,
				pA.Name, pA.LicenseNumber, pA.BillingNumber, pA.ExtendedProperties, defaultContactPointAfterMerge,
				new List<ExternalPractitionerContactPoint>(),
				new Dictionary<ExternalPractitionerContactPoint, ExternalPractitionerContactPoint>());

			Assert.AreEqual(dest.ContactPoints.Count, pA.ContactPoints.Count + pB.ContactPoints.Count);
			Assert.IsNotNull(dest.DefaultContactPoint);
			Assert.AreEqual(dest.DefaultContactPoint.Name, defaultContactPointAfterMerge.Name);
		}

		#endregion

		#region Test Merge Topology

		[Test]
		public void Test_Topology_Chain_Merge()
		{
			// Setup the basic practitioners, each with one contact point.
			var p1 = TestExternalPractitionerHelper.CreatePractitioner("A", "1");
			var p2 = TestExternalPractitionerHelper.CreatePractitioner("B", "2");
			var p3 = TestExternalPractitionerHelper.CreatePractitioner("C", "3");
			var p4 = TestExternalPractitionerHelper.CreatePractitioner("D", "4");
			var cpP1 = TestExternalPractitionerHelper.AddContactPoint(p1, "cpP1", "cpP1");
			var cpP2 = TestExternalPractitionerHelper.AddContactPoint(p2, "cpP2", "cpP2");
			var cpP3 = TestExternalPractitionerHelper.AddContactPoint(p3, "cpP3", "cpP3");
			var cpP4 = TestExternalPractitionerHelper.AddContactPoint(p4, "cpP4", "cpP4");

			// Perform Merge p1 -> p2 -> p3 -> p4 -> ultimateDest
			var p12 = TestExternalPractitionerHelper.SimpleMerge(p1, p2);
			var p123 = TestExternalPractitionerHelper.SimpleMerge(p12, p3);
			var ultimateDest = TestExternalPractitionerHelper.SimpleMerge(p123, p4);

			// Get a reference to all the contact points
			var p12_cpP1 = CollectionUtils.SelectFirst(p12.ContactPoints, cp => cp.Name == cpP1.Name);
			var p12_cpP2 = CollectionUtils.SelectFirst(p12.ContactPoints, cp => cp.Name == cpP2.Name);
			var p123_cpP1 = CollectionUtils.SelectFirst(p123.ContactPoints, cp => cp.Name == cpP1.Name);
			var p123_cpP2 = CollectionUtils.SelectFirst(p123.ContactPoints, cp => cp.Name == cpP2.Name);
			var p123_cpP3 = CollectionUtils.SelectFirst(p123.ContactPoints, cp => cp.Name == cpP3.Name);
			var dest_cpP1 = CollectionUtils.SelectFirst(ultimateDest.ContactPoints, cp => cp.Name == cpP1.Name);
			var dest_cpP2 = CollectionUtils.SelectFirst(ultimateDest.ContactPoints, cp => cp.Name == cpP2.Name);
			var dest_cpP3 = CollectionUtils.SelectFirst(ultimateDest.ContactPoints, cp => cp.Name == cpP3.Name);
			var dest_cpP4 = CollectionUtils.SelectFirst(ultimateDest.ContactPoints, cp => cp.Name == cpP4.Name);

			// Verifying all practitioners are merged into the right one
			Assert.AreEqual(p1.MergedInto, p12);
			Assert.AreEqual(p2.MergedInto, p12);
			Assert.AreEqual(p3.MergedInto, p123);
			Assert.AreEqual(p12.MergedInto, p123);
			Assert.AreEqual(p123.MergedInto, ultimateDest);
			Assert.IsNull(ultimateDest.MergedInto);

			// Verifying all practitioners are ultimately merged into the right one
			Assert.AreEqual(p1.GetUltimateMergeDestination(), ultimateDest);
			Assert.AreEqual(p2.GetUltimateMergeDestination(), ultimateDest);
			Assert.AreEqual(p3.GetUltimateMergeDestination(), ultimateDest);
			Assert.AreEqual(p4.GetUltimateMergeDestination(), ultimateDest);
			Assert.AreEqual(p12.GetUltimateMergeDestination(), ultimateDest);
			Assert.AreEqual(p123.GetUltimateMergeDestination(), ultimateDest);
			Assert.AreEqual(ultimateDest.GetUltimateMergeDestination(), ultimateDest);

			// Verifying all contact points are ultimately merged into the right one
			Assert.AreEqual(cpP1.MergedInto, p12_cpP1);
			Assert.AreEqual(cpP2.MergedInto, p12_cpP2);
			Assert.AreEqual(p12_cpP1.MergedInto, p123_cpP1);
			Assert.AreEqual(p12_cpP2.MergedInto, p123_cpP2);
			Assert.AreEqual(cpP3.MergedInto, p123_cpP3);
			Assert.AreEqual(p123_cpP1.MergedInto, dest_cpP1);
			Assert.AreEqual(p123_cpP2.MergedInto, dest_cpP2);
			Assert.AreEqual(p123_cpP3.MergedInto, dest_cpP3);
			Assert.AreEqual(cpP4.MergedInto, dest_cpP4);
			Assert.IsNull(dest_cpP1.MergedInto);
			Assert.IsNull(dest_cpP2.MergedInto);
			Assert.IsNull(dest_cpP3.MergedInto);
			Assert.IsNull(dest_cpP4.MergedInto);

			// Verifying all contact points are ultimately merged into the right one
			Assert.AreEqual(cpP1.GetUltimateMergeDestination(), dest_cpP1);
			Assert.AreEqual(cpP2.GetUltimateMergeDestination(), dest_cpP2);
			Assert.AreEqual(cpP3.GetUltimateMergeDestination(), dest_cpP3);
			Assert.AreEqual(cpP4.GetUltimateMergeDestination(), dest_cpP4);
			Assert.AreEqual(p12_cpP1.GetUltimateMergeDestination(), dest_cpP1);
			Assert.AreEqual(p12_cpP2.GetUltimateMergeDestination(), dest_cpP2);
			Assert.AreEqual(p123_cpP1.GetUltimateMergeDestination(), dest_cpP1);
			Assert.AreEqual(p123_cpP2.GetUltimateMergeDestination(), dest_cpP2);
			Assert.AreEqual(p123_cpP3.GetUltimateMergeDestination(), dest_cpP3);
			Assert.AreEqual(dest_cpP1.GetUltimateMergeDestination(), dest_cpP1);
			Assert.AreEqual(dest_cpP2.GetUltimateMergeDestination(), dest_cpP2);
			Assert.AreEqual(dest_cpP3.GetUltimateMergeDestination(), dest_cpP3);
			Assert.AreEqual(dest_cpP4.GetUltimateMergeDestination(), dest_cpP4);
		}

		[Test]
		public void Test_Topology_Binary_Merge()
		{
			// Setup the basic practitioners, each with one contact point.
			var p1 = TestExternalPractitionerHelper.CreatePractitioner("A", "1");
			var p2 = TestExternalPractitionerHelper.CreatePractitioner("B", "2");
			var p3 = TestExternalPractitionerHelper.CreatePractitioner("C", "3");
			var p4 = TestExternalPractitionerHelper.CreatePractitioner("D", "4");
			var cpP1 = TestExternalPractitionerHelper.AddContactPoint(p1, "cpP1", "cpP1");
			var cpP2 = TestExternalPractitionerHelper.AddContactPoint(p2, "cpP2", "cpP2");
			var cpP3 = TestExternalPractitionerHelper.AddContactPoint(p3, "cpP3", "cpP3");
			var cpP4 = TestExternalPractitionerHelper.AddContactPoint(p4, "cpP4", "cpP4");

			// Perform Merge p1+p2->p12, p3+p4->p34, p12+p34->ultimateDest
			var p12 = TestExternalPractitionerHelper.SimpleMerge(p1, p2);
			var p34 = TestExternalPractitionerHelper.SimpleMerge(p3, p4);
			var ultimateDest = TestExternalPractitionerHelper.SimpleMerge(p12, p34);

			// Get a reference to all the contact points
			var p12_cpP1 = CollectionUtils.SelectFirst(p12.ContactPoints, cp => cp.Name == cpP1.Name);
			var p12_cpP2 = CollectionUtils.SelectFirst(p12.ContactPoints, cp => cp.Name == cpP2.Name);
			var p34_cpP3 = CollectionUtils.SelectFirst(p34.ContactPoints, cp => cp.Name == cpP3.Name);
			var p34_cpP4 = CollectionUtils.SelectFirst(p34.ContactPoints, cp => cp.Name == cpP4.Name);
			var dest_cpP1 = CollectionUtils.SelectFirst(ultimateDest.ContactPoints, cp => cp.Name == cpP1.Name);
			var dest_cpP2 = CollectionUtils.SelectFirst(ultimateDest.ContactPoints, cp => cp.Name == cpP2.Name);
			var dest_cpP3 = CollectionUtils.SelectFirst(ultimateDest.ContactPoints, cp => cp.Name == cpP3.Name);
			var dest_cpP4 = CollectionUtils.SelectFirst(ultimateDest.ContactPoints, cp => cp.Name == cpP4.Name);

			// Verifying all practitioners are merged into the right one
			Assert.AreEqual(p1.MergedInto, p12);
			Assert.AreEqual(p2.MergedInto, p12);
			Assert.AreEqual(p3.MergedInto, p34);
			Assert.AreEqual(p4.MergedInto, p34);
			Assert.AreEqual(p12.MergedInto, ultimateDest);
			Assert.AreEqual(p34.MergedInto, ultimateDest);
			Assert.IsNull(ultimateDest.MergedInto);

			// Verifying all practitioners are ultimately merged into the right one
			Assert.AreEqual(p1.GetUltimateMergeDestination(), ultimateDest);
			Assert.AreEqual(p2.GetUltimateMergeDestination(), ultimateDest);
			Assert.AreEqual(p3.GetUltimateMergeDestination(), ultimateDest);
			Assert.AreEqual(p4.GetUltimateMergeDestination(), ultimateDest);
			Assert.AreEqual(p12.GetUltimateMergeDestination(), ultimateDest);
			Assert.AreEqual(p34.GetUltimateMergeDestination(), ultimateDest);
			Assert.AreEqual(ultimateDest.GetUltimateMergeDestination(), ultimateDest);

			// Verifying all contact points are merged into the right one
			Assert.AreEqual(cpP1.MergedInto, p12_cpP1);
			Assert.AreEqual(cpP2.MergedInto, p12_cpP2);
			Assert.AreEqual(cpP3.MergedInto, p34_cpP3);
			Assert.AreEqual(cpP4.MergedInto, p34_cpP4);
			Assert.AreEqual(p12_cpP1.MergedInto, dest_cpP1);
			Assert.AreEqual(p12_cpP2.MergedInto, dest_cpP2);
			Assert.AreEqual(p34_cpP3.MergedInto, dest_cpP3);
			Assert.AreEqual(p34_cpP4.MergedInto, dest_cpP4);
			Assert.IsNull(dest_cpP1.MergedInto);
			Assert.IsNull(dest_cpP2.MergedInto);
			Assert.IsNull(dest_cpP3.MergedInto);
			Assert.IsNull(dest_cpP4.MergedInto);

			// Verifying all contact points are ultimately merged into the right one
			Assert.AreEqual(cpP1.GetUltimateMergeDestination(), dest_cpP1);
			Assert.AreEqual(cpP2.GetUltimateMergeDestination(), dest_cpP2);
			Assert.AreEqual(cpP3.GetUltimateMergeDestination(), dest_cpP3);
			Assert.AreEqual(cpP4.GetUltimateMergeDestination(), dest_cpP4);
			Assert.AreEqual(p12_cpP1.GetUltimateMergeDestination(), dest_cpP1);
			Assert.AreEqual(p12_cpP2.GetUltimateMergeDestination(), dest_cpP2);
			Assert.AreEqual(p34_cpP3.GetUltimateMergeDestination(), dest_cpP3);
			Assert.AreEqual(p34_cpP4.GetUltimateMergeDestination(), dest_cpP4);
			Assert.AreEqual(dest_cpP1.GetUltimateMergeDestination(), dest_cpP1);
			Assert.AreEqual(dest_cpP2.GetUltimateMergeDestination(), dest_cpP2);
			Assert.AreEqual(dest_cpP3.GetUltimateMergeDestination(), dest_cpP3);
			Assert.AreEqual(dest_cpP4.GetUltimateMergeDestination(), dest_cpP4);
		}

		[Test]
		[ExpectedException(typeof(WorkflowException))]
		public void Test_Circular_Merge()
		{
			var p1 = TestExternalPractitionerHelper.CreatePractitioner("A", "1");
			var p2 = TestExternalPractitionerHelper.CreatePractitioner("B", "2");

			var p12 = TestExternalPractitionerHelper.SimpleMerge(p1, p2);
			TestExternalPractitionerHelper.SimpleMerge(p12, p1);
		}

		#endregion
	}
}
