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
			m_ModelPlugin = new WorkstationModel();
			m_ModelPlugin.Start();
		}
		
		[TestFixtureTearDown]
		public void Cleanup()
		{
			m_ModelPlugin.Stop();
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
			Assert.AreSame(WorkstationModel.Me, m_ModelPlugin);
		}

		[Test]
		public void IsStarted()
		{
			m_ModelPlugin.Start();
			Assert.IsTrue(m_ModelPlugin.Started);
		}

		[Test]
		public void IsStopped()
		{
			m_ModelPlugin.Stop();
			Assert.IsFalse(m_ModelPlugin.Started);
		}

		[Test]
		public void GetName()
		{
			bool result = (m_ModelPlugin.Name == "ClearCanvas.Workstation.Model");
			Assert.IsTrue(result);
		}

		[Test]
		public void GetPluginType()
		{
			bool result = (m_ModelPlugin.Type == Plugin.PluginType.Model);
			Assert.IsTrue(result);
		}
 */
	}
}

#endif