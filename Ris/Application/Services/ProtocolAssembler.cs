using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
    public class ProtocolAssembler
    {
        public ProtocolDetail CreateProtocolDetail(Protocol protocol, IPersistenceContext context)
        {
            ProtocolDetail detail = new ProtocolDetail();

            detail.Author = protocol.Author != null ? new StaffAssembler().CreateStaffSummary(protocol.Author, context) : null;
            detail.Status = EnumUtils.GetEnumValueInfo(protocol.Status, context);

            detail.Codes = protocol.Codes == null
                ? new List<ProtocolCodeDetail>()
                : CollectionUtils.Map<ProtocolCode, ProtocolCodeDetail>(protocol.Codes,
                    delegate(ProtocolCode code) { return CreateProtocolCodeDetail(code); });

            return detail;
        }

        public ProtocolCodeDetail CreateProtocolCodeDetail(ProtocolCode pc)
        {
            ProtocolCodeDetail detail = new ProtocolCodeDetail(pc.GetRef(), pc.Name, pc.Description);
            return detail;
        }
        public void UpdateProtocol(Protocol protocol, ProtocolDetail detail, IPersistenceContext context)
        {
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
