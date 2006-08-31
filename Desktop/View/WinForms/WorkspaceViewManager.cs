using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using Crownwood.DotNetMagic.Common;
using Crownwood.DotNetMagic.Docking;
using Crownwood.DotNetMagic.Controls;

namespace ClearCanvas.Desktop.View.WinForms
{
	class WorkspaceViewManager
	{
		private DesktopForm _desktopForm;
		private TabbedGroups _tabbedGroups;

		public WorkspaceViewManager(DesktopForm desktopForm, TabbedGroups tabbedGroups)
		{
			Platform.CheckForNullReference(desktopForm, "desktopForm");
			Platform.CheckForNullReference(tabbedGroups, "tabbedGroups");

			_desktopForm = desktopForm;
            _tabbedGroups = tabbedGroups;

			_tabbedGroups.PageCloseRequest += new TabbedGroups.PageCloseRequestHandler(OnTabbedGroupPageClosePressed);
			_tabbedGroups.PageChanged += new TabbedGroups.PageChangeHandler(OnTabbedGroupPageChanged);
		}

		public void AddWorkpace(IWorkspace workspace)
		{
			try
			{
				RebuildMenusAndToolbars();
				// Add the new tab
				WorkspaceTabPage workspaceTab = new WorkspaceTabPage(workspace);
				workspaceTab.Selected = true;
				_tabbedGroups.ActiveLeaf.TabPages.Add(workspaceTab);
			}
			catch (Exception ex)
			{
				Platform.Log(ex, LogLevel.Error);
				throw;
			}

		}

		public void RemoveWorkspace(IWorkspace workspace)
		{
			if (workspace == null || _tabbedGroups.RootSequence == null)
				return;

			try
			{
				// Remove the tab
				TabPageCollection tabPages;
				WorkspaceTabPage tabPage = FindTabPage(_tabbedGroups.RootSequence, workspace, out tabPages)
					as WorkspaceTabPage;

				if (tabPage == null)
					return;

				tabPages.Remove(tabPage);

				// Remove workspace from the model
				_desktopForm.DesktopWindow.WorkspaceManager.Workspaces.Remove(workspace);

				// TODO: We probably need to go into the TabControls and clear them
				// out to prevent references to workspace from being retained.  Will
				// do that when I deal with the memory allocation ticket #86.
				GC.Collect();
			}
			catch (Exception ex)
			{
				Platform.Log(ex, LogLevel.Error);
				throw;
			}

		}

		public void ActivateWorkspace(IWorkspace workspace)
		{
			if (workspace == null || _tabbedGroups.RootSequence == null)
				return;

			// When the active workspace changes we need to rebuild
			// the menu and toolbars to reflect the tools in use for the active workspace
			try
			{
				RebuildMenusAndToolbars();

				// Find the tab that owns the workspace and activate it
				TabPageCollection collection;
				WorkspaceTabPage tab = FindTabPage(_tabbedGroups.RootSequence, workspace, out collection) as WorkspaceTabPage;
				if (tab != null)
				{
					tab.Selected = true;
				}
			}
			catch (Exception ex)
			{
				Platform.Log(ex, LogLevel.Error);
				throw;
			}
		}

		private Crownwood.DotNetMagic.Controls.TabPage FindTabPage(
			TabGroupSequence nodeGroup, 
			IWorkspace workspace,
			out TabPageCollection containingCollection)
		{
			for (int i = 0; i < nodeGroup.Count; i++)
			{
				TabGroupBase node = nodeGroup[i];

				if (node.IsSequence)
				{
					Crownwood.DotNetMagic.Controls.TabPage page = FindTabPage(node as TabGroupSequence, workspace, out containingCollection);

					if (page != null)
						return page;
				}

				if (node.IsLeaf)
				{
					TabGroupLeaf leaf = node as TabGroupLeaf;

					foreach (WorkspaceTabPage page in leaf.TabPages)
					{
						if (page.Workspace == workspace)
						{
							containingCollection = leaf.TabPages;
							return page;
						}
					}
				}
			}

			containingCollection = null;
			return null;
		}

		private void OnTabbedGroupPageClosePressed(TabbedGroups groups, TGCloseRequestEventArgs e)
		{
			try
			{
				WorkspaceTabPage page = e.TabPage as WorkspaceTabPage;
				// We cancel so that DotNetMagic doesn't remove the tab; we want
				// to do that manually
				e.Cancel = true;

				RemoveWorkspace(page.Workspace);

				string str = String.Format("Workspaces: {0}", _desktopForm.DesktopWindow.WorkspaceManager.Workspaces.Count);
				System.Diagnostics.Trace.Write(str);
			}
			catch (Exception ex)
			{
				Platform.Log(ex, LogLevel.Error);
			}
		}

		private void OnTabbedGroupPageChanged(TabbedGroups tg, Crownwood.DotNetMagic.Controls.TabPage tp)
		{
			// Check for case when the last page in the group has been deleted
			if (tp == null)
				return;

			WorkspaceTabPage page = tp as WorkspaceTabPage;
			WorkspaceManager workspaceManager = page.Workspace.DesktopWindow.WorkspaceManager;
			workspaceManager.ActiveWorkspace = page.Workspace;
		}

		private void RebuildMenusAndToolbars()
		{
			_desktopForm.RebuildMenusAndToolbars();
		}

	}
}
