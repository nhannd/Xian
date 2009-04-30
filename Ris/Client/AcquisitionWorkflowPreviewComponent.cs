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

using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Extension point for views onto <see cref="AcquisitionWorkflowPreviewComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class AcquisitionWorkflowPreviewComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// AcquisitionWorkflowPreviewComponent class
    /// </summary>
    [AssociateView(typeof(AcquisitionWorkflowPreviewComponentViewExtensionPoint))]
    public class AcquisitionWorkflowPreviewComponent : ApplicationComponent
    {
        private WorklistQueryResult _worklistItem;
        private ModalityProcedureStep _scheduledProcStep;
        private PatientProfile _patientProfile;

        private IModalityWorkflowService _workflowService;

        /// <summary>
        /// Constructor
        /// </summary>
        public AcquisitionWorkflowPreviewComponent()
        {
        }

        public WorklistQueryResult WorklistItem
        {
            get { return _worklistItem; }
            set
            {
                _worklistItem = value;
                Refresh();
            }
        }

        public override void Start()
        {
            _workflowService = ApplicationContext.GetService<IModalityWorkflowService>();
            _sexChoices = _workflowService.GetSexEnumTable();
            _orderPriorities = _workflowService.GetOrderPriorityEnumTable();

            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        private void Refresh()
        {
            if (_worklistItem != null)
            {
                _scheduledProcStep = _workflowService.LoadWorklistItemPreview(_worklistItem);

                _patientProfile = CollectionUtils.SelectFirst<PatientProfile>(
                    _scheduledProcStep.RequestedProcedure.Order.Patient.Profiles,
                    delegate(PatientProfile pp) { return _worklistItem.PatientProfile.RefersTo(pp); });

            }
            
            NotifyAllPropertiesChanged();
        }

        #region Presentation Model

        public string Name
        {
            get { return Format.Custom(_patientProfile.Name); }
        }

        public string DateOfBirth
        {
            get { return Format.Date(_patientProfile.DateOfBirth); }
        }

        public string Mrn
        {
            get { return Format.Custom(_patientProfile.Mrn); }
        }

        public string Healthcard
        {
            get { return Format.Custom(_patientProfile.Healthcard); }
        }

        public string Sex
        {
            get { return _sexChoices[_patientProfile.Sex].Value; }
        }

        public string AccessionNumber
        {
            get { return _scheduledProcStep.RequestedProcedure.Order.AccessionNumber; }
        }

        public string DiagnosticServiceName
        {
            get { return _scheduledProcStep.RequestedProcedure.Order.DiagnosticService.Name; }
        }

        public string OrderingFacility
        {
            get { return _scheduledProcStep.RequestedProcedure.Order.OrderingFacility.Name; }
        }

        public string OrderPriority
        {
            get { return _orderPriorities[_scheduledProcStep.RequestedProcedure.Order.Priority].Value; }
        }

        public string ProcedureType
        {
            get { return _scheduledProcStep.RequestedProcedure.Type.Name; }
        }

        #endregion
    }
}
