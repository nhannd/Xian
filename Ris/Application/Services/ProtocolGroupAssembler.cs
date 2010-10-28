#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
    public class ProtocolGroupAssembler
    {
		public ProtocolCodeSummary GetProtocolCodeSummary(ProtocolCode protocolCode)
		{
			return new ProtocolCodeSummary(
				protocolCode.GetRef(), protocolCode.Name, protocolCode.Description, protocolCode.Deactivated);
		}

		public ProtocolCodeDetail GetProtocolCodeDetail(ProtocolCode protocolCode)
		{
			return new ProtocolCodeDetail(
				protocolCode.GetRef(), protocolCode.Name, protocolCode.Description, protocolCode.Deactivated);
		}

        public ProtocolGroupSummary GetProtocolGroupSummary(ProtocolGroup protocolGroup)
        {
            return new ProtocolGroupSummary(protocolGroup.GetRef(), protocolGroup.Name, protocolGroup.Description);
        }

        public ProtocolGroupDetail GetProtocolGroupDetail(ProtocolGroup group, IPersistenceContext context)
        {
			List<ProtocolCodeSummary> codes = CollectionUtils.Map<ProtocolCode, ProtocolCodeSummary>(
                group.Codes,
				delegate(ProtocolCode code) { return GetProtocolCodeSummary(code); });

            ProcedureTypeGroupAssembler assembler = new ProcedureTypeGroupAssembler();
            List<ProcedureTypeGroupSummary> readingGroups = CollectionUtils.Map<ProcedureTypeGroup, ProcedureTypeGroupSummary>(
                group.ReadingGroups,
                delegate(ProcedureTypeGroup readingGroup) { return assembler.GetProcedureTypeGroupSummary(readingGroup, context); });
            
            return new ProtocolGroupDetail(group.Name, group.Description, codes, readingGroups);
        }

		public void UpdateProtocolCode(ProtocolCode code, ProtocolCodeDetail detail)
		{
			code.Name = detail.Name;
			code.Description = detail.Description;
			code.Deactivated = detail.Deactivated;
		}

        public void UpdateProtocolGroup(ProtocolGroup group, ProtocolGroupDetail detail, IPersistenceContext context)
        {
            group.Name = detail.Name;
            group.Description = detail.Description;

            group.Codes.Clear();
            detail.Codes.ForEach(delegate(ProtocolCodeSummary summary)
            {
				group.Codes.Add(context.Load<ProtocolCode>(summary.ProtocolCodeRef));
            });

            group.ReadingGroups.Clear();
            detail.ReadingGroups.ForEach(delegate(ProcedureTypeGroupSummary procedureTypeGroupSummary)
            {
                group.ReadingGroups.Add(context.Load<ReadingGroup>(procedureTypeGroupSummary.ProcedureTypeGroupRef));
            });
        }
    }
}
