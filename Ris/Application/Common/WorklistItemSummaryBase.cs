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
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;
using System.Reflection;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class WorklistItemSummaryBase : DataContractBase
    {
        public WorklistItemSummaryBase(
            EntityRef procedureStepRef,
            EntityRef procedureRef,
            EntityRef orderRef,
            EntityRef patientRef,
            EntityRef profileRef,
            CompositeIdentifierDetail mrn,
            PersonNameDetail name,
            string accessionNumber,
            EnumValueInfo orderPriority,
            EnumValueInfo patientClass,
            string diagnosticServiceName,
            string procedureName,
            string procedureStepName,
            DateTime? scheduledStartTime)
        {
            this.ProcedureStepRef = procedureStepRef;
            this.ProcedureRef = procedureRef;
            this.OrderRef = orderRef;
            this.PatientRef = patientRef;
            this.PatientProfileRef = profileRef;
            this.Mrn = mrn;
            this.PatientName = name;
            this.AccessionNumber = accessionNumber;
            this.OrderPriority = orderPriority;
            this.PatientClass = patientClass;
            this.DiagnosticServiceName = diagnosticServiceName;
            this.ProcedureName = procedureName;
            this.ProcedureStepName = procedureStepName;
            this.ScheduledStartTime = scheduledStartTime;
        }

        [DataMember]
        public EntityRef ProcedureStepRef;

        [DataMember]
        public EntityRef ProcedureRef;

        [DataMember]
        public EntityRef OrderRef;

        [DataMember]
        public EntityRef PatientRef;

        [DataMember]
        public EntityRef PatientProfileRef;

        [DataMember]
        public CompositeIdentifierDetail Mrn;

        [DataMember]
        public PersonNameDetail PatientName;

        [DataMember]
        public string AccessionNumber;

        [DataMember]
        public EnumValueInfo OrderPriority;

        [DataMember]
        public EnumValueInfo PatientClass;

        [DataMember]
        public string DiagnosticServiceName;

        [DataMember]
        public string ProcedureName;

        [DataMember]
        public string ProcedureStepName;

        [DataMember]
        public DateTime? ScheduledStartTime;

        /// <summary>
        /// Convenience method to support dynamic access to properties and fields of this class.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public object GetProperty(string property)
        {
            FieldInfo field = this.GetType().GetField(property);
            if (field != null)
                return field.GetValue(this);

            PropertyInfo prop = this.GetType().GetProperty(property);
            if (prop != null)
                return prop.GetValue(this, null);

            throw new MissingMemberException(this.GetType().Name, property);
        }

        /// <summary>
        /// Convenience method to support dynamic access to properties and fields of this class.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public T GetProperty<T>(string property)
        {
            return (T)GetProperty(property);
        }

        public override bool Equals(object obj)
        {
            WorklistItemSummaryBase that = obj as WorklistItemSummaryBase;
            if (that != null)
                return Equals(this.ProcedureStepRef, that.ProcedureStepRef);

            return false;
        }

        public override int GetHashCode()
        {
            return this.ProcedureStepRef.GetHashCode();
        }
    }
}
