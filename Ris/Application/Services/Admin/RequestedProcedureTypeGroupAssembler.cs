using System;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.RequestedProcedureTypeGroupAdmin;
using ClearCanvas.Ris.Application.Services.Admin.RequestedProcedureTypeGroupAdmin;

namespace ClearCanvas.Ris.Application.Services.Admin
{
    internal class RequestedProcedureTypeGroupAssembler
    {
        public RequestedProcedureTypeGroupSummary GetRequestedProcedureTypeGroupSummary(RequestedProcedureTypeGroup rptGroup, IPersistenceContext context)
        {
            EnumValueInfo category = GetCategoryEnumValueInfo(rptGroup, context);
            return new RequestedProcedureTypeGroupSummary(rptGroup.GetRef(), rptGroup.Name, rptGroup.Description, category);
        }

        public RequestedProcedureTypeGroupDetail GetRequestedProcedureTypeGroupDetail(RequestedProcedureTypeGroup rptGroup, IPersistenceContext context)
        {
            RequestedProcedureTypeGroupDetail detail = new RequestedProcedureTypeGroupDetail();

            detail.Name = rptGroup.Name;
            detail.Description = rptGroup.Description;
            detail.Category = GetCategoryEnumValueInfo(rptGroup, context);

            RequestedProcedureTypeAssembler assembler = new RequestedProcedureTypeAssembler();
            detail.RequestedProcedureTypes = CollectionUtils.Map<RequestedProcedureType, RequestedProcedureTypeSummary, List<RequestedProcedureTypeSummary>>(
                rptGroup.RequestedProcedureTypes,
                delegate (RequestedProcedureType rpt)
                    {
                        return assembler.GetRequestedProcedureTypeSummary(rpt);
                    });

            return detail;
        }

        private EnumValueInfo GetCategoryEnumValueInfo(RequestedProcedureTypeGroup rptGroup, IPersistenceContext context)
        {
            RequestedProcedureTypeGroupCategoryEnumTable table = context.GetBroker<IRequestedProcedureTypeGroupCategoryEnumBroker>().Load();
            return new EnumValueInfo(
                rptGroup.Category.ToString(),
                table[rptGroup.Category].Value,
                table[rptGroup.Category].Description);
        }

        public void UpdateRequestedProcedureTypeGroup(RequestedProcedureTypeGroup group, RequestedProcedureTypeGroupDetail detail, IPersistenceContext context)
        {
            group.Name = detail.Name;
            group.Description = detail.Description;
            group.Category = (RequestedProcedureTypeGroupCategory)Enum.Parse(typeof (RequestedProcedureTypeGroupCategory), detail.Category.Code);
            
            group.RequestedProcedureTypes.Clear();
            detail.RequestedProcedureTypes.ForEach(
                delegate(RequestedProcedureTypeSummary summary)
                    {
                        group.RequestedProcedureTypes.Add(context.Load<RequestedProcedureType>(summary.EntityRef));
                    });
        }
    }
}