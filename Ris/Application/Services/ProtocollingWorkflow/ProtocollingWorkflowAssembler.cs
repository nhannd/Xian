using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services.ProtocollingWorkflow
{
    internal class ProtocollingWorkflowAssembler
    {
        public ProtocolCodeDetail CreateProtocolCodeDetail(ProtocolCode pc)
        {
            ProtocolCodeDetail detail = new ProtocolCodeDetail(pc.Name, pc.Description);
            return detail;
        }

        public ProtocolDetail CreateProtocolDetail(Protocol protocol, IPersistenceContext context)
        {
            ProtocolDetail detail = new ProtocolDetail();

            detail.Author = new StaffAssembler().CreateStaffSummary(protocol.Author, context);
            detail.ApprovalRequired = protocol.ApprovalRequired;

            detail.Codes = CollectionUtils.Map<ProtocolCode, ProtocolCodeDetail>(
                protocol.Codes, 
                delegate(ProtocolCode code) { return CreateProtocolCodeDetail(code); });

            detail.Notes = CollectionUtils.Map<ProtocolNote, ProtocolNoteDetail>(
                protocol.Notes,
                delegate(ProtocolNote note) { return CreateProtocolNoteDetail(note, context); });

            return detail;
        }

        public ProtocolNoteDetail CreateProtocolNoteDetail(ProtocolNote note, IPersistenceContext context)
        {
            StaffSummary author = new StaffAssembler().CreateStaffSummary(note.Author, context);

            return new ProtocolNoteDetail(author, note.TimeStamp, note.Text);
        }
    }
}