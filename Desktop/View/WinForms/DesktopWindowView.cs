using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using Crownwood.DotNetMagic.Controls;
using Crownwood.DotNetMagic.Docking;
using System.ComponentModel;

namespace ClearCanvas.Desktop.View.WinForms
{
    public class DesktopWindowView : DesktopObjectView, IDesktopWindowView
    {
        private static DesktopWindowView _lastActiveWindow;

        private DesktopForm _form;
        private OrderedSet<WorkspaceView> _workspaceActivationOrder;

        protected internal DesktopWindowView(DesktopWindow window)
        {
            _form = new DesktopForm();
            _workspaceActivationOrder = new OrderedSet<WorkspaceView>();

            // listen to some events on the form
            _form.VisibleChanged += new EventHandler(FormVisibleChangedEventHandler);
            _form.Activated += new EventHandler(FormActivatedEventHandler);
            _form.Deactivate += new EventHandler(FormDeactivateEventHandler);
            _form.FormClosing += new FormClosingEventHandler(FormFormClosingEventHandler);
            _form.TabbedGroups.PageCloseRequest += new TabbedGroups.PageCloseRequestHandler(TabbedGroupPageClosePressedEventHandler);
            _form.TabbedGroups.PageChanged += new TabbedGroups.PageChangeHandler(TabbedGroupPageChangedEventHandler);
            // NY: We subscribe to ContentHiding instead of ContentHidden because ContentHidden
            // is fired when the user double clicks the caption bar of a docking window, which
            // results in a crash. (Ticket #144)
            _form.DockingManager.ContentHiding += new DockingManager.ContentHidingHandler(DockingManagerContentHidingEventHandler);
            _form.DockingManager.ContentShown += new DockingManager.ContentHandler(DockingManagerContentShownEventHandler);
            _form.DockingManager.ContentAutoHideOpening += new DockingManager.ContentHandler(DockingManagerContentAutoHideOpeningEventHandler);
            _form.DockingManager.ContentAutoHideClosed += new DockingManager.ContentHandler(DockingManagerContentAutoHideClosedEventHandler);
            _form.DockingManager.WindowActivated += new DockingManager.WindowHandler(DockingManagerWindowActivatedEventHandler);
            _form.DockingManager.WindowDeactivated += new DockingManager.WindowHandler(FormDockingManagerWindowDeactivatedEventHandler);
        }

        #region Form Event Handlers

        private void FormFormClosingEventHandler(object sender, FormClosingEventArgs e)
        {
            // cancel the request - don't let winforms close the form
            e.Cancel = true;

            // notify the model that a close request was made
            RaiseCloseRequested();
        }

        private void FormDeactivateEventHandler(object sender, EventArgs e)
        {
            // do nothing
        }

        private void FormActivatedEventHandler(object sender, EventArgs e)
        {
            if (_lastActiveWindow != this)
            {
                if (_lastActiveWindow != null)
                {
                    _lastActiveWindow.SetActiveStatus(false);
                }

                this.SetActiveStatus(true);

                _lastActiveWindow = this;
            }
        }

        private void FormVisibleChangedEventHandler(object sender, EventArgs e)
        {
            this.SetVisibleStatus(_form.Visible);
        }

        #endregion

        #region Workspace Management

        internal void AddWorkspaceView(WorkspaceView workspaceView)
        {
            // When we add a new workspace, we need to
            HideShelves();

            _form.TabbedGroups.ActiveLeaf.TabPages.Add(workspaceView.TabPage);
            workspaceView.TabPage.Selected = true;

            _form.TabbedGroups.DisplayTabMode = DisplayTabModes.ShowAll;
        }

        internal void RemoveWorkspaceView(WorkspaceView workspaceView)
        {
            // remove this page from the activation order
            _workspaceActivationOrder.Remove(workspaceView);

            // if the page being removed is the selected page and there are other workspaces,
            // select the last active one before removing this one
            if (workspaceView.TabPage.Selected && _workspaceActivationOrder.Count > 0)
            {
                _workspaceActivationOrder.LastElement.TabPage.Selected = true;
            }

            // Remove the tab
            TabPageCollection tabPages;
            bool found = FindTabPageCollection(_form.TabbedGroups.RootSequence, workspaceView.TabPage, out tabPages);
            if (found)
                tabPages.Remove(workspaceView.TabPage);

            // notify that we are no longer visible
            workspaceView.SetVisibleStatus(false);

            // When there are no tabs left, turn off the tab control strip.
            // Done purely for aesthetic reasons.
            if (_form.TabbedGroups.ActiveLeaf.TabPages.Count == 0)
                _form.TabbedGroups.DisplayTabMode = DisplayTabModes.HideAll;

            _form.DockingManager.Container.Focus();
        }

