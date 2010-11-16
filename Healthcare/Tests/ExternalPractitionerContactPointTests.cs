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

using System;
using System.Collections.Generic;
using NUnit.Framework;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare.Tests
{
	[TestFixture]
	public class ExternalPractitionerContactPointTests
	{
		internal static class TestHelper
		{
			/// <summary>
			/// Performs a simple merge of two contact points.
			/// </summary>
			/// <remarks>
			/// Destination is the primary contact point.  The result will have all info inherit from destination.
			/// All the collections are copied to the target.  The source collections will be expired.
			/// </remarks>
			public static ExternalPractitionerContactPoint SimpleMerge(ExternalPractitionerContactPoint src, ExternalPractitionerContactPoint dest)
			{
				var allPhoneNumbers = CollectionUtils.Concat(CloneCollection(dest.TelephoneNumbers), CloneCollection(src.TelephoneNumbers));
				var allAddresses = CollectionUtils.Concat(CloneCollection(dest.Addresses), CloneCollection(src.Addresses));
				var allEmailAddresses = CollectionUtils.Concat(CloneCollection(dest.EmailAddresses), CloneCollection(src.EmailAddresses));

				var result = ExternalPractitionerContactPoint.MergeContactPoints(src, dest,
					dest.Name, dest.Description, dest.PreferredResultCommunicationMode,
					allPhoneNumbers, allAddresses, allEmailAddresses);

				return result;
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
			/// Add an address to a contact point with the specified city and province.
			/// </summary>
			public static Address AddAddress(ExternalPractitionerContactPoint cp, 
				string city, string province)
			{
				var address = new Address {City = city, Province = province};
				cp.Addresses.Add(address);
				return address;
			}

			/// <summary>
			/// Add a telephone number to a contact point with the specified area code and number.
			/// </summary>
			public static TelephoneNumber AddTelephoneNumber(ExternalPractitionerContactPoint cp,
				string areaCode, string number)
			{
				var telephone = new TelephoneNumber {AreaCode = areaCode, Number = number};
				cp.TelephoneNumbers.Add(telephone);
				return telephone;
			}

			/// <summary>
			/// Add an email address to a contact point.
			/// </summary>
			public static EmailAddress AddEmailAddress(ExternalPractitionerContactPoint cp,
				string address)
			{
				var email = new EmailAddress { Address = address };
				cp.EmailAddresses.Add(email);
				return email;
			}

			/// <summary>
			/// Clones all items in the collection.
			/// </summary>
			private static ICollection<T> CloneCollection<T>(ICollection<T> items)
				where T : ICloneable
			{
				return CollectionUtils.Map(items, (T item) => (T)item.Clone());
			}
		}

		#region Basic Sanity Tests

		[Test]
		[ExpectedException(typeof(WorkflowException))]
		public void Test_Merge_ContactPoints_With_Different_Practitioners()
		{
			var p1 = ExternalPractitionerTests.TestHelper.CreatePractitioner("A", "1");
			var p2 = ExternalPractitionerTests.TestHelper.CreatePractitioner("B", "2");
			var cp1 = TestHelper.AddContactPoint(p1, "cp1", "cp1");
			var cp2 = TestHelper.AddContactPoint(p2, "cp2", "cp2");
			TestHelper.SimpleMerge(cp1, cp2);
		}

		[Test]
		[ExpectedException(typeof(WorkflowException))]
		public void Test_Merge_Same_ContactPoint()
		{
			var p1 = ExternalPractitionerTests.TestHelper.CreatePractitioner("A", "1");
			var cp1 = TestHelper.AddContactPoint(p1, "cp1", "cp1");
			Assert.IsFalse(cp1.IsMerged);
			Assert.IsFalse(cp1.Deactivated);

			TestHelper.SimpleMerge(cp1, cp1);
		}

		#endregion

		#region Test Activate/Deactivate Merged/NotMerged ContactPoint

		[Test]
		public void Test_Deactivate_Merged_ContactPoint()
		{
			var p1 = ExternalPractitionerTests.TestHelper.CreatePractitioner("A", "1");
			var cp1 = TestHelper.AddContactPoint(p1, "cp1", "cp1");
			var cp2 = TestHelper.AddContactPoint(p1, "cp2", "cp2");

			Assert.IsFalse(cp1.IsMerged);
			Assert.IsFalse(cp1.Deactivated);
			Assert.IsFalse(cp2.IsMerged);
			Assert.IsFalse(cp2.Deactivated);

			TestHelper.SimpleMerge(cp1, cp2);

			Assert.IsTrue(cp1.IsMerged);
			Assert.IsTrue(cp1.Deactivated);
			Assert.IsTrue(cp2.IsMerged);
			Assert.IsTrue(cp2.Deactivated);

			// should be a no-op
			cp1.MarkDeactivated(true);
			cp2.MarkDeactivated(true);
		}

		[Test]
		[ExpectedException(typeof(WorkflowException))]
		public void Test_Activate_Merged_ContactPoint()
		{
			var p1 = ExternalPractitionerTests.TestHelper.CreatePractitioner("A", "1");
			var cp1 = TestHelper.AddContactPoint(p1, "cp1", "cp1");
			var cp2 = TestHelper.AddContactPoint(p1, "cp2", "cp2");

			Assert.IsFalse(cp1.IsMerged);
			Assert.IsFalse(cp1.Deactivated);
			Assert.IsFalse(cp2.IsMerged);
			Assert.IsFalse(cp2.Deactivated);

			TestHelper.SimpleMerge(cp1, cp2);

			Assert.IsTrue(cp1.IsMerged);
			Assert.IsTrue(cp1.Deactivated);
			Assert.IsTrue(cp2.IsMerged);
			Assert.IsTrue(cp2.Deactivated);

			// Activated merged/deactivated cp
			cp1.MarkDeactivated(false);
			cp2.MarkDeactivated(false);
		}

		[Test]
		public void Test_Deactivate_NotMerged_ContactPoint()
		{
			var p1 = ExternalPractitionerTests.TestHelper.CreatePractitioner("A", "1");
			var cp1 = TestHelper.AddContactPoint(p1, "cp1", "cp1");
			Assert.IsFalse(cp1.IsMerged);
			Assert.IsFalse(cp1.Deactivated);

			cp1.MarkDeactivated(true);
			Assert.IsTrue(cp1.Deactivated);
		}

		[Test]
		public void Test_Activate_NotMerged_ContactPoint()
		{
			var p1 = ExternalPractitionerTests.TestHelper.CreatePractitioner("A", "1");
			var cp1 = TestHelper.AddContactPoint(p1, "cp1", "cp1");
			cp1.MarkDeactivated(true);

			Assert.IsFalse(cp1.IsMerged);
			Assert.IsTrue(cp1.Deactivated);

			cp1.MarkDeactivated(false);
			Assert.IsFalse(cp1.Deactivated);
		}

		#endregion

		#region Test Merge Activated/Deactivated ContactPoints

		[Test]
		public void Test_Merge_Activated_ContactPoints()
		{
			var p1 = ExternalPractitionerTests.TestHelper.CreatePractitioner("A", "1");
			var cp1 = TestHelper.AddContactPoint(p1, "cp1", "cp1");
			var cp2 = TestHelper.AddContactPoint(p1, "cp2", "cp2");

			Assert.IsFalse(cp1.IsMerged);
			Assert.IsFalse(cp1.Deactivated);
			Assert.IsFalse(cp2.IsMerged);
			Assert.IsFalse(cp2.Deactivated);

			TestHelper.SimpleMerge(cp1, cp2);
		}

		[Test]
		[ExpectedException(typeof(WorkflowException))]
		public void Test_Merge_Deactivated_ContactPoints_Left()
		{
			var p1 = ExternalPractitionerTests.TestHelper.CreatePractitioner("A", "1");
			var cp1 = TestHelper.AddContactPoint(p1, "cp1", "cp1");
			var cp2 = TestHelper.AddContactPoint(p1, "cp2", "cp2");

			cp1.MarkDeactivated(true);
			Assert.IsFalse(cp1.IsMerged);
			Assert.IsFalse(cp2.IsMerged);
			Assert.IsTrue(cp1.Deactivated); // Left is deactivated
			Assert.IsFalse(cp2.Deactivated);

			TestHelper.SimpleMerge(cp1, cp2);
		}

		[Test]
		[ExpectedException(typeof(WorkflowException))]
		public void Test_Merge_Deactivated_ContactPoints_Right()
		{
			var p1 = ExternalPractitionerTests.TestHelper.CreatePractitioner("A", "1");
			var cp1 = TestHelper.AddContactPoint(p1, "cp1", "cp1");
			var cp2 = TestHelper.AddContactPoint(p1, "cp2", "cp2");

			cp2.MarkDeactivated(true);
			Assert.IsFalse(cp1.IsMerged);
			Assert.IsFalse(cp1.Deactivated);
			Assert.IsFalse(cp2.IsMerged);
			Assert.IsTrue(cp2.Deactivated); // Right is deactivated

			TestHelper.SimpleMerge(cp1, cp2);
		}

		#endregion

		#region Test Merge Merged ContactPoints

		[Test]
		[ExpectedException(typeof(WorkflowException))]
		public void Test_Merge_Merged_ContactPoints_Left()
		{
			var p1 = ExternalPractitionerTests.TestHelper.CreatePractitioner("A", "1");
			var cp1 = TestHelper.AddContactPoint(p1, "cp1", "cp1");
			var cp2 = TestHelper.AddContactPoint(p1, "cp2", "cp2");
			var cp3 = TestHelper.AddContactPoint(p1, "cp3", "cp3");

			cp1.SetMergedInto(cp3);
			Assert.IsTrue(cp1.IsMerged); // Left is already merged
			Assert.IsFalse(cp2.IsMerged);

			TestHelper.SimpleMerge(cp1, cp2);
		}

		[Test]
		[ExpectedException(typeof(WorkflowException))]
		public void Test_Merge_Merged_ContactPoints_Right()
		{
			var p1 = ExternalPractitionerTests.TestHelper.CreatePractitioner("A", "1");
			var cp1 = TestHelper.AddContactPoint(p1, "cp1", "cp1");
			var cp2 = TestHelper.AddContactPoint(p1, "cp2", "cp2");
			var cp3 = TestHelper.AddContactPoint(p1, "cp3", "cp3");

			cp2.SetMergedInto(cp3);
			Assert.IsFalse(cp1.IsMerged);
			Assert.IsTrue(cp2.IsMerged); // Right is aleady merged

			TestHelper.SimpleMerge(cp1, cp2);
		}

		#endregion

		#region Test Merged ContactPoint Properties

		[Test]
		public void Test_Merged_ContactPoint_Basic_Properties()
		{
			var p1 = ExternalPractitionerTests.TestHelper.CreatePractitioner("A", "1");
			var cp1 = TestHelper.AddContactPoint(p1, "cp1", "cp1");
			var cp2 = TestHelper.AddContactPoint(p1, "cp2", "cp2");
			var result = TestHelper.SimpleMerge(cp1, cp2);

			Assert.AreEqual(result.Name, cp2.Name);
			Assert.AreEqual(result.Description, cp2.Description);
			Assert.AreEqual(result.PreferredResultCommunicationMode, cp2.PreferredResultCommunicationMode);
		}

		[Test]
		public void Test_Merged_ContactPoint_Collection_Properties()
		{
			var p1 = ExternalPractitionerTests.TestHelper.CreatePractitioner("A", "1");

			var cp1 = TestHelper.AddContactPoint(p1, "cp1", "cp1");
			var a1 = TestHelper.AddAddress(cp1, "Toronto", "ON");
			var t1 = TestHelper.AddTelephoneNumber(cp1, "416", "1111111");
			var e1 = TestHelper.AddEmailAddress(cp1, "work@clearcanvas.ca");

			var cp2 = TestHelper.AddContactPoint(p1, "cp2", "cp2");
			var a2 = TestHelper.AddAddress(cp2, "Mississauga", "ON");
			var t2 = TestHelper.AddTelephoneNumber(cp1, "905", "2222222");
			var e2 = TestHelper.AddEmailAddress(cp1, "home@clearcanvas.ca");

			// perform the merge, and get reference to all collections under result
			var result = TestHelper.SimpleMerge(cp1, cp2);
			var result_a1 = CollectionUtils.SelectFirst(result.Addresses, a => a.IsSameAddress(a1));
			var result_a2 = CollectionUtils.SelectFirst(result.Addresses, a => a.IsSameAddress(a2));
			var result_t1 = CollectionUtils.SelectFirst(result.TelephoneNumbers, t => t.IsSameNumber(t1));
			var result_t2 = CollectionUtils.SelectFirst(result.TelephoneNumbers, t => t.IsSameNumber(t2));
			var result_e1 = CollectionUtils.SelectFirst(result.EmailAddresses, e => e.IsSameEmailAddress(e1));
			var result_e2 = CollectionUtils.SelectFirst(result.EmailAddresses, e => e.IsSameEmailAddress(e2));

			// Verifying all the collection exists
			Assert.AreEqual(result.Addresses.Count, 2);
			Assert.AreEqual(result.TelephoneNumbers.Count, 2);
			Assert.AreEqual(result.EmailAddresses.Count, 2);
			Assert.IsNotNull(result_a1);
			Assert.IsNotNull(result_a2);
			Assert.IsNotNull(result_t1);
			Assert.IsNotNull(result_t2);
			Assert.IsNotNull(result_e1);
			Assert.IsNotNull(result_e2);
		}

		[Test]
		public void Test_Merged_ContactPoint_IsMerged()
		{
			var p1 = ExternalPractitionerTests.TestHelper.CreatePractitioner("A", "1");
			var cp1 = TestHelper.AddContactPoint(p1, "cp1", "cp1");
			var cp2 = TestHelper.AddContactPoint(p1, "cp2", "cp2");

			Assert.IsFalse(cp1.IsMerged);
			Assert.IsFalse(cp2.IsMerged);

			var result = TestHelper.SimpleMerge(cp1, cp2);

			Assert.IsTrue(cp1.IsMerged);
			Assert.IsTrue(cp2.IsMerged);
			Assert.IsFalse(result.IsMerged); // result is not merged
		}

		[Test]
		public void Test_Merged_ContactPoint_Deactivated()
		{
			var p1 = ExternalPractitionerTests.TestHelper.CreatePractitioner("A", "1");
			var cp1 = TestHelper.AddContactPoint(p1, "cp1", "cp1");
			var cp2 = TestHelper.AddContactPoint(p1, "cp2", "cp2");

			Assert.IsFalse(cp1.Deactivated);
			Assert.IsFalse(cp1.Deactivated);

			var result = TestHelper.SimpleMerge(cp1, cp2);

			Assert.IsTrue(cp1.Deactivated);
			Assert.IsTrue(cp2.Deactivated);
			Assert.IsFalse(result.Deactivated); // result is not deactivated
		}

		#endregion

		#region Test Merge Topology

		[Test]
		public void Test_Topology_Chain_Merge()
		{
			var p1 = ExternalPractitionerTests.TestHelper.CreatePractitioner("A", "1");
			var cp1 = TestHelper.AddContactPoint(p1, "cp1", "cp1");
			var cp2 = TestHelper.AddContactPoint(p1, "cp2", "cp2");
			var cp3 = TestHelper.AddContactPoint(p1, "cp3", "cp3");
			var cp4 = TestHelper.AddContactPoint(p1, "cp4", "cp4");

			// Perform Merge cp1 -> cp2 -> cp3 -> cp4 -> ultimateDest
			var cp12 = TestHelper.SimpleMerge(cp1, cp2);
			var cp123 = TestHelper.SimpleMerge(cp12, cp3);
			var ultimateDest = TestHelper.SimpleMerge(cp123, cp4);

			// Verifying all contact points are ultimately merged into the right one
			Assert.AreEqual(cp1.MergedInto, cp12);
			Assert.AreEqual(cp2.MergedInto, cp12);
			Assert.AreEqual(cp12.MergedInto, cp123);
			Assert.AreEqual(cp3.MergedInto, cp123);
			Assert.AreEqual(cp123.MergedInto, ultimateDest);
			Assert.AreEqual(cp4.MergedInto, ultimateDest);
			Assert.IsNull(ultimateDest.MergedInto);

			// Verifying all contact points are ultimately merged into the right one
			Assert.AreEqual(cp1.GetUltimateMergeDestination(), ultimateDest);
			Assert.AreEqual(cp2.GetUltimateMergeDestination(), ultimateDest);
			Assert.AreEqual(cp12.GetUltimateMergeDestination(), ultimateDest);
			Assert.AreEqual(cp3.GetUltimateMergeDestination(), ultimateDest);
			Assert.AreEqual(cp123.GetUltimateMergeDestination(), ultimateDest);
			Assert.AreEqual(ultimateDest.GetUltimateMergeDestination(), ultimateDest);
		}

		[Test]
		public void Test_Topology_Binary_Merge()
		{
			var p1 = ExternalPractitionerTests.TestHelper.CreatePractitioner("A", "1");
			var cp1 = TestHelper.AddContactPoint(p1, "cp1", "cp1");
			var cp2 = TestHelper.AddContactPoint(p1, "cp2", "cp2");
			var cp3 = TestHelper.AddContactPoint(p1, "cp3", "cp3");
			var cp4 = TestHelper.AddContactPoint(p1, "cp4", "cp4");

			// Perform Merge cp1+cp2->cp12, cp3+cp4->cp34, cp12+cp34->ultimateDest
			var cp12 = TestHelper.SimpleMerge(cp1, cp2);
			var cp34 = TestHelper.SimpleMerge(cp3, cp4);
			var ultimateDest = TestHelper.SimpleMerge(cp12, cp34);

			// Verifying all contact points are ultimately merged into the right one
			Assert.AreEqual(cp1.MergedInto, cp12);
			Assert.AreEqual(cp2.MergedInto, cp12);
			Assert.AreEqual(cp3.MergedInto, cp34);
			Assert.AreEqual(cp4.MergedInto, cp34);
			Assert.AreEqual(cp12.MergedInto, ultimateDest);
			Assert.AreEqual(cp34.MergedInto, ultimateDest);
			Assert.IsNull(ultimateDest.MergedInto);

			// Verifying all contact points are ultimately merged into the right one
			Assert.AreEqual(cp1.GetUltimateMergeDestination(), ultimateDest);
			Assert.AreEqual(cp2.GetUltimateMergeDestination(), ultimateDest);
			Assert.AreEqual(cp3.GetUltimateMergeDestination(), ultimateDest);
			Assert.AreEqual(cp4.GetUltimateMergeDestination(), ultimateDest);
			Assert.AreEqual(cp12.GetUltimateMergeDestination(), ultimateDest);
			Assert.AreEqual(cp34.GetUltimateMergeDestination(), ultimateDest);
			Assert.AreEqual(ultimateDest.GetUltimateMergeDestination(), ultimateDest);
		}

		[Test]
		[ExpectedException(typeof(WorkflowException))]
		public void Test_Circular_Merge()
		{
			var p1 = ExternalPractitionerTests.TestHelper.CreatePractitioner("A", "1");
			var cp1 = TestHelper.AddContactPoint(p1, "cp1", "cp1");
			var cp2 = TestHelper.AddContactPoint(p1, "cp2", "cp2");

			var cp12 = TestHelper.SimpleMerge(cp1, cp2);
			TestHelper.SimpleMerge(cp12, cp1); // Merge back with cp1
		}

		#endregion

		#region Test Merged Default/NonDefault ContactPoints

		[Test]
		public void Test_Merge_NonDefault_Contact_Points()
		{
			var p1 = ExternalPractitionerTests.TestHelper.CreatePractitioner("A", "1");
			var cp1 = TestHelper.AddContactPoint(p1, "cp1", "cp1");
			var cp2 = TestHelper.AddContactPoint(p1, "cp2", "cp2");
			var cp3 = TestHelper.AddContactPoint(p1, "cp3", "cp3");

			Assert.IsTrue(cp1.IsDefaultContactPoint);
			Assert.IsFalse(cp2.IsDefaultContactPoint);
			Assert.IsFalse(cp3.IsDefaultContactPoint);

			// Merge non-default cp
			var result = TestHelper.SimpleMerge(cp2, cp3);

			Assert.IsTrue(cp1.IsDefaultContactPoint); // cp1 remain a default
			Assert.IsFalse(cp2.IsDefaultContactPoint);
			Assert.IsFalse(cp3.IsDefaultContactPoint);
			Assert.IsFalse(result.IsDefaultContactPoint);
		}

		[Test]
		public void Test_Merge_Default_ContactPoints()
		{
			var p1 = ExternalPractitionerTests.TestHelper.CreatePractitioner("A", "1");
			var cp1 = TestHelper.AddContactPoint(p1, "cp1", "cp1");
			var cp2 = TestHelper.AddContactPoint(p1, "cp2", "cp2");
			var cp3 = TestHelper.AddContactPoint(p1, "cp3", "cp3");

			Assert.IsTrue(cp1.IsDefaultContactPoint);
			Assert.IsFalse(cp2.IsDefaultContactPoint);
			Assert.IsFalse(cp3.IsDefaultContactPoint);

			// Merge cp1, which is a default cp
			var result = TestHelper.SimpleMerge(cp1, cp2);

			Assert.IsFalse(cp1.IsDefaultContactPoint);
			Assert.IsFalse(cp2.IsDefaultContactPoint);
			Assert.IsFalse(cp3.IsDefaultContactPoint);
			Assert.IsTrue(result.IsDefaultContactPoint); // result is a default
		}


		#endregion

		#region Test Merge Effects on Practitioner

		[Test]
		public void Test_Merge_ContactPoints_Practitioner_Edited()
		{
			var p1 = ExternalPractitionerTests.TestHelper.CreatePractitioner("A", "1");
			var cp1 = TestHelper.AddContactPoint(p1, "cp1", "cp1");
			var cp2 = TestHelper.AddContactPoint(p1, "cp2", "cp2");
			Assert.IsNull(p1.LastEditedTime);

			TestHelper.SimpleMerge(cp1, cp2);

			Assert.IsNotNull(p1.LastEditedTime);
		}

		#endregion

	}
}
