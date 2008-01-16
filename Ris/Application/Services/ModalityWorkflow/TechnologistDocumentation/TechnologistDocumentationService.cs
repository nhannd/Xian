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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation;
using ClearCanvas.Workflow;
using Iesi.Collections.Generic;

namespace ClearCanvas.Ris.Application.Services.ModalityWorkflow.TechnologistDocumentation
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(ITechnologistDocumentationService))]
    public class TechnologistDocumentationService : ApplicationServiceBase, ITechnologistDocumentationService
    {
        #region ITechnologistDocumentationService Members


        [ReadOperation]
        public CanCompleteOrderDocumentationResponse CanCompleteOrderDocumentation(CanCompleteOrderDocumentationRequest request)
        {
            Order order = this.PersistenceContext.Load<Order>(request.OrderRef);
            return new CanCompleteOrderDocumentationResponse(order.CanCompleteDocumentation);
        }


        [UpdateOperation]
        public SaveDataResponse SaveData(SaveDataRequest request)
        {
            Order order = PersistenceContext.Load<Order>(request.OrderRef);

            CopyProperties(order.ExtendedProperties, request.OrderExtendedProperties);

            foreach (KeyValuePair<EntityRef, Dictionary<string, string>> pair in request.PerformedProcedureStepExtendedProperties)
            {
                EntityRef ppsRef = pair.Key;
                Dictionary<string, string> extendedProperties = pair.Value;

                PerformedProcedureStep pps = PersistenceContext.Load<PerformedProcedureStep>(ppsRef);
                CopyProperties(pps.ExtendedProperties, extendedProperties);
            }

            this.PersistenceContext.SynchState();

            SaveDataResponse response = new SaveDataResponse();
            ProcedurePlanAssembler assembler = new ProcedurePlanAssembler();
            response.ProcedurePlan = assembler.CreateProcedurePlanSummary(order, this.PersistenceContext);

            return response;
        }

        [UpdateOperation]
        public CompleteOrderDocumentationResponse CompleteOrderDocumentation(CompleteOrderDocumentationRequest request)
        {
            Order order = this.PersistenceContext.Load<Order>(request.OrderRef);

            foreach (Procedure procedure in order.Procedures)
            {
                if(procedure.DocumentationProcedureStep != null && procedure.DocumentationProcedureStep.State != ActivityStatus.CM)
                {
                    procedure.DocumentationProcedureStep.Complete();
                }
            }

            this.PersistenceContext.SynchState();

            CompleteOrderDocumentationResponse response = new CompleteOrderDocumentationResponse();
            ProcedurePlanAssembler assembler = new ProcedurePlanAssembler();
            response.ProcedurePlan = assembler.CreateProcedurePlanSummary(order, this.PersistenceContext);

            return response;
        }

        #endregion


        private void CopyProperties(IDictionary<string, string> dest, IDictionary<string, string> source)
        {
            foreach (KeyValuePair<string, string> kvp in source)
            {
                dest[kvp.Key] = kvp.Value;
            }
        }
    }
}