        private bool FindTabPageCollection(
                TabGroupSequence nodeGroup,
                Crownwood.DotNetMagic.Controls.TabPage tabPage,
                out TabPageCollection containingCollection)
        {
            for (int i = 0; i < nodeGroup.Count; i++)
            {
                TabGroupBase node = nodeGroup[i];

                if (node.IsSequence)
                {
                    bool found = FindTabPageCollection(node as TabGroupSequence, tabPage, out containingCollection);

                    if (found)
                        return true;
                }

                if (node.IsLeaf)
                {
                    TabGroupLeaf leaf = node as TabGroupLeaf;
                    if (leaf.TabPages.Contains(tabPage))
                    {
                        containingCollection = leaf.TabPages;
                        return true;
                    }
                }
            }

            containingCollection = null;
            return false;
        }

        private void TabbedGroupPageClosePressedEventHandler(TabbedGroups groups, TGCloseRequestEventArgs e)
        {
            WorkspaceView wv = (WorkspaceView)e.TabPage.Tag;

            // We cancel so that DotNetMagic doesn't remove the tab; we want
            // to do that programatically
            e.Cancel = true;

            // raise close requested event
            wv.RaiseCloseRequested();
        }

        private void TabbedGroupPageChangedEventHandler(TabbedGroups tg, Crownwood.DotNetMagic.Controls.TabPage tp)
        {
            // de-activate the previous workspace before activating the new one
            WorkspaceView lastActive = _workspaceActivationOrder.LastElement;
            if (lastActive != null)
            {
                lastActive.SetActiveStatus(false);
            }

            // important to check tp != null to account for the case where the last workspace closes
            WorkspaceView nowActive = (tp != null) ? (WorkspaceView)tp.Tag : null;
            if (nowActive != null)
            {
                nowActive.SetVisibleStatus(true);   // the very first time the page is selected, need to change its visible status
                nowActive.SetActiveStatus(true);
                _workspaceActivationOrder.Add(nowActive);
            }
        }

        #endregion

        #region Shelf Management

        internal Content AddShelfView(ShelfView shelfView, Control control, string title, ShelfDisplayHint hint)
        {
            Content content = _form.DockingManager.Contents.Add(control, title);
            content.Tag = shelfView;

            // Make sure the window is the size as it's been defined by the tool
            //if ((hint & ShelfDisplayHint.MaximizeOnDock) != 0)
            //    content.DisplaySize = _form.DockingManager.Container.Size;
            //else
                content.DisplaySize = content.Control.Size;

            content.AutoHideSize = content.Control.Size;
            content.FloatingSize = content.Control.Size;

            if ((hint & ShelfDisplayHint.DockAutoHide) != 0)
                _form.DockingManager.Container.SuspendLayout();

            // Dock the window on the correct edge
            if ((hint & ShelfDisplayHint.DockTop) != 0)
                _form.DockingManager.AddContentWithState(content, State.DockTop);
            else if ((hint & ShelfDisplayHint.DockBottom) != 0)
                _form.DockingManager.AddContentWithState(content, State.DockBottom);
            else if ((hint & ShelfDisplayHint.DockLeft) != 0)
                _form.DockingManager.AddContentWithState(content, State.DockLeft);
            else if ((hint & ShelfDisplayHint.DockRight) != 0)
                _form.DockingManager.AddContentWithState(content, State.DockRight);
            else
                _form.DockingManager.AddContentWithState(content, State.Floating);

            if ((hint & ShelfDisplayHint.DockAutoHide) != 0)
            {
                _form.DockingManager.ToggleContentAutoHide(content);
                _form.DockingManager.Container.ResumeLayout();
                _form.DockingManager.BringAutoHideIntoView(content);
            }

            return content;
        }

        internal void ShowShelfView(ShelfView shelfView)
        {
            if (shelfView.Content.IsDocked)
            {
                if (shelfView.Content.IsAutoHidden)   // auto-hide mode
                {
                    // show without activating
                    _form.DockingManager.BringAutoHideIntoView(shelfView.Content); // show it
                }
                else
                {
                    // content is pinned - therefore it should be already visible
                }
            }
            else
            {
                // floating
                _form.DockingManager.ShowContent(shelfView.Content);
            }
        }

        internal void HideShelfView(ShelfView shelfView)
        {
            if (shelfView.Content.IsDocked)
            {
                if (shelfView.Content.IsAutoHidden)   // auto-hide mode
                {
                    // only one auto-hide window can be showing at a given time, so calling this method should hide it
                    _form.DockingManager.RemoveShowingAutoHideWindows();
                }
                else
                {
                    // content is pinned - putting it in auto-hide mode should hide it
                    _form.DockingManager.ToggleContentAutoHide(shelfView.Content);

                    // the window seems to remain active even though it is not visible, which doesn't make much sense
                    // therefore, let's report it as inactive
                    shelfView.SetActiveStatus(false);
                    // since we don't seem to get a content-hiding message in this case, need to explicitly set this
                    shelfView.SetVisibleStatus(false);
                }
            }
            else
            {
                // floating
                _form.DockingManager.HideContent(shelfView.Content);

                // since we don't seem to get a content-hiding message in this case, need to explicitly set this
                shelfView.SetVisibleStatus(false);  
            }
        }

