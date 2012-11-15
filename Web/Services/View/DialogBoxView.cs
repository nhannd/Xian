#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Web.Common.Events;
using ClearCanvas.Web.Common.Messages;
using DialogBox = ClearCanvas.Desktop.DialogBox;

namespace ClearCanvas.Web.Services.View
{
    internal class DialogBoxView : DesktopObjectView, IDialogBoxView
    {
        private DesktopWindowView _desktopWindowView;
        private IApplicationComponent _component;
        private IWebApplicationComponentView _componentView;

        private DialogBox _dialogBox;
        private WebDialogBoxAction _result;

        private string _title;
        private bool _isRunningModal;

        internal DialogBoxView(DesktopWindowView desktopWindowView)
        {
            _desktopWindowView = desktopWindowView;
        }
        
        public override void SetTitle(string title)
        {
            if (Equals(_title, title))
                return;

            _title = title;
            if (_isRunningModal)
                NotifyEntityPropertyChanged("Title", _title);
        }

        public override void Open()
        {
        }

        public override void Show()
        {
        }

        public override void Hide()
        {
        }

        public override void Activate()
        {
        }

        public DialogBoxAction RunModal()
        {
            FireEvent(new DialogBoxShownEvent
                          {
                              //NOTE (Phoenix5): It's a bit weird that we're sending this on the application's behalf;
                              //If there were actually a DesktopWindow data contract, we could tell
                              //_desktopWindowView to fire it, which would be less weird.
                              SenderId = ApplicationContext.ApplicationId,
                              DialogBox = (Common.Entities.DialogBox)GetEntity()
                          });

            _isRunningModal = true;
            ((IWebSynchronizationContext)SynchronizationContext.Current).RunModal();

            FireEvent(new DialogBoxDismissedEvent());

            switch (_result)
            {
                case WebDialogBoxAction.Cancel:
                    return DialogBoxAction.Cancel;
                case WebDialogBoxAction.Yes:
                    return DialogBoxAction.Yes;
                case WebDialogBoxAction.No:
                    return DialogBoxAction.No;
                default:
                    return DialogBoxAction.Ok;
            }
        }

        protected override void OnCloseRequested(EventArgs e)
        {
 	         base.OnCloseRequested(e);
             if (!_isRunningModal)
                 return;
            
            if (!_dialogBox.QueryCloseReady())
                return;

            ((IWebSynchronizationContext)SynchronizationContext.Current).BreakModal();
        }

        public override void SetModelObject(object modelObject)
        {
            _dialogBox = (DialogBox)modelObject;
            _component = (IApplicationComponent)_dialogBox.Component;
            _componentView = (IWebApplicationComponentView)ViewFactory.CreateAssociatedView(_dialogBox.Component.GetType());
            _componentView.SetComponent((IApplicationComponent)_dialogBox.Component);
        }

        public override void ProcessMessage(Common.Message message)
        {
            //It is up to the hosted component to determine how the dialog box is closed, etc.
        }

        protected override void Initialize()
        {
        }

        protected override Common.Entity CreateEntity()
        {
            return new Common.Entities.DialogBox();
        }

        protected override void UpdateEntity(Common.Entity entity)
        {
            var dialogBox = (Common.Entities.DialogBox) entity;
            dialogBox.Title = _title;
            dialogBox.ApplicationComponent = _componentView.GetEntity();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing)
                return;

            if (!_isRunningModal)
                return;


            _result = _component.ExitCode == ApplicationComponentExitCode.Accepted
                             ? WebDialogBoxAction.Ok
                             : WebDialogBoxAction.Cancel;

            ((IWebSynchronizationContext)SynchronizationContext.Current).BreakModal();
        }
    }
}
