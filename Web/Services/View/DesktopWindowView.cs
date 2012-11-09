#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Web.Common;
using ClearCanvas.Web.Common.Entities;
using ClearCanvas.Web.Common.Events;

namespace ClearCanvas.Web.Services.View
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
    /// </remarks>
    public class DesktopWindowView : DesktopObjectView, IDesktopWindowView
    {
        private DialogBoxAction _result;
        private IDesktopAlertContext _alertContext;
        private DesktopWindow _window;
        private List<IWorkspaceView> _workspaceViews;
 
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="window"></param>
        protected internal DesktopWindowView(DesktopWindow window)
        {
            _window = window;
            _workspaceViews = new List<IWorkspaceView>();
        }

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
        /// </remarks>
        /// <param name="shelf"></param>
        /// <returns></returns>
        public virtual IShelfView CreateShelfView(Shelf shelf)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Creates a new view for the specified <see cref="DialogBox"/>.
        /// </summary>
        /// <remarks>
        /// Override this method if you want to return a custom implementation of <see cref="IDialogBoxView"/>.
        /// </remarks>
        /// <param name="dialogBox"></param>
        /// <returns></returns>
        public virtual IDialogBoxView CreateDialogBoxView(DialogBox dialogBox)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Sets the menu model, causing the menu displayed on the screen to be updated.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="model"></param>
        public virtual void SetMenuModel(ActionModelNode model)
        {
        }

        /// <summary>
        /// Sets the toolbar model, causing the toolbar displayed on the screen to be updated.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="model"></param>
        public virtual void SetToolbarModel(ActionModelNode model)
        {
        }

        /// <summary>
        /// Displays a message box.
        /// </summary>
        /// <remarks>
        /// Override this method if you need to customize the display of message boxes.
        /// </remarks>
        /// <param name="message"></param>
        /// <param name="buttons"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public virtual DialogBoxAction ShowMessageBox(string message, string title, MessageBoxActions buttons)
        {
            if (Services.ApplicationContext.Current != null)
            {

                var view = new MessageBoxView();
                view.SetModelObject(this);

                MessageBox box = view.GetEntity();
                box.YesLabel = SR.YesLabel;
                box.NoLabel = SR.NoLabel;
                box.CancelLabel = SR.CancelLabel;
                box.OkLabel = SR.OkLabel;
                box.Message = message;
                box.Title = title;
                switch (buttons)
                {
                    case MessageBoxActions.Ok:
                        box.Actions = WebMessageBoxActions.Ok;
                        _result = DialogBoxAction.Ok;
                        break;
                    case MessageBoxActions.OkCancel:
                        box.Actions = WebMessageBoxActions.OkCancel;
                        _result = DialogBoxAction.Ok;
                        break;
                    case MessageBoxActions.YesNo:
                        box.Actions = WebMessageBoxActions.YesNo;
                        _result = DialogBoxAction.Yes;
                        break;
                    case MessageBoxActions.YesNoCancel:
                        box.Actions = WebMessageBoxActions.YesNoCancel;
                        _result = DialogBoxAction.Yes;
                        break;
                }

                var @event = new MessageBoxShownEvent
                    {
                        Identifier = box.Identifier,
                        MessageBox = box,
                        SenderId = Services.ApplicationContext.Current.ApplicationId,
                    };

                FireEvent(@event);

                var context = (SynchronizationContext.Current as IWebSynchronizationContext);
                if (context != null) context.RunModal();
                return _result;
            }

            return DialogBoxAction.Ok;
        }

        public void SetAlertContext(IDesktopAlertContext alertContext)
        {
            _alertContext = alertContext;
        }

        public void ShowAlert(AlertNotificationArgs args)
        {
            var view = new AlertView();
            view.SetModelObject(this._window);
            view.SetLinkAction(args.LinkAction);

            Alert alertEvent = view.GetEntity();
            alertEvent.Message = args.Message;

            var image = _alertContext.GetIcon(args.Level).CreateIcon(IconSize.Large, new ResourceResolver(typeof(DesktopWindow).Assembly));

            using (var theStream = new MemoryStream())
            {
                image.Save(theStream, ImageFormat.Png);
                theStream.Position = 0;
                alertEvent.Icon = Convert.ToBase64String(theStream.GetBuffer());
            }

            alertEvent.DismissOnLinkClicked = args.DismissOnLinkClicked;
          
            switch (args.Level)
            {
                case AlertLevel.Error:
                    alertEvent.Level = WebAlertLevel.Error;
                    break;
                case AlertLevel.Warning:
                    alertEvent.Level = WebAlertLevel.Warning;
                    break;
                case AlertLevel.Info:
                    alertEvent.Level = WebAlertLevel.Info;
                    break;
            }

            var @event = new AlertShownEvent
                {
                    Identifier = alertEvent.Identifier,
                    AlertEvent = alertEvent,
                    SenderId = Services.ApplicationContext.Current.ApplicationId,
                };

            FireEvent(@event);            
        }

        /// <summary>
        /// Called to dismiss the dialog
        /// </summary>
        /// <param name="action"></param>
        public void Dismiss(DialogBoxAction action)
        {
            _result = action;

            IWebSynchronizationContext context = (SynchronizationContext.Current as IWebSynchronizationContext);
            if (context != null) context.BreakModal();
        }

        /// <summary>
    	/// Shows a 'Save file' dialog in front of this window.
    	/// </summary>
    	/// <param name="args"></param>
    	/// <returns></returns>
    	public virtual FileDialogResult ShowSaveFileDialogBox(FileDialogCreationArgs args)
    	{
            throw new NotSupportedException();
    	}

    	/// <summary>
    	/// Shows a 'Open file' dialog in front of this window.
    	/// </summary>
    	/// <param name="args"></param>
    	/// <returns></returns>
    	public virtual FileDialogResult ShowOpenFileDialogBox(FileDialogCreationArgs args)
    	{
            throw new NotSupportedException();
		}

    	/// <summary>
    	/// Shows a 'Select folder' dialog in front of this window.
    	/// </summary>
    	/// <param name="args"></param>
    	/// <returns></returns>
    	public FileDialogResult ShowSelectFolderDialogBox(SelectFolderDialogCreationArgs args)
    	{
            throw new NotSupportedException();
    	}

    	#endregion

        #region DesktopObjectView overrides

        public override void SetTitle(string title)
        {
        }

        /// <summary>
        /// Opens this view, showing the form on the screen.
        /// </summary>
        public override void Open()
        {
        }

        public override void Hide()
        {
        }

        /// <summary>
        /// Activates the view, activating the form on the screen.
        /// </summary>
        public override void Activate()
        {
        }

        /// <summary>
        /// Shows the view, making the form visible on the screen.
        /// </summary>
        public override void Show()
        {
        }

        public override void SetModelObject(object modelObject)
        {
            throw new NotImplementedException();
        }

        public override void ProcessMessage(Message message)
        {
            throw new NotImplementedException();
        }

        protected override void Initialize()
        {
        }

        protected override Entity CreateEntity()
        {
            throw new NotImplementedException();
        }

        protected override void UpdateEntity(Entity entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Disposes of this object, closing the
        ///  form.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing)
                return;
        
            if (_workspaceViews.Count == 0)
                return;

            foreach (var workspaceView in _workspaceViews)
                workspaceView.Dispose();

            _workspaceViews.Clear();
        }

        #endregion      
  	
        #region Workspace Management


        internal void AddWorkspaceView(WorkspaceView workspaceView)
        {
            workspaceView.SetVisibleStatus(true);
            workspaceView.SetActiveStatus(true);
            _workspaceViews.Add(workspaceView);
        }

        internal void RemoveWorkspaceView(WorkspaceView workspaceView)
        {
            // notify that we are no longer visible
            workspaceView.SetActiveStatus(false);
            workspaceView.SetVisibleStatus(false);
            _workspaceViews.Remove(workspaceView);
        }

        #endregion
    }
}
