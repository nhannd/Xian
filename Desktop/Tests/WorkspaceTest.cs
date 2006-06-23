#if	UNIT_TESTS

using System;
using System.Collections;
using NUnit.Framework;
using ClearCanvas.Common.Tests;

namespace ClearCanvas.Desktop.Tests
{
	[TestFixture]
	public class WorkspaceTest
	{
		public WorkspaceTest()
		{
		}

		[TestFixtureSetUp]
		public void Init()
		{
			PluginSetupTeardown.PluginSetup();
		}
	
		[TestFixtureTearDown]
		public void Cleanup()
		{
			PluginSetupTeardown.PluginTeardown();
		}

		[Test]
		public void Test()
		{
			Assert.Fail();
		}
	}
}

#endif