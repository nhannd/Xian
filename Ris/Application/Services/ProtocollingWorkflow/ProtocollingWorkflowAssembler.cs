using System;
using System.Collections.Generic;
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
            ProtocolCodeDetail detail = new ProtocolCodeDetail(pc.GetRef(), pc.Name, pc.Description);
            return detail;
        }

        public ProtocolDetail CreateProtocolDetail(Protocol protocol, IPersistenceContext context)
        {
            ProtocolDetail detail = new ProtocolDetail();

            detail.Author = protocol.Author != null ? new StaffAssembler().CreateStaffSummary(protocol.Author, context) : null;
            detail.Status = EnumUtils.GetEnumValueInfo(protocol.Status, context);

            detail.Codes = protocol.Codes == null
                ? new List<ProtocolCodeDetail>()
                : CollectionUtils.Map<ProtocolCode, ProtocolCodeDetail>(protocol.Codes, 
                    delegate(ProtocolCode code) { return CreateProtocolCodeDetail(code); });

            detail.Notes = protocol.Notes == null
                ? new List<ProtocolNoteDetail>()
                : CollectionUtils.Map<ProtocolNote, ProtocolNoteDetail>(
                    protocol.Notes,
                    delegate(ProtocolNote note) { return CreateProtocolNoteDetail(note, context); });

            return detail;
        }

        public ProtocolNoteDetail CreateProtocolNoteDetail(ProtocolNote note, IPersistenceContext context)
        {
            StaffSummary author = new StaffAssembler().CreateStaffSummary(note.Author, context);

            return new ProtocolNoteDetail(author, note.TimeStamp, note.Text);
        }

        public void UpdateProtocol(Protocol protocol, ProtocolDetail detail, IPersistenceContext context)
        {
            //protocol.ApprovalRequired = detail.ApprovalRequired;

            if (detail.Author != null && detail.Author.StaffRef != null)
            {
                protocol.Author = context.Load<Staff>(detail.Author.StaffRef);
            }

            protocol.Codes.Clear();
            foreach (ProtocolCodeDetail codeDetail in detail.Codes)
            {
                ProtocolCode code = context.Load<ProtocolCode>(codeDetail.EntityRef, EntityLoadFlags.Proxy);
                protocol.Codes.Add(code);
            }
        }

        public ProtocolGroupSummary CreateProtocolGroupSummary(ProtocolGroup group)
        {
            return new ProtocolGroupSummary(group.GetRef(), group.Name, group.Description);
        }

        public ProtocolGroupDetail CreateProtocolGroupDetail(ProtocolGroup group, IPersistenceContext context)
        {
            List<ProtocolCodeDetail> codes = CollectionUtils.Map<ProtocolCode, ProtocolCodeDetail>(
                group.Codes,
                delegate(ProtocolCode code) { return CreateProtocolCodeDetail(code); });

            RequestedProcedureTypeGroupAssembler assembler = new RequestedProcedureTypeGroupAssembler();
            List<RequestedProcedureTypeGroupSummary> groups =
                CollectionUtils.Map<RequestedProcedureTypeGroup, RequestedProcedureTypeGroupSummary>(
                    group.ReadingGroups,
                    delegate(RequestedProcedureTypeGroup readingGroup) { return assembler.GetRequestedProcedureTypeGroupSummary(readingGroup, context); });

            return new ProtocolGroupDetail(group.Name, group.Description, codes, groups);
        }
    }
}