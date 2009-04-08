#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using Crownwood.DotNetMagic.Controls;
using Crownwood.DotNetMagic.Docking;
using System.IO;
using ClearCanvas.Common.Utilities;

using WinFormsScreen = System.Windows.Forms.Screen;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// WinForms implementation of <see cref="IDesktopWindowView"/>. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class may subclassed if customization is desired.  In this case, the <see cref="ApplicationView"/>
    /// class must also be subclassed in order to instantiate the subclass from 
    /// its <see cref="ApplicationView.CreateDesktopWindowView"/> method.
    /// </para>
    /// <para>
    /// Reasons for subclassing may include: overriding the <see cref="CreateDesktopForm"/> factory method to supply
    /// a custom subclass of the <see cref="DesktopForm"/> class, overriding the <see cref="CreateWorkspaceView"/>,
    /// <see cref="CreateShelfView"/>,
    /// or <see cref="CreateDialogBoxView"/> factory methods to supply custom subclasses of these view classes, overriding
    /// <see cref="SetMenuModel"/> or <see cref="SetToolbarModel"/> to customize the menu/toolbar display,
    /// and overriding <see cref="ShowMessageBox"/> to customize the display of message boxes.
    /// </para>
    /// </remarks>
    public class DesktopWindowView : DesktopObjectView, IDesktopWindowView
    {
        private static OrderedSet<DesktopWindowView> _desktopWindowActivationOrder = new OrderedSet<DesktopWindowView>();

    	private DesktopWindow _desktopWindow;
        private DesktopForm _form;
        private OrderedSet<WorkspaceView> _workspaceActivationOrder;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="window"></param>
        protected internal DesktopWindowView(DesktopWindow window)
        {
        	_desktopWindow = window;
            _form = CreateDesktopForm();
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

    	internal string DesktopWindowName
    	{
			get { return _desktopWindow.Name; }	
    	}

        #region Form Event Handlers

        /// <summary>
        /// Cancels the forms closing event, and raises our <see cref="CloseRequested"/> event instead.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormFormClosingEventHandler(object sender, FormClosingEventArgs e)
        {
            // cancel the request - don't let winforms close the form
            e.Cancel = true;

            switch (e.CloseReason)
            {
                case System.Windows.Forms.CloseReason.ApplicationExitCall:
                case System.Windows.Forms.CloseReason.TaskManagerClosing:
                case System.Windows.Forms.CloseReason.WindowsShutDown:
                    // windows is trying close the application, not just this window
                    // rather than propagate the request to close this window, we need
                    // to ask the entire desktop to quit
                    Application.Quit();
                    break;
                case System.Windows.Forms.CloseReason.UserClosing:
                case System.Windows.Forms.CloseReason.None:

                    // notify the model that a close request was made
                    RaiseCloseRequested();
                    break;
                default:
                    // other close reasons are not applicable
                    break;
            }
        }

        private void FormDeactivateEventHandler(object sender, EventArgs e)
        {
            // do nothing
            // note: if we are showing a modal dialog, the form gets de-activated, but we are still the active desktop window
            // therefore, this event is not really useful to us
        }

        /// <summary>
        /// Handles the forms Activated event in order to track the currently active window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormActivatedEventHandler(object sender, EventArgs e)
        {
            // de-activate the previous window before activating the new one
            DesktopWindowView lastActive = _desktopWindowActivationOrder.LastElement;
            if (lastActive != this)
            {
                if (lastActive != null)
                {
                    lastActive.SetActiveStatus(false);
                }

                this.SetActiveStatus(true);
                _desktopWindowActivationOrder.Add(this);
            }
        }

        /// <summary>
        /// Handles the forms visible event in order to track our visible status.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormVisibleChangedEventHandler(object sender, EventArgs e)
        {
            this.SetVisibleStatus(_form.Visible);
        }

        #endregion

        #region Workspace Management

        internal void AddWorkspaceView(WorkspaceView workspaceView)
        {
            // When we add a new workspace, we need to
            HideShelvesOnWorkspaceOpen();

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

        internal Content AddShelfView(ShelfView shelfView, Control control, string title, ShelfDisplayHint hint, MemoryStream shelfRestoreStream)
        {
        	// Forcing this makes the control resize *before* adding it to the DotNetMagic control, 
        	// so the shelf will be the correct size.  This would be done automatically when the
			// control gets added - we're just doing it a bit prematurely in order to get the correct size.
        	control.Font = _form.DockingManager.TabControlFont;
        	Size displaySize = control.Size;

			Content content = _form.DockingManager.Contents.Add(control, title);
			content.Tag = shelfView;

			if (shelfRestoreStream != null)
			{
				content.LoadContentFromStream(shelfRestoreStream);

				_form.DockingManager.ShowContent(content);
				if (content.IsAutoHidden && hint != ShelfDisplayHint.HideOnWorkspaceOpen)
					_form.DockingManager.BringAutoHideIntoView(content);

				return content;
			}

        	content.DisplaySize = displaySize;
        	content.AutoHideSize = displaySize;
        	content.FloatingSize = displaySize;

        	if ((hint & ShelfDisplayHint.DockAutoHide) != 0)
        		_form.DockingManager.Container.SuspendLayout();

        	// Dock the window on the correct edge
        	if ((hint & ShelfDisplayHint.DockTop) != 0)
        	{
        		_form.DockingManager.AddContentWithState(content, State.DockTop);
        	}
        	else if ((hint & ShelfDisplayHint.DockBottom) != 0)
        	{
        		_form.DockingManager.AddContentWithState(content, State.DockBottom);
        	}
        	else if ((hint & ShelfDisplayHint.DockLeft) != 0)
        	{
        		_form.DockingManager.AddContentWithState(content, State.DockLeft);
        	}
        	else if ((hint & ShelfDisplayHint.DockRight) != 0)
        	{
        		_form.DockingManager.AddContentWithState(content, State.DockRight);
        	}
        	else
        	{
				if ((hint & ShelfDisplayHint.ShowNearMouse) == ShelfDisplayHint.ShowNearMouse)
				{
					content.DisplayLocation = Control.MousePosition;
				}

        		_form.DockingManager.AddContentWithState(content, State.Floating);
        	}

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
			if (!shelfView.Content.Visible)
				shelfView.Content.BringToFront();

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

        internal void HideShelvesOnWorkspaceOpen()
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
                // auto-hidden - bring into view
                _form.DockingManager.BringAutoHideIntoView(shelfView.Content);
            }
            else
            {
                // docked or floating - ensure we are in front
                shelfView.Content.BringToFront();
            }

            // set focus to the control - this is what actually activates the window
            shelfView.Content.Control.Focus();
        }

        internal void RemoveShelfView(ShelfView shelfView)
        {
			shelfView.SaveState();

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

        /// <summary>
        /// Creates a new view for the specified <see cref="Workspace"/>.
        /// </summary>
        /// <remarks>
        /// Override this method if you want to return a custom implementation of <see cref="IWorkspaceView"/>.
        /// In practice, it is preferable to subclass <see cref="WorkspaceView"/> rather than implement <see cref="IWorkspaceView"/>
        /// directly.
        /// </remarks>
        /// <param name="workspace"></param>
        /// <returns></returns>
        public virtual IWorkspaceView CreateWorkspaceView(Workspace workspace)
        {
            return new WorkspaceView(workspace, this);
        }

        /// <summary>
        /// Creates a new view for the specified <see cref="Shelf"/>.
        /// </summary>
        /// <remarks>
        /// Override this method if you want to return a custom implementation of <see cref="IShelfView"/>.
        /// In practice, it is preferable to subclass <see cref="ShelfView"/> rather than implement <see cref="IShelfView"/>
        /// directly.
        /// </remarks>
        /// <param name="shelf"></param>
        /// <returns></returns>
        public virtual IShelfView CreateShelfView(Shelf shelf)
        {
            return new ShelfView(shelf, this);
        }

        /// <summary>
        /// Creates a new view for the specified <see cref="DialogBox"/>.
        /// </summary>
        /// <remarks>
        /// Override this method if you want to return a custom implementation of <see cref="IDialogBoxView"/>.
        /// In practice, it is preferable to subclass <see cref="DialogBoxView"/> rather than implement <see cref="IDialogBoxView"/>
        /// directly.
        /// </remarks>
        /// <param name="dialogBox"></param>
        /// <returns></returns>
        public virtual IDialogBoxView CreateDialogBoxView(DialogBox dialogBox)
        {
            return new DialogBoxView(dialogBox, this);
        }

        /// <summary>
        /// Sets the menu model, causing the menu displayed on the screen to be updated.
        /// </summary>
        /// <remarks>
        /// The default implementation just sets the <see cref="DesktopForm.MenuModel"/> property.
        /// Override this method if you need to perform custom processing.
        /// </remarks>
        /// <param name="model"></param>
        public virtual void SetMenuModel(ActionModelNode model)
        {
            _form.MenuModel = model;
        }

        /// <summary>
        /// Sets the toolbar model, causing the toolbar displayed on the screen to be updated.
        /// </summary>
        /// <remarks>
        /// The default implementation just sets the <see cref="DesktopForm.ToolbarModel"/> property.
        /// Override this method if you need to perform custom processing.
        /// </remarks>
        /// <param name="model"></param>
        public virtual void SetToolbarModel(ActionModelNode model)
        {
            _form.ToolbarModel = model;
        }

        /// <summary>
        /// Displays a message box.
        /// </summary>
        /// <remarks>
        /// Override this method if you need to customize the display of message boxes.
        /// </remarks>
        /// <param name="message"></param>
        /// <param name="buttons"></param>
        /// <returns></returns>
        public virtual DialogBoxAction ShowMessageBox(string message, string title, MessageBoxActions buttons)
        {
            MessageBox mb = new MessageBox();
            return mb.Show(message, title, buttons, _form);
        }

    	/// <summary>
    	/// Shows a 'Save file' dialog in front of this window.
    	/// </summary>
    	/// <param name="args"></param>
    	/// <returns></returns>
    	public virtual FileDialogResult ShowSaveFileDialogBox(FileDialogCreationArgs args)
    	{
			SaveFileDialog dialog = new SaveFileDialog();
			PrepareFileDialog(dialog, args);
			dialog.OverwritePrompt = true;

			DialogResult dr = dialog.ShowDialog(_form);
			if(dr == DialogResult.OK)
			{
				return new FileDialogResult(DialogBoxAction.Ok, dialog.FileName);
			}
			else 
				return new FileDialogResult(DialogBoxAction.Cancel, null);
    	}

    	/// <summary>
    	/// Shows a 'Open file' dialog in front of this window.
    	/// </summary>
    	/// <param name="args"></param>
    	/// <returns></returns>
    	public virtual FileDialogResult ShowOpenFileDialogBox(FileDialogCreationArgs args)
    	{
			OpenFileDialog dialog = new OpenFileDialog();
			PrepareFileDialog(dialog, args);
    		dialog.CheckFileExists = true;
    		dialog.ShowReadOnly = false;
    		dialog.Multiselect = false; //could add support in future if necessary

			DialogResult dr = dialog.ShowDialog(_form);
			if (dr == DialogResult.OK)
			{
				return new FileDialogResult(DialogBoxAction.Ok, dialog.FileName);
			}
			else
				return new FileDialogResult(DialogBoxAction.Cancel, null);
		}

    	/// <summary>
    	/// Shows a 'Select folder' dialog in front of this window.
    	/// </summary>
    	/// <param name="args"></param>
    	/// <returns></returns>
    	public FileDialogResult ShowSelectFolderDialogBox(SelectFolderDialogCreationArgs args)
    	{
    		FolderBrowserDialog dialog = new FolderBrowserDialog();
    		dialog.SelectedPath = args.Path ?? "";
    		dialog.Description = args.Prompt ?? "";
    		dialog.ShowNewFolderButton = args.AllowCreateNewFolder;

    		DialogResult dr = dialog.ShowDialog(_form);
    		if (dr == DialogResult.OK)
    		{
    			return new FileDialogResult(DialogBoxAction.Ok, dialog.SelectedPath);
    		}
    		else
    			return new FileDialogResult(DialogBoxAction.Cancel, null);
    	}

    	#endregion

        #region DesktopObjectView overrides

        /// <summary>
        /// Opens this view, showing the form on the screen.
        /// </summary>
        public override void Open()
        {
			try
			{
				LoadWindowSettings();
			}
			catch (Exception e)
			{
				// if the window settings can't be loaded for any reason,
				// just log it and move on
				Platform.Log(LogLevel.Error, e);
			}

            _form.Show();
        }

        /// <summary>
        /// Activates the view, activating the form on the screen.
        /// </summary>
        public override void Activate()
        {
            _form.Activate();
        }

        /// <summary>
        /// Shows the view, making the form visible on the screen.
        /// </summary>
        public override void Show()
        {
            _form.Show();
        }

        /// <summary>
        /// Hides the view, hiding the form on the screen.
        /// </summary>
        public override void Hide()
        {
            _form.Hide();
        }

        /// <summary>
        /// Sets the title that is displayed in the form's title bar.
        /// </summary>
        /// <remarks>
        /// Override this method if you need to customize the title that is displayed on the form.
        /// </remarks>
        /// <param name="title"></param>
        public override void SetTitle(string title)
        {
            _form.Text = title;
        }

        /// <summary>
        /// Disposes of this object, closing the form.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && _form != null)
            {
				_form.VisibleChanged -= new EventHandler(FormVisibleChangedEventHandler);
				_form.Activated -= new EventHandler(FormActivatedEventHandler);
				_form.Deactivate -= new EventHandler(FormDeactivateEventHandler);
				_form.FormClosing -= new FormClosingEventHandler(FormFormClosingEventHandler);
				_form.TabbedGroups.PageCloseRequest -= new TabbedGroups.PageCloseRequestHandler(TabbedGroupPageClosePressedEventHandler);
				_form.TabbedGroups.PageChanged -= new TabbedGroups.PageChangeHandler(TabbedGroupPageChangedEventHandler);
				_form.DockingManager.ContentHiding -= new DockingManager.ContentHidingHandler(DockingManagerContentHidingEventHandler);
				_form.DockingManager.ContentShown -= new DockingManager.ContentHandler(DockingManagerContentShownEventHandler);
				_form.DockingManager.ContentAutoHideOpening -= new DockingManager.ContentHandler(DockingManagerContentAutoHideOpeningEventHandler);
				_form.DockingManager.ContentAutoHideClosed -= new DockingManager.ContentHandler(DockingManagerContentAutoHideClosedEventHandler);
				_form.DockingManager.WindowActivated -= new DockingManager.WindowHandler(DockingManagerWindowActivatedEventHandler);
				_form.DockingManager.WindowDeactivated -= new DockingManager.WindowHandler(FormDockingManagerWindowDeactivatedEventHandler);

                try
                {
                    SaveWindowSettings();
                }
                catch (Exception e)
                {
                    // if the window settings can't be saved for any reason,
                    // just log it and move on
                    Platform.Log(LogLevel.Error, e);
                }

                // bug #1171: if this window is the active window and there are other windows,
                // select the previously active one before destroying this one
                if (_desktopWindowActivationOrder.LastElement == this && _desktopWindowActivationOrder.Count > 1)
                {
                    _desktopWindowActivationOrder.SecondLastElement.Activate();
                }

                // remove this window from the activation order
                _desktopWindowActivationOrder.Remove(this);


                // now that we've cleaned up the activation,
                // we can destroy the form safely without worrying 
                // about the OS triggering activation events

                // this will close the form without firing any events
                _form.Dispose();
                _form = null;

                // notify that we are no longer visible
                SetVisibleStatus(false);
            }

            base.Dispose(disposing);
        }

        #endregion

        /// <summary>
        /// Called to create an instance of a <see cref="DesktopForm"/> for use by this view.
        /// </summary>
        /// <returns></returns>
        protected virtual DesktopForm CreateDesktopForm()
        {
            return new DesktopForm();
        }

        /// <summary>
        /// Gets the <see cref="DesktopForm"/> that is displayed on the screen.
        /// </summary>
        protected internal DesktopForm DesktopForm
        {
            get { return _form; }
        }

		private void LoadWindowSettings()
		{
			Rectangle screenRectangle;
			FormWindowState windowState;
			if (!DesktopViewSettings.Default.GetDesktopWindowState(_desktopWindow.Name, out screenRectangle, out windowState))
			{
				screenRectangle = WinFormsScreen.PrimaryScreen.Bounds;

				// Make the window size 75% of the primary screen
				float scale = 0.75f;
				_form.Width = (int)(screenRectangle.Width * scale);
				_form.Height = (int)(screenRectangle.Height * scale);

				_form.StartPosition = FormStartPosition.CenterScreen;
			}
			else
			{
				_form.Location = screenRectangle.Location;
				_form.Size = screenRectangle.Size;
				_form.StartPosition = FormStartPosition.Manual;

				// If window was last closed when minimized, don't open it up minimized,
				// but rather just open it normally
				if (windowState == FormWindowState.Minimized)
					_form.WindowState = FormWindowState.Normal;
				else
					_form.WindowState = windowState;
			}
		}

		private void SaveWindowSettings()
		{
			Rectangle windowRectangle;
			// If the window state is normal, just save its location and size
			if (_form.WindowState == FormWindowState.Normal)
				windowRectangle = new Rectangle(_form.Location, _form.Size);
			// But, if it's minimized or maximized, save the restore bounds instead
			else
				windowRectangle = _form.RestoreBounds;

			FormWindowState windowState = _form.WindowState;

			DesktopViewSettings.Default.SaveDesktopWindowState(_desktopWindow.Name, windowRectangle, windowState);
		}

		private void PrepareFileDialog(FileDialog dialog, FileDialogCreationArgs args)
		{
			dialog.AddExtension = !string.IsNullOrEmpty(args.FileExtension);
			dialog.DefaultExt = args.FileExtension;
			dialog.FileName = args.FileName;
			dialog.InitialDirectory = args.Directory;
			dialog.RestoreDirectory = true;
			dialog.Title = args.Title;

			dialog.Filter = StringUtilities.Combine(args.Filters, "|",
				delegate(FileExtensionFilter f) { return f.Description + "|" + f.Filter; });
		}
	}
}
