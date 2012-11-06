#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Web.Common;
using ClearCanvas.Web.Common.Messages;

namespace ClearCanvas.Web.Services.View
{
    public class AlertView : WebView<Web.Common.Entities.Alert>
    {
        private DesktopWindow _desktopWindow;
        private Action<DesktopWindow> _linkAction;


        public override void SetModelObject(object modelObject)
        {
            _desktopWindow = (DesktopWindow)modelObject;
        }

        protected override void Initialize()
        {
            throw new NotImplementedException();
        }

        protected override void UpdateEntity(Web.Common.Entities.Alert entity)
        {
        }

        public void SetLinkAction(Action<DesktopWindow> a)
        {
            _linkAction = a;
        }

        public override void ProcessMessage(Message message)
        {
            var msg = message as DismissAlertMessage;
            if (msg != null)
            {
                if (msg.LinkClicked)
                {
                    try
                    {
                        if (_linkAction != null)
                        {
                            _linkAction(_desktopWindow);
                        }
                    }
                    catch (Exception e)
                    {
                        Platform.Log(LogLevel.Error, e);
                    }
                }
                ApplicationContext.EntityHandlers.Remove(this);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing && _desktopWindow != null)
            {
                _desktopWindow = null;
            }
        }
    }
}
