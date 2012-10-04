#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if UNIT_TESTS

using System.Collections.Generic;
using NUnit.Framework;

namespace ClearCanvas.Distribution.Update.Services.Tests
{
	[TestFixture]
	public class ComponentUpgradeTests
	{
		[Test]
		public void SameEditionTest()
		{
			Component installed = Component.CreateCommunityWorkstation(WorkstationVersions.v15SP1, true);
			Component latest = Component.CreateCommunityWorkstation(WorkstationVersions.v20SP1, true);
			Assert.IsTrue(new IsSameEditionTest().Test(latest, installed));

			installed = Component.CreateCommunityWorkstation(WorkstationVersions.v15SP1, false);
			latest = Component.CreateCommunityWorkstation(WorkstationVersions.v20SP1, false);
			Assert.IsTrue(new IsSameEditionTest().Test(latest, installed));

			installed = Component.CreateCommunityWorkstation(WorkstationVersions.v15SP1, true);
			latest = Component.CreateCommunityWorkstation(WorkstationVersions.v20SP1, false);
			Assert.IsTrue(new IsSameEditionTest().Test(latest, installed));

			installed = Component.CreateCommunityWorkstation(WorkstationVersions.v15SP1, true);
			latest = Component.CreateClinicalWorkstation(WorkstationVersions.v30ClinicalOfficial);
			Assert.IsFalse(new IsSameEditionTest().Test(latest, installed));
		}

		[Test]
		public void CommunityEditionTest()
		{
			Component installed = Component.CreateCommunityWorkstation(WorkstationVersions.v15SP1, true);
			Assert.IsTrue(new IsCommunityEditionTest().Test(installed));

			installed = Component.CreateCommunityWorkstation(WorkstationVersions.v15SP1, false);
			Assert.IsTrue(new IsCommunityEditionTest().Test(installed));

			installed = Component.CreateClinicalWorkstation(WorkstationVersions.v30ClinicalOfficial);
			Assert.IsFalse(new IsCommunityEditionTest().Test(installed));
		}

		[Test]
		public void OrTest()
		{
			var test = new OrTest { InnerTests = new List<ComponentUpgradeTest>() {new TrueTest(), new FalseTest()} };
			Assert.IsTrue(test.Test(null, null));
			test = new OrTest { InnerTests = new List<ComponentUpgradeTest>() { new FalseTest(), new FalseTest() } };
			Assert.IsFalse(test.Test(null, null));
			test = new OrTest { InnerTests = new List<ComponentUpgradeTest>() { new TrueTest() } };
			Assert.IsTrue(test.Test(null, null));
		}

		[Test]
		public void AndTest()
		{
			var test = new AndTest { InnerTests = new List<ComponentUpgradeTest>() { new TrueTest(), new TrueTest() } };
			Assert.IsTrue(test.Test(null, null));
			test = new AndTest { InnerTests = new List<ComponentUpgradeTest>() { new TrueTest(), new FalseTest() } };
			Assert.IsFalse(test.Test(null, null));
			test = new AndTest { InnerTests = new List<ComponentUpgradeTest>() { new FalseTest() } };
			Assert.IsFalse(test.Test(null, null));
		}

		[Test]
		public void NotTest()
		{
			var test = new NotTest() { InnerTest = new FalseTest()};
			Assert.IsTrue(test.Test(null, null));
			test = new NotTest() { InnerTest = new TrueTest() };
			Assert.IsFalse(test.Test(null, null));
		}


		[Test]
		public void IsPropertyEqualTest()
		{
			const string value = "Test";
			var test = new IsPropertyEqualTest() {Property = "Edition", Value = value};
			var component = new Component {Edition = value};
			Assert.IsTrue(test.Test(null, component));

			component.Edition = value + "1";
			Assert.IsFalse(test.Test(null, component));
		}
	}
}

#endif