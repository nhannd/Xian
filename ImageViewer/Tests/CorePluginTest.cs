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
			_modelPlugin = new WorkstationModel();
			_modelPlugin.Start();
		}
		
		[TestFixtureTearDown]
		public void Cleanup()
		{
			_modelPlugin.Stop();
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
			Assert.AreSame(WorkstationModel.Me, _modelPlugin);
		}

		[Test]
		public void IsStarted()
		{
			_modelPlugin.Start();
			Assert.IsTrue(_modelPlugin.Started);
		}

		[Test]
		public void IsStopped()
		{
			_modelPlugin.Stop();
			Assert.IsFalse(_modelPlugin.Started);
		}

		[Test]
		public void GetName()
		{
			bool result = (_modelPlugin.Name == "ClearCanvas.Workstation.Model");
			Assert.IsTrue(result);
		}

		[Test]
		public void GetPluginType()
		{
			bool result = (_modelPlugin.Type == Plugin.PluginType.Model);
			Assert.IsTrue(result);
		}
 */
	}
}

#endif