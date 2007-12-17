#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

using System;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.RequestedProcedureTypeGroupAdmin;

namespace ClearCanvas.Ris.Application.Services
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
                        return assembler.CreateRequestedProcedureTypeSummary(rpt);
                    });

            return detail;
        }

        private EnumValueInfo GetCategoryEnumValueInfo(RequestedProcedureTypeGroup rptGroup, IPersistenceContext context)
        {
            return EnumUtils.GetEnumValueInfo(rptGroup.Category, context);
        }

        public void UpdateRequestedProcedureTypeGroup(RequestedProcedureTypeGroup group, RequestedProcedureTypeGroupDetail detail, IPersistenceContext context)
        {
            group.Name = detail.Name;
            group.Description = detail.Description;
            group.Category = EnumUtils.GetEnumValue<RequestedProcedureTypeGroupCategory>(detail.Category);
            
            group.RequestedProcedureTypes.Clear();
            detail.RequestedProcedureTypes.ForEach(
                delegate(RequestedProcedureTypeSummary summary)
                    {
                        group.RequestedProcedureTypes.Add(context.Load<RequestedProcedureType>(summary.EntityRef));
                    });
        }
    }
}