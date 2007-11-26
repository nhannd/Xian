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

using Iesi.Collections;
using ClearCanvas.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Workflow;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare.Workflow.Registration
{
    public class WorklistItemKey
    {
        private readonly EntityRef _orderRef;
        private readonly EntityRef _profileRef;

        public WorklistItemKey(EntityRef orderRef, EntityRef profileRef)
        {
            _orderRef = orderRef;
            _profileRef = profileRef;
        }

        public EntityRef OrderRef
        {
            get { return _orderRef; }
        }

        public EntityRef ProfileRef
        {
            get { return _profileRef; }
        }
    }

    public class WorklistItem : WorklistItemBase, IEquatable<WorklistItem>
    {
        private readonly HealthcardNumber _healthcardNumber;
        private readonly DateTime? _dateOfBirth;
        private readonly Sex _sex;

        /// <summary>
        /// Constructor
        /// </summary>
        public WorklistItem(
            Order order,
            Patient patient,
            PatientProfile profile,
            PatientIdentifier mrn,
            PersonName patientName,
            string accessionNumber,
            OrderPriority orderPriority,
            PatientClassEnum patientClass,
            string diagnosticServiceName,
            DateTime? scheduledStartTime,
            HealthcardNumber healthcardNumber,
            DateTime? dateOfBirth,
            Sex sex)
            : base(
                null,
                null,
                order,
                patient,
                profile,
                mrn,
                patientName,
                accessionNumber,
                orderPriority,
                patientClass,
                diagnosticServiceName,
                null,
                scheduledStartTime
            )
        {
            _healthcardNumber = healthcardNumber;
            _dateOfBirth = dateOfBirth;
            _sex = sex;
        }

        #region Public Properties

        public HealthcardNumber HealthcardNumber
        {
            get { return _healthcardNumber; }
        }

        public DateTime? DateOfBirth
        {
            get { return _dateOfBirth; }
        }

        public Sex Sex
        {
            get { return _sex; }
        }

        #endregion

        /// <summary>
        /// Overridden to be based on order rather than procedure step.
        /// </summary>
        /// <param name="worklistItem"></param>
        /// <returns></returns>
        public bool Equals(WorklistItem worklistItem)
        {
            if (worklistItem == null) return false;
            return Equals(this.OrderRef, worklistItem.OrderRef);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as WorklistItem);
        }

        /// <summary>
        /// Overridden to be based on order rather than procedure step.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.OrderRef != null ? this.OrderRef.GetHashCode() : 0;
        }
    }
}
