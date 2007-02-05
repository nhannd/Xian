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

		public void AddWorkpaceTab(IWorkspace workspace)
		{
			try
			{
				_tabbedGroups.DisplayTabMode = DisplayTabModes.ShowAll;
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

        public void RemoveWorkspaceTab(IWorkspace workspace)
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
				
				// tabPages.Remove does not seem to call Dispose(), so let's do it explicitly
				if(!tabPage.IsDisposed)
				    tabPage.Dispose();

                GC.Collect();
				GC.WaitForPendingFinalizers();
				//string str = String.Format("Memory: {0}", GC.GetTotalMemory(false));
				//Platform.Log(str);

				// When there are no tabs left, turn off the tab control strip.
				// Done purely for aesthetic reasons.
				if (_tabbedGroups.ActiveLeaf.TabPages.Count == 0)
					_tabbedGroups.DisplayTabMode = DisplayTabModes.HideAll;
            }
            catch (Exception ex)
            {
                Platform.Log(ex, LogLevel.Error);
                throw;
            }
        }

		public void ActivateWorkspace(IWorkspace workspace)
		{
			// When the active workspace changes we need to rebuild
			// the menu and toolbars to reflect the tools in use for the active workspace
			try
			{
				RebuildMenusAndToolbars();

				if (workspace == null || _tabbedGroups.RootSequence == null)
					return;

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
				// to do that manually, and only if the workspace is successfully removed
				e.Cancel = true;

                // Try to remove workspace from the model (this may not succeed)
				_desktopForm.DesktopWindow.WorkspaceManager.Workspaces.Remove(page.Workspace);
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
