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
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare.Workflow.Modality
{
    public class WorklistItemKey : IWorklistItemKey
    {
        private EntityRef _modalityProcedureStep;

        public WorklistItemKey(EntityRef modalityProcedureStep)
        {
            _modalityProcedureStep = modalityProcedureStep;
        }

        public EntityRef ModalityProcedureStep
        {
            get { return _modalityProcedureStep; }
            set { _modalityProcedureStep = value; }
        }
    }

    public class WorklistItem : WorklistItemBase
    {
        private EntityRef _profileRef;
        private EntityRef _patientRef;
        private EntityRef _orderRef;

        private CompositeIdentifier _mrn;
        private PersonName _patientName;
        private string _accessionNumber;
        private ModalityProcedureStepType _modalityProcedureStepType;
        private RequestedProcedureType _requestedProcedureType;
        private OrderPriority _priority;
        private ClearCanvas.Healthcare.Modality _modality;

        public WorklistItem(
            ModalityProcedureStep modalityProcedureStep,
            Order order,
            Patient patient,
            PatientProfile profile,
            string accessionNumber,
            OrderPriority priority,
            RequestedProcedureType requestedProcedureType,
            ModalityProcedureStepType modalityProcedureStepType,
            ClearCanvas.Healthcare.Modality modality)
            : base(new WorklistItemKey(modalityProcedureStep.GetRef()))
        {
            _orderRef = order.GetRef();
            _patientRef = patient.GetRef();
            _profileRef = profile.GetRef();
            _mrn = profile.Mrn;
            _patientName = profile.Name;
            _accessionNumber = accessionNumber;
            _priority = priority;
            _requestedProcedureType = requestedProcedureType;
            _modalityProcedureStepType = modalityProcedureStepType;
            _modality = modality;
        }


        public EntityRef ModalityProcedureStepRef
        {
            get { return (this.Key as WorklistItemKey).ModalityProcedureStep; }
        }

        public EntityRef PatientProfileRef
        {
            get { return _profileRef; }
        }

        public CompositeIdentifier Mrn
        {
            get { return _mrn; }
        }

        public PersonName PatientName
        {
            get { return _patientName; }
        }

        public string AccessionNumber
        {
            get { return _accessionNumber; }
        }

        public OrderPriority Priority
        {
            get { return _priority; }
        }

        public RequestedProcedureType RequestedProcedureType
        {
            get { return _requestedProcedureType; }
        }

        public ModalityProcedureStepType ModalityProcedureStepType
        {
            get { return _modalityProcedureStepType; }
        }

        public ClearCanvas.Healthcare.Modality Modality
        {
            get { return _modality; }
        }
    }
}
