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
		StubWorkspace _workspace;

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
			WorkspaceManager wm = new WorkspaceManager(new DesktopWindow());
			wm.Workspaces.ItemAdded += new EventHandler<WorkspaceEventArgs>(OnWorkspaceAdded);
			wm.Workspaces.ItemRemoved += new EventHandler<WorkspaceEventArgs>(OnWorkspaceRemoved);

			_workspace = null;
            StubWorkspace ws = new StubWorkspace();
			wm.Workspaces.Add(ws);
			Assert.AreEqual(1, wm.Workspaces.Count);
			// Verify add event was fired
			Assert.AreEqual(ws, _workspace);

			_workspace = null;
			wm.Workspaces.Remove(ws);
			Assert.AreEqual(0, wm.Workspaces.Count);
			// Verify add event was fired
			Assert.AreEqual(ws, _workspace);
		}

		[Test]
		public void AddDuplicateWorkspace()
		{
			WorkspaceManager wm = new WorkspaceManager(new DesktopWindow());

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
			WorkspaceManager wm = new WorkspaceManager(new DesktopWindow());

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
			WorkspaceManager wm = new WorkspaceManager(new DesktopWindow());

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
			WorkspaceManager wm = new WorkspaceManager(new DesktopWindow());

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
			WorkspaceManager wm = new WorkspaceManager(new DesktopWindow());
			wm.Workspaces.Add(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void RemoveNullWorkspace()
		{
			WorkspaceManager wm = new WorkspaceManager(new DesktopWindow());
			wm.Workspaces.Remove(null);
		}

		[Test]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void IndexOutOfRangeTooLow()
		{
			WorkspaceManager wm = new WorkspaceManager(new DesktopWindow());
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
			WorkspaceManager wm = new WorkspaceManager(new DesktopWindow());
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
			WorkspaceManager wm = new WorkspaceManager(new DesktopWindow());
			Assert.AreEqual(0, wm.Workspaces.Count);

            StubWorkspace ws2 = wm.Workspaces[-1] as StubWorkspace;
		}

		private void OnWorkspaceAdded(object sender, WorkspaceEventArgs e)
		{
            _workspace = ((WorkspaceEventArgs)e).Workspace as StubWorkspace;
		}

		private void OnWorkspaceRemoved(object sender, WorkspaceEventArgs e)
		{
            _workspace = ((WorkspaceEventArgs)e).Workspace as StubWorkspace;
		}
	}
}

#endif
