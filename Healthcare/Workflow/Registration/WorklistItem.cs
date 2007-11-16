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
    public class WorklistItemKey : IWorklistItemKey
    {
        private EntityRef _orderRef;
        private EntityRef _profileRef;

        public WorklistItemKey(EntityRef orderRef, EntityRef profileRef)
        {
            _orderRef = orderRef;
            _profileRef = profileRef;
        }

        /// <summary>
        /// Primary Key
        /// </summary>
        public EntityRef OrderRef
        {
            get { return _orderRef; }
            set { _orderRef = value; }
        }

        public EntityRef ProfileRef
        {
            get { return _profileRef; }
            set { _profileRef = value; }
        }
    }

    public class WorklistItem : WorklistItemBase
    {
        // PatientProfile data
        private EntityRef _patientRef;
        private PatientIdentifier _mrn;
        private PersonName _patientName;
        private HealthcardNumber _healthcardNumber;
        private DateTime? _dateOfBirth;
        private Sex _sex;

        // Order data
        private DateTime? _scheduledStartTime;
        private PatientClassEnum _patientClass;
        private OrderPriority _orderPriority;
        private string _accessionNumber;
        private string _diagnosticServiceName;

        public WorklistItem(Order order)
            : base(new WorklistItemKey(order.GetRef(), null))
        {
            //TODO: choose the profile based on some location instead of visit assigning authority
            PatientProfile profile = order.Patient.Profiles.Count == 1 ?
                CollectionUtils.FirstElement<PatientProfile>(order.Patient.Profiles) :
                CollectionUtils.SelectFirst<PatientProfile>(order.Patient.Profiles,
                delegate(PatientProfile pp)
                {
                    return pp.Mrn.AssigningAuthority == order.Visit.VisitNumber.AssigningAuthority;
                });

            WorklistItemKey thisKey = (WorklistItemKey)this.Key;
            thisKey.ProfileRef = profile.GetRef();

            _patientRef = order.Patient.GetRef();
            _mrn = profile.Mrn;
            _patientName = profile.Name;
            _healthcardNumber = profile.Healthcard;
            _dateOfBirth = profile.DateOfBirth;
            _sex = profile.Sex;
            _orderPriority = order.Priority;
            _patientClass = order.Visit.PatientClass;
            _accessionNumber = order.AccessionNumber;
            _scheduledStartTime = order.ScheduledStartTime;
            _diagnosticServiceName = order.DiagnosticService.Name;
        }

        #region Public Properties

        public EntityRef ProfileRef
        {
            get { return ((WorklistItemKey)this.Key).ProfileRef; }
        }

        public EntityRef OrderRef
        {
            get { return ((WorklistItemKey)this.Key).OrderRef; }
        }

        public EntityRef PatientRef
        {
            get { return _patientRef; }
        }

        public PatientIdentifier Mrn
        {
            get { return _mrn; }
        }

        public PersonName PatientName
        {
            get { return _patientName; }
        }

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

        public DateTime? ScheduledStartTime
        {
            get { return _scheduledStartTime; }
        }

        public OrderPriority OrderPriority
        {
            get { return _orderPriority; }
        }

        public string AccessionNumber
        {
            get { return _accessionNumber; }
        }

        public string DiagnosticServiceName
        {
            get { return _diagnosticServiceName; }    
        }

        public PatientClassEnum PatientClass
        {
            get { return _patientClass; }
        }

        #endregion
    }
}
