#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Layout.Basic;
using ClearCanvas.ImageViewer.Web.Common.Entities;
using ClearCanvas.ImageViewer.Web.Common.Messages;
using ClearCanvas.Web.Common;
using ClearCanvas.Web.Common.Entities;
using ClearCanvas.Web.Services.View;

namespace ClearCanvas.ImageViewer.Web.View
{
    [ExtensionOf(typeof(LayoutChangerActionViewExtensionPoint))]
    internal class LayoutChangerActionView : StandardWebActionView
    {
        public new LayoutChangerAction Action
        {
            get { return (LayoutChangerAction)base.Action; }
        }

        public override void ProcessMessage(Message message)
        {
            var msg = message as SetLayoutActionMessage;
            if (msg != null)
                Action.SetLayout(msg.Rows, msg.Columns);
        }

        protected override Entity CreateEntity()
        {
            return new WebLayoutChangerAction();
        }

        protected override void UpdateEntity(Entity entity)
        {
            base.UpdateEntity(entity);

            var layoutChangerEntity = (WebLayoutChangerAction)entity;
            layoutChangerEntity.MaxColumns = Action.MaxColumns;
            layoutChangerEntity.MaxRows = Action.MaxRows;
            layoutChangerEntity.ActionID = Action.ActionID;
        }
    }
}
