using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
    public class ProtocolGroupAssembler
    {
        public ProtocolGroupSummary GetProtocolGroupSummary(ProtocolGroup protocolGroup)
        {
            return new ProtocolGroupSummary(protocolGroup.GetRef(), protocolGroup.Name, protocolGroup.Description);
        }

        public ProtocolGroupDetail GetProtocolGroupDetail(ProtocolGroup group, IPersistenceContext context)
        {
            List<ProtocolCodeDetail> codes = CollectionUtils.Map<ProtocolCode, ProtocolCodeDetail>(
                group.Codes,
                delegate(ProtocolCode code) { return new ProtocolCodeDetail(code.GetRef(), code.Name, code.Description);});

            RequestedProcedureTypeGroupAssembler assembler = new RequestedProcedureTypeGroupAssembler();
            List<RequestedProcedureTypeGroupSummary> readingGroups = CollectionUtils.Map<RequestedProcedureTypeGroup, RequestedProcedureTypeGroupSummary>(
                group.ReadingGroups,
                delegate(RequestedProcedureTypeGroup readingGroup) { return assembler.GetRequestedProcedureTypeGroupSummary(readingGroup, context); });
            
            return new ProtocolGroupDetail(group.Name, group.Description, codes, readingGroups);
        }

        public void UpdateProtocolGroup(ProtocolGroup group, ProtocolGroupDetail detail, IPersistenceContext context)
        {
            group.Name = detail.Name;
            group.Description = detail.Description;

            group.Codes.Clear();
            detail.Codes.ForEach(delegate(ProtocolCodeDetail protocolCodeDetail)
            {
                group.Codes.Add(context.Load<ProtocolCode>(protocolCodeDetail.EntityRef));
            });

            group.ReadingGroups.Clear();
            detail.ReadingGroups.ForEach(delegate(RequestedProcedureTypeGroupSummary requestedProcedureTypeGroupSummary)
            {
                group.ReadingGroups.Add(context.Load<RequestedProcedureTypeGroup>(requestedProcedureTypeGroupSummary.EntityRef));
            });
        }
    }
}
