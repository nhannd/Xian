using System;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.ImageViewer.Dicom.Core.Command;

namespace ClearCanvas.ImageViewer.Dicom.Core.Rules.AutoRoute
{
    public class InsertAutoRouteCommand : DataAccessCommand
    {
        private TransferSyntax _syntax;
        private string _aeTitle;
        private ViewerActionContext _context;
        
        public InsertAutoRouteCommand()
            : base("Insert Auto Route")
        {
        }

        public InsertAutoRouteCommand(ViewerActionContext context, string device, DateTime scheduledTime,
                                      string transferSyntaxUid)
            : this()
        {
            _aeTitle = device;
            _context = context;
            if (!string.IsNullOrEmpty(transferSyntaxUid))
            {
                _syntax = TransferSyntax.GetTransferSyntax(transferSyntaxUid);
            }
        }

        public InsertAutoRouteCommand(ViewerActionContext context, string device, string transferSyntaxUid)
            : this()
        {
            _aeTitle = device;
            _context = context;
            if (!string.IsNullOrEmpty(transferSyntaxUid))
            {
                _syntax = TransferSyntax.GetTransferSyntax(transferSyntaxUid);
            }

        }

        protected override void OnExecute(CommandProcessor theProcessor)
        {

        }
    }
}
