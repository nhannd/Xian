#if	UNIT_TESTS

using System;
using NUnit.Framework;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.Desktop.Tests
{
	[TestFixture]
	public class WorkspaceManagerTest
	{
		StubWorkspace m_Workspace;

		public WorkspaceManagerTest()
		{
		}

		[TestFixtureSetUp]
		public void Init()
		{
		}
		
		[TestFixtureTearDown]
		public void Cleanup()
		{
		}

		[Test]
		public void CreateAddRemoveWorkspace()
		{
			WorkspaceManager wm = new WorkspaceManager();
			wm.Workspaces.ItemAdded += new EventHandler<WorkspaceEventArgs>(OnWorkspaceAdded);
			wm.Workspaces.ItemRemoved += new EventHandler<WorkspaceEventArgs>(OnWorkspaceRemoved);

			m_Workspace = null;
            StubWorkspace ws = new StubWorkspace();
			wm.Workspaces.Add(ws);
			Assert.AreEqual(1, wm.Workspaces.Count);
			// Verify add event was fired
			Assert.AreEqual(ws, m_Workspace);

			m_Workspace = null;
			wm.Workspaces.Remove(ws);
			Assert.AreEqual(0, wm.Workspaces.Count);
			// Verify add event was fired
			Assert.AreEqual(ws, m_Workspace);
		}

		[Test]
		public void AddDuplicateWorkspace()
		{
			WorkspaceManager wm = new WorkspaceManager();

			//wm.RemoveAllWorkspaces();
            StubWorkspace ws = new StubWorkspace();

			wm.Workspaces.Add(ws);
			Assert.AreEqual(1, wm.Workspaces.Count);

			wm.Workspaces.Add(ws);
			Assert.AreEqual(1, wm.Workspaces.Count);
		}

		[Test]
		public void RemoveAllWorkspaces()
		{
			WorkspaceManager wm = new WorkspaceManager();

            StubWorkspace ws = new StubWorkspace();
			wm.Workspaces.Add(ws);

            ws = new StubWorkspace();
			wm.Workspaces.Add(ws);

            ws = new StubWorkspace();
			wm.Workspaces.Add(ws);

			Assert.AreEqual(3, wm.Workspaces.Count);

			//wm.RemoveAllWorkspaces();

			Assert.AreEqual(0, wm.Workspaces.Count);
		}

		[Test]
		public void ActiveWorkspace()
		{
			WorkspaceManager wm = new WorkspaceManager();

            StubWorkspace ws1 = new StubWorkspace();
			wm.Workspaces.Add(ws1);

			//wm.ActiveWorkspace = ws1;
			Assert.AreEqual(ws1, wm.ActiveWorkspace);

			wm.Workspaces.Remove(ws1);
			Assert.AreEqual(null, wm.ActiveWorkspace);

            StubWorkspace ws2 = new StubWorkspace();
			wm.Workspaces.Add(ws2);
			//wm.ActiveWorkspace = ws2;

            Workspace ws3 = new StubWorkspace();
			wm.Workspaces.Add(ws3);
			//wm.ActiveWorkspace = ws3;

			//wm.RemoveAllWorkspaces();
			Assert.AreEqual(null, wm.ActiveWorkspace);
		}

		[Test]
		public void ForEachWorkspace()
		{
			WorkspaceManager wm = new WorkspaceManager();

            StubWorkspace ws1 = new StubWorkspace();
			wm.Workspaces.Add(ws1);

            StubWorkspace ws2 = new StubWorkspace();
			wm.Workspaces.Add(ws2);

			foreach (Workspace ws in wm.Workspaces)
				ws.ToString();
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void AddNullWorkspace()
		{
			WorkspaceManager wm = new WorkspaceManager();
			wm.Workspaces.Add(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void RemoveNullWorkspace()
		{
			WorkspaceManager wm = new WorkspaceManager();
			wm.Workspaces.Remove(null);
		}

		[Test]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void IndexOutOfRangeTooLow()
		{
			WorkspaceManager wm = new WorkspaceManager();
			Assert.AreEqual(0, wm.Workspaces.Count);

            StubWorkspace ws1 = new StubWorkspace();
			wm.Workspaces.Add(ws1);
			Assert.AreEqual(1, wm.Workspaces.Count);

            StubWorkspace ws2 = wm.Workspaces[-1] as StubWorkspace;
		}

		[Test]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void IndexOutOfRangeTooHigh()
		{
			WorkspaceManager wm = new WorkspaceManager();
			Assert.AreEqual(0, wm.Workspaces.Count);

            StubWorkspace ws1 = new StubWorkspace();
			wm.Workspaces.Add(ws1);
			Assert.AreEqual(1, wm.Workspaces.Count);

            StubWorkspace ws2 = wm.Workspaces[1] as StubWorkspace;
		}

		[Test]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void IndexOutOfRangeEmptyWorkspace()
		{
			WorkspaceManager wm = new WorkspaceManager();
			Assert.AreEqual(0, wm.Workspaces.Count);

            StubWorkspace ws2 = wm.Workspaces[-1] as StubWorkspace;
		}

		private void OnWorkspaceAdded(object sender, WorkspaceEventArgs e)
		{
            m_Workspace = ((WorkspaceEventArgs)e).Workspace as StubWorkspace;
		}

		private void OnWorkspaceRemoved(object sender, WorkspaceEventArgs e)
		{
            m_Workspace = ((WorkspaceEventArgs)e).Workspace as StubWorkspace;
		}
	}
}

#endif
