#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Web.Common;
using ClearCanvas.Web.Common.Entities;
using ClearCanvas.Web.Common.Messages;

namespace ClearCanvas.Web.Services.View
{
    public class MessageBoxView : WebView<MessageBox>
    {
        private DesktopWindowView _desktopWindowView;

        public override void SetModelObject(object modelObject)
		{
            _desktopWindowView = (DesktopWindowView)modelObject;
		}

        protected override void Initialize()
        {
        }

		protected override void UpdateEntity(MessageBox entity)
		{
		}

        public override void ProcessMessage(Message message)
        {
            DismissMessageBoxMessage msg = message as DismissMessageBoxMessage;
            if (msg != null)
            {
                switch (msg.Result)
                {
                    case WebDialogBoxAction.Yes:
                        _desktopWindowView.Dismiss(DialogBoxAction.Yes);
                        break;
                    case WebDialogBoxAction.No:
                        _desktopWindowView.Dismiss(DialogBoxAction.No);
                        break;
                    case WebDialogBoxAction.Ok:
                        _desktopWindowView.Dismiss(DialogBoxAction.Ok);
                        break;
                    case WebDialogBoxAction.Cancel:
                        _desktopWindowView.Dismiss(DialogBoxAction.Cancel);
                        break;
                }

                ApplicationContext.EntityHandlers.Remove(this);
            }
        }

        protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing && _desktopWindowView != null)
			{
				_desktopWindowView = null;
			}
		}
    }
}
