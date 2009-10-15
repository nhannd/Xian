#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System.Collections.Generic;
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
			var detail = new ProtocolDetail();
			var staffAssembler = new StaffAssembler();

			detail.ProtocolRef = protocol.GetRef();
			detail.Author = protocol.Author != null ? staffAssembler.CreateStaffSummary(protocol.Author, context) : null;
			detail.Supervisor = protocol.Supervisor != null ? staffAssembler.CreateStaffSummary(protocol.Supervisor, context) : null;
			detail.Status = EnumUtils.GetEnumValueInfo(protocol.Status, context);
			detail.Urgency = EnumUtils.GetEnumValueInfo(protocol.Urgency);
			detail.RejectReason = EnumUtils.GetEnumValueInfo(protocol.RejectReason);

			detail.Codes = protocol.Codes == null
				? new List<ProtocolCodeSummary>()
				: CollectionUtils.Map<ProtocolCode, ProtocolCodeSummary>(protocol.Codes, CreateProtocolCodeSummary);

			var procedureAssembler = new ProcedureAssembler();
			detail.Procedures = protocol.Procedures == null
				? new List<ProcedureDetail>()
				: CollectionUtils.Map<Procedure, ProcedureDetail>(
					protocol.Procedures,
					procedure => procedureAssembler.CreateProcedureDetail(procedure, delegate { return false; }, false, context));

			return detail;
		}

		public ProtocolCodeDetail CreateProtocolCodeDetail(ProtocolCode pc)
		{
			return new ProtocolCodeDetail(pc.GetRef(), pc.Name, pc.Description, pc.Deactivated);
		}

		public ProtocolCodeSummary CreateProtocolCodeSummary(ProtocolCode code)
		{
			return new ProtocolCodeSummary(code.GetRef(), code.Name, code.Description, code.Deactivated);
		}

		public void UpdateProtocol(Protocol protocol, ProtocolDetail detail, IPersistenceContext context)
		{
			protocol.Urgency = EnumUtils.GetEnumValue<ProtocolUrgencyEnum>(detail.Urgency, context);

			// if detail specifies a supervisor use it otherwise retain previous supervisor (null or valued)
			protocol.Supervisor = detail.Supervisor != null ? context.Load<Staff>(detail.Supervisor.StaffRef) : protocol.Supervisor;

			protocol.Codes.Clear();
			foreach (var codeSummary in detail.Codes)
			{
				var code = context.Load<ProtocolCode>(codeSummary.ProtocolCodeRef, EntityLoadFlags.Proxy);
				protocol.Codes.Add(code);
			}
		}

		public ProtocolGroupSummary CreateProtocolGroupSummary(ProtocolGroup group)
		{
			return new ProtocolGroupSummary(group.GetRef(), group.Name, group.Description);
		}

		public ProtocolGroupDetail CreateProtocolGroupDetail(ProtocolGroup group, IPersistenceContext context)
		{
			var codes = CollectionUtils.Map<ProtocolCode, ProtocolCodeSummary>(
				group.Codes,
				CreateProtocolCodeSummary);

			var assembler = new ProcedureTypeGroupAssembler();
			var groups = CollectionUtils.Map<ProcedureTypeGroup, ProcedureTypeGroupSummary>(
				group.ReadingGroups,
				readingGroup => assembler.GetProcedureTypeGroupSummary(readingGroup, context));

			return new ProtocolGroupDetail(group.Name, group.Description, codes, groups);
		}
	}
}
