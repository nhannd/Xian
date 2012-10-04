#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using NUnit.Framework;
using System;

#if UNIT_TESTS

namespace ClearCanvas.Distribution.Update.Services.Tests
{
	[TestFixture]
	public class StandardUpgradeTests
	{
		private readonly StandardUpgradeTest _test = new StandardUpgradeTest();

		[Test]
		public void TestUpgradeWorkstation_Community_15SP1To20SP1()
		{
			//real upgrade scenario, 1.5 SP1 to 2.0SP1
			Component installed = Component.CreateCommunityWorkstation(WorkstationVersions.v15SP1, true);
			Component latest = Component.CreateCommunityWorkstation(WorkstationVersions.v20SP1, true);
			Assert.AreEqual(installed.Edition, String.Empty);
			Assert.AreEqual(installed.Edition, latest.Edition);

			Assert.IsTrue(_test.Test(latest, installed));
		}

		[Test]
		public void TestUpgradeWorkstation_Community_20SP1To20SP1()
		{
			//test 2.0SP1 installed where the same version is most recent.
			Component installed = Component.CreateCommunityWorkstation(WorkstationVersions.v20SP1, true);
			Component latest = Component.CreateCommunityWorkstation(WorkstationVersions.v20SP1, true);
			Assert.AreEqual(installed.Edition, String.Empty);
			Assert.AreEqual(installed.Edition, latest.Edition);

			Assert.IsFalse(_test.Test(latest, installed));
		}

		[Test]
		public void TestUpgradeWorkstation_Community_20SP1To20SP2()
		{
			//test 2.0SP1 installed where the same version is a more recent service pack.
			Component installed = Component.CreateCommunityWorkstation(WorkstationVersions.v20SP1, true);

			const string pretendSP2Version = "2.0.13000.10000";
			Component latest = Component.CreateCommunityWorkstation(pretendSP2Version, true);
			latest.VersionSuffix = VersionSuffixes.SP2; //this value isn't actually used in the logic anywhere, so it could be excluded.
			Assert.IsTrue(_test.Test(latest, installed));

			latest = Component.CreateCommunityWorkstation(pretendSP2Version, false);
			latest.VersionSuffix = VersionSuffixes.SP2; //this value isn't actually used in the logic anywhere, so it could be excluded.
			Assert.IsTrue(_test.Test(latest, installed));
		}

		[Test]
		public void TestUpgradeWorkstation_Community_20SP1To30()
		{
			//Theoretical future community upgrade 2.0SP1 to 3.0
			Component installed = Component.CreateCommunityWorkstation(WorkstationVersions.v20SP1, true);
			//Just use the 3.0 beta version# as the latest community version.
			Component latest = Component.CreateCommunityWorkstation(WorkstationVersions.v30ClinicalOfficial, false);
			Assert.AreEqual(installed.Edition, String.Empty);
			Assert.AreNotEqual(installed.Edition, latest.Edition);
			Assert.AreEqual(latest.Edition, EditionNames.Community);

			Assert.IsTrue(_test.Test(latest, installed));
		}

		[Test]
		public void TestUpgradeWorkstation_Community20SP1To30Clinical()
		{
			Component installed = Component.CreateCommunityWorkstation(WorkstationVersions.v20SP1, true);
			//Just use the 3.0 beta version# as though it were the latest.
			Component latest = Component.CreateClinicalWorkstation(WorkstationVersions.v30ClinicalOfficial);
			Assert.AreEqual(installed.Name, latest.Name);
			Assert.AreNotEqual(installed.Edition, latest.Edition);
			Assert.AreNotEqual(installed.Version, latest.Version);
			
			Assert.IsFalse(_test.Test(latest, installed));
		}

		[Test]
		public void TestUpgradeWorkstation_30ClinicalTo20SP1Community()
		{
			Component installed = Component.CreateClinicalWorkstation(WorkstationVersions.v30ClinicalOfficial);
			Component latest = Component.CreateCommunityWorkstation(WorkstationVersions.v20SP1, true);
			Assert.AreEqual(installed.Name, latest.Name);
			Assert.AreNotEqual(installed.Edition, latest.Edition);
			Assert.AreNotEqual(installed.Version, latest.Version);

			Assert.IsFalse(_test.Test(latest, installed));
		}

		[Test]
		public void TestUpgradeWorkstation_30Clinical_BetaToOfficial()
		{
			Component installed = Component.CreateClinicalWorkstation(WorkstationVersions.v30ClinicalBeta);
			Component latest = Component.CreateClinicalWorkstation(WorkstationVersions.v30ClinicalOfficial);

			Assert.IsTrue(_test.Test(latest, installed));
		}

		[Test]
		public void TestUpgradeWrongComponent()
		{
			Component installed = Component.CreateCommunityWorkstation(WorkstationVersions.v20SP1, true);
			Component latest = Component.CreateCommunityImageServer(WorkstationVersions.v20SP1, true);
			Assert.IsFalse(_test.Test(latest, installed));
		}
	}
}

#endif