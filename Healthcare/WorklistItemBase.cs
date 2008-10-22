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
using System.Text;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Healthcare
{
    public class WorklistItemBase
    {
        private readonly EntityRef _procedureStepRef;
        private readonly EntityRef _procedureRef;
        private readonly EntityRef _orderRef;
        private readonly EntityRef _patientRef;
        private readonly EntityRef _profileRef;


        private readonly PatientIdentifier _mrn;
        private readonly PersonName _patientName;
        private readonly string _accessionNumber;
        private readonly OrderPriority _orderPriority;
        private readonly PatientClassEnum _patientClass;
        private readonly string _diagnosticServiceName;
        private readonly string _procedureName;
    	private readonly bool _procedurePortable;
    	private readonly Laterality _procedureLaterality;
        private readonly string _procedureStepName;
        private readonly DateTime? _time;


        public WorklistItemBase(
            ProcedureStep procedureStep,
            Procedure procedure,
            Order order,
            Patient patient,
            PatientProfile profile,
            PatientIdentifier mrn,
            PersonName patientName,
            string accessionNumber,
            OrderPriority orderPriority,
            PatientClassEnum patientClass,
            string diagnosticServiceName,
            string procedureName,
			bool procedurePortable,
			Laterality procedureLaterality,
			DateTime? time
            )
        {
            _procedureStepRef = procedureStep == null ? null : procedureStep.GetRef();
            _procedureRef = procedure == null ? null : procedure.GetRef();
            _orderRef = order == null ? null : order.GetRef();
            _patientRef = patient == null ? null : patient.GetRef();
            _profileRef = profile == null ? null : profile.GetRef();
            _mrn = mrn;
            _patientName = patientName;
            _accessionNumber = accessionNumber;
            _orderPriority = orderPriority;
            _patientClass = patientClass;
            _diagnosticServiceName = diagnosticServiceName;
            _procedureName = procedureName;
        	_procedurePortable = procedurePortable;
        	_procedureLaterality = procedureLaterality;
            _procedureStepName = procedureStep == null ? null : procedureStep.Name;
            _time = time;
        }


        public EntityRef ProcedureStepRef
        {
            get { return _procedureStepRef; }
        }

        public EntityRef ProcedureRef
        {
            get { return _procedureRef; }
        }

        public EntityRef OrderRef
        {
            get { return _orderRef; }
        }

        public EntityRef PatientRef
        {
            get { return _patientRef; }
        }

        public EntityRef PatientProfileRef
        {
            get { return _profileRef; }
        }

        public PatientIdentifier Mrn
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

        public OrderPriority OrderPriority
        {
            get { return _orderPriority; }
        }

        public PatientClassEnum PatientClass
        {
            get { return _patientClass; }
        }

        public string DiagnosticServiceName
        {
            get { return _diagnosticServiceName; }
        }

        public string ProcedureName
        {
            get { return _procedureName; }
        }

    	public bool ProcedurePortable
    	{
			get { return _procedurePortable; }
    	}

    	public Laterality ProcedureLaterality
    	{
			get { return _procedureLaterality; }
    	}

        public string ProcedureStepName
        {
            get { return _procedureStepName; }
        }

        public DateTime? Time
        {
            get { return _time; }
        }
    }
}
