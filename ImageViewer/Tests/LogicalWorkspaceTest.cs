#if	UNIT_TESTS

using System;
using System.Collections;
using NUnit.Framework;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Tests
{
	[TestFixture]
	public class LogicalWorkspaceTest
	{
		DisplaySet m_DisplaySet;

		public LogicalWorkspaceTest()
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
		public void VerifyProperties()
		{
			LogicalWorkspace ws = new LogicalWorkspace(new ImageWorkspace("studyUID"));
			/*IToolManager mgr = ws as IToolManager;

			Assert.IsNotNull(mgr.CommandHistory);
			Assert.IsNotNull(mgr.CurrentMappableModalTools);
			Assert.IsNotNull(mgr.WorkspaceTools);*/
		}

		[Test]
		public void AddRemoveDisplaySets()
		{
			LogicalWorkspace ws = new LogicalWorkspace(new ImageWorkspace("studyUID"));

			DisplaySet ds1 = new DisplaySet();
			ws.DisplaySets.ItemAdded += new EventHandler<DisplaySetEventArgs>(OnDisplaySetAdded);
			ws.DisplaySets.ItemRemoved += new EventHandler<DisplaySetEventArgs>(OnDisplaySetRemoved);

			m_DisplaySet = null;
			ws.DisplaySets.Add(ds1);
			Assert.AreEqual(1, ws.DisplaySets.Count);
			Assert.AreEqual(ds1, m_DisplaySet);

			DisplaySet ds2 = new DisplaySet();
			ws.DisplaySets.Add(ds2);
			Assert.AreEqual(2, ws.DisplaySets.Count);

			Assert.AreEqual(ds1, ws.DisplaySets[0]);
			Assert.AreEqual(ds2, ws.DisplaySets[1]);

			ws.DisplaySets.Remove(ds1);
			Assert.AreEqual(1, ws.DisplaySets.Count);

			m_DisplaySet = null;
			ws.DisplaySets.Remove(ds2);
			Assert.AreEqual(0, ws.DisplaySets.Count);
			Assert.AreEqual(ds2, m_DisplaySet);
		}

		[Test]
		public void AddDuplicateDisplaySet()
		{
			LogicalWorkspace ws = new LogicalWorkspace(new ImageWorkspace("studyUID"));

			DisplaySet ds = new DisplaySet();
			ws.DisplaySets.Add(ds);
			Assert.AreEqual(1, ws.DisplaySets.Count);

			ws.DisplaySets.Add(ds);
			Assert.AreEqual(1, ws.DisplaySets.Count);
		}

		[Test]
		public void ForEachDisplaySet()
		{
			LogicalWorkspace ws = new LogicalWorkspace(new ImageWorkspace("studyUID"));

			DisplaySet ds1 = new DisplaySet();
			ws.DisplaySets.Add(ds1);

			DisplaySet ds2 = new DisplaySet();
			ws.DisplaySets.Add(ds2);

			foreach (DisplaySet ds in ws.DisplaySets)
				ds.ToString();
		}

		[Test]
		public void SelectDisplaySets()
		{
			LogicalWorkspace ws = new LogicalWorkspace(new ImageWorkspace("studyUID"));

			DisplaySet ds1 = new DisplaySet();
			ws.DisplaySets.Add(ds1);

			//ds1.SetSelected(true, SelectionType.Single);

			DisplaySet[] dsList = null; //ws.SelectedDisplaySets;
			Assert.AreEqual(1, dsList.Length);
			Assert.AreEqual(ds1, dsList[0]);

			DisplaySet ds2 = new DisplaySet();
			ws.DisplaySets.Add(ds2);

			//ds2.SetSelected(true, SelectionType.Single);
			dsList = null; //ws.SelectedDisplaySets;
			Assert.AreEqual(1, dsList.Length);
			Assert.AreEqual(ds2, dsList[0]);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void AddNullDisplaySet()
		{
			LogicalWorkspace ws = new LogicalWorkspace(new ImageWorkspace("studyUID"));
			ws.DisplaySets.Add(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void RemoveNullDisplaySet()
		{
			LogicalWorkspace ws = new LogicalWorkspace(new ImageWorkspace("studyUID"));
			ws.DisplaySets.Remove(null);
		}

		[Test]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void IndexOutOfRangeTooLow()
		{
			LogicalWorkspace ws = new LogicalWorkspace(new ImageWorkspace("studyUID"));
			Assert.AreEqual(0, ws.DisplaySets.Count);

			DisplaySet ds1 = new DisplaySet();
			ws.DisplaySets.Add(ds1);
			Assert.AreEqual(1, ws.DisplaySets.Count);

			DisplaySet ds2 = ws.DisplaySets[-1];
		}

		[Test]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void IndexOutOfRangeTooHigh()
		{
			LogicalWorkspace ws = new LogicalWorkspace(new ImageWorkspace("studyUID"));
			Assert.AreEqual(0, ws.DisplaySets.Count);

			DisplaySet ds1 = new DisplaySet();
			ws.DisplaySets.Add(ds1);
			Assert.AreEqual(1, ws.DisplaySets.Count);

			DisplaySet ds2 = ws.DisplaySets[1];
		}

		[Test]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void IndexOutOfRangeEmptyWorkspace()
		{
			LogicalWorkspace ws = new LogicalWorkspace(new ImageWorkspace("studyUID"));
			Assert.AreEqual(0, ws.DisplaySets.Count);

			DisplaySet ds2 = ws.DisplaySets[0];
		}

		private void OnDisplaySetAdded(object sender, DisplaySetEventArgs e)
		{
			m_DisplaySet = e.DisplaySet;
		}

		private void OnDisplaySetRemoved(object sender, DisplaySetEventArgs e)
		{
			m_DisplaySet = e.DisplaySet;
		}
	}
}

#endif