using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Layout.Basic;
using ClearCanvas.ImageViewer.Web.Common.Messages;
using ClearCanvas.Web.Common;
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
