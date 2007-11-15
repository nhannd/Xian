#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.DiagnosticServiceAdmin;
using System.Security.Permissions;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services.Admin.DiagnosticServiceAdmin
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(IDiagnosticServiceAdminService))]
    public class DiagnosticServiceAdminService : ApplicationServiceBase, IDiagnosticServiceAdminService
    {
        #region IDiagnosticServiceAdminService Members

        [ReadOperation]
        public TextQueryResponse<DiagnosticServiceSummary> TextQuery(TextQueryRequest request)
        {
            IDiagnosticServiceBroker broker = PersistenceContext.GetBroker<IDiagnosticServiceBroker>();
            DiagnosticServiceAssembler assembler = new DiagnosticServiceAssembler();

            TextQueryHelper<DiagnosticService, DiagnosticServiceSearchCriteria, DiagnosticServiceSummary> helper
                = new TextQueryHelper<DiagnosticService, DiagnosticServiceSearchCriteria, DiagnosticServiceSummary>(
                    delegate(string rawQuery)
                    {
                        IList<string> terms = TextQueryHelper.ParseTerms(rawQuery);
                        List<DiagnosticServiceSearchCriteria> criteria = new List<DiagnosticServiceSearchCriteria>();

                        // allow matching on name (assume entire query is a name which may contain spaces)
                        DiagnosticServiceSearchCriteria nameCriteria = new DiagnosticServiceSearchCriteria();
                        nameCriteria.Name.StartsWith(rawQuery);
                        criteria.Add(nameCriteria);

                        // allow matching of any term against ID
                        criteria.AddRange(CollectionUtils.Map<string, DiagnosticServiceSearchCriteria>(terms,
                                     delegate(string term)
                                     {
                                         DiagnosticServiceSearchCriteria c = new DiagnosticServiceSearchCriteria();
                                         c.Id.StartsWith(term);
                                         return c;
                                     }));

                        return criteria.ToArray();
                    },
                    delegate(DiagnosticService ds)
                    {
                        return assembler.CreateDiagnosticServiceSummary(ds);
                    },
                    delegate(DiagnosticServiceSearchCriteria[] criteria)
                    {
                        return broker.Count(criteria);
                    },
                    delegate(DiagnosticServiceSearchCriteria[] criteria, SearchResultPage page)
                    {
                        return broker.Find(criteria, page);
                    });
            return helper.Query(request);
        }

        [ReadOperation]
        public ListDiagnosticServicesResponse ListDiagnosticServices(ListDiagnosticServicesRequest request)
        {
            DiagnosticServiceAssembler assembler = new DiagnosticServiceAssembler();

            DiagnosticServiceSearchCriteria criteria = new DiagnosticServiceSearchCriteria();
            if (!string.IsNullOrEmpty(request.DiagnosticServiceName))
                criteria.Name.StartsWith(request.DiagnosticServiceName);
            if (!string.IsNullOrEmpty(request.DiagnosticServiceId))
                criteria.Id.StartsWith(request.DiagnosticServiceId);

            return new ListDiagnosticServicesResponse(
                CollectionUtils.Map<DiagnosticService, DiagnosticServiceSummary>(
                    PersistenceContext.GetBroker<IDiagnosticServiceBroker>().Find(criteria, request.Page),
                    delegate(DiagnosticService s)
                    {
                        return assembler.CreateDiagnosticServiceSummary(s);
                    }));
        }

        #endregion
    }
}
