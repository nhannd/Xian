using System;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.ImageViewer.Dicom.Core.Command;

namespace ClearCanvas.ImageViewer.Dicom.Core.Rules.AutoRoute
{
    public class InsertAutoRouteCommand : DataAccessCommand
    {
        public InsertAutoRouteCommand() : base("Insert Auto Route")
        {
        }

        public InsertAutoRouteCommand(ViewerActionContext context, string device, DateTime scheduledTime) : this()
        {
         
        }

        public InsertAutoRouteCommand(ViewerActionContext context, string device) : this()
        {
         
        }

        protected override void OnExecute(CommandProcessor theProcessor)
        {
         
        }
    }
}