        internal void HideShelves()
        {
            // 1) Retract all visible autohide windows
            // 2) Put docked windows in autohide mode if the tool has specified so
            _form.DockingManager.RemoveShowingAutoHideWindows();

            for (int i = 0; i < _form.DockingManager.Contents.Count; i++)
            {
                Content content = _form.DockingManager.Contents[i];

                ShelfView shelfView = (ShelfView)content.Tag;
                if ((shelfView.DisplayHint & ShelfDisplayHint.HideOnWorkspaceOpen) != 0)
                {
                    shelfView.Hide();
                }
            }
        }
        
        internal void ActivateShelfView(ShelfView shelfView)
        {
            if (shelfView.Content.IsAutoHidden)
            {
                _form.DockingManager.BringAutoHideIntoView(shelfView.Content);
            }
            else
            {
                shelfView.Content.BringToFront();
            }
            shelfView.Content.Control.Focus();
        }

        internal void RemoveShelfView(ShelfView shelfView)
        {
            _form.DockingManager.Contents.Remove(shelfView.Content);
            shelfView.SetVisibleStatus(false);
        }

        private void DockingManagerContentHidingEventHandler(Content c, CancelEventArgs cea)
        {
            // this event is fired when the X on the shelf is clicked
            ShelfView shelfView = (ShelfView)c.Tag;

            // don't let dotnetmagic remove the shelf
            cea.Cancel = true;

            shelfView.RaiseCloseRequested();
        }

        private void DockingManagerContentShownEventHandler(Content c, EventArgs cea)
        {
        }

        private void DockingManagerContentAutoHideClosedEventHandler(Content c, EventArgs cea)
        {
            ShelfView shelfView = (ShelfView)c.Tag;

            shelfView.SetActiveStatus(false);   // force active to false, since the dotnetmagic events are not reliable
            shelfView.SetVisibleStatus(false);
        }

        private void DockingManagerContentAutoHideOpeningEventHandler(Content c, EventArgs cea)
        {
            ShelfView shelfView = (ShelfView)c.Tag;
            shelfView.SetVisibleStatus(true);
        }

        private void FormDockingManagerWindowDeactivatedEventHandler(DockingManager dm, Window wd)
        {
            Content content = (wd as WindowContent).CurrentContent;

            // seems that content may sometimes be null - not sure why
            // in this case, just ignore the event
            if (content != null)
            {
                ShelfView shelfView = (ShelfView)content.Tag;
                shelfView.SetActiveStatus(false);
            }
        }

        private void DockingManagerWindowActivatedEventHandler(DockingManager dm, Window wd)
        {
            Content content = (wd as WindowContent).CurrentContent;

            // seems that content may sometimes be null - not sure why
            // in this case, just ignore the event
            if (content != null)
            {
                ShelfView shelfView = (ShelfView)content.Tag;
                // when activated, report both visible and active status
                shelfView.SetVisibleStatus(true);
                shelfView.SetActiveStatus(true);
            }
        }

        #endregion

        #region IDesktopWindowView Members

        public IWorkspaceView CreateWorkspaceView(Workspace workspace)
        {
            return new WorkspaceView(workspace, this);
        }

        public IShelfView CreateShelfView(Shelf shelf)
        {
            return new ShelfView(shelf, this);
        }

        public void SetMenuModel(ActionModelNode model)
        {
            _form.MenuModel = model;
        }

        public void SetToolbarModel(ActionModelNode model)
        {
            _form.ToolbarModel = model;
        }

        public DialogBoxAction ShowMessageBox(string message, MessageBoxActions buttons)
        {
            MessageBox mb = new MessageBox();
            return mb.Show(message, buttons, _form);
        }

        public IDialogBoxView CreateDialogBoxView(DialogBox dialogBox)
        {
            return new DialogBoxView(dialogBox, _form);
        }

        #endregion

        #region DesktopObjectView overrides

        public override void Open()
        {
            _form.LoadWindowSettings();
            _form.Show();
        }

        public override void Activate()
        {
            _form.Activate();
        }

        public override void Show()
        {
            _form.Show();
        }

        public override void Hide()
        {
            _form.Hide();
        }

        public override void SetTitle(string title)
        {
            _form.Text = title;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _form != null)
            {
                _form.SaveWindowSettings();

                // this will close the form without firing any events
                _form.Dispose();
                _form = null;

                // notify that we are no longer visible
                SetVisibleStatus(false);
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
