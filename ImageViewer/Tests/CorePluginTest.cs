#if	UNIT_TESTS

using System;
using NUnit.Framework;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Tests
{
	[TestFixture]
	public class ModelPluginTest
	{
		public ModelPluginTest()
		{
		}
/*
		[TestFixtureSetUp]
		public void Init()
		{
			_ModelPlugin = new WorkstationModel();
			_ModelPlugin.Start();
		}
		
		[TestFixtureTearDown]
		public void Cleanup()
		{
			_ModelPlugin.Stop();
		}

		[Test]
		public void GetWorkspaceManager()
		{
			Assert.IsNotNull(WorkstationModel.WorkspaceManager);
		}

		[Test]
		public void GetMe()
		{
			Assert.IsNotNull(WorkstationModel.Me);
			Assert.AreSame(WorkstationModel.Me, _ModelPlugin);
		}

		[Test]
		public void IsStarted()
		{
			_ModelPlugin.Start();
			Assert.IsTrue(_ModelPlugin.Started);
		}

		[Test]
		public void IsStopped()
		{
			_ModelPlugin.Stop();
			Assert.IsFalse(_ModelPlugin.Started);
		}

		[Test]
		public void GetName()
		{
			bool result = (_ModelPlugin.Name == "ClearCanvas.Workstation.Model");
			Assert.IsTrue(result);
		}

		[Test]
		public void GetPluginType()
		{
			bool result = (_ModelPlugin.Type == Plugin.PluginType.Model);
			Assert.IsTrue(result);
		}
 */
	}
}

#endif