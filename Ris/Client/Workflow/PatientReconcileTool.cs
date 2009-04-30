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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
    [MenuAction("apply", "folderexplorer-items-contextmenu/Reconcile Patients", "Reconcile")]
    [ButtonAction("apply", "folderexplorer-items-toolbar/Reconcile Patients", "Reconcile")]
    [Tooltip("apply", "Reconcile patient profiles")]
	[IconSet("apply", IconScheme.Colour, "Icons.ReconcilePatientToolSmall.png", "Icons.ReconcilePatientToolMedium.png", "Icons.ReconcilePatientToolLarge.png")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
    [ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Patient.Reconcile)]

    [ExtensionOf(typeof(PatientBiographyToolExtensionPoint))]
    [ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
    [ExtensionOf(typeof(BookingWorkflowItemToolExtensionPoint))]
    public class PatientReconcileTool : ToolBase
    {
        private bool _enabled;
        private event EventHandler _enabledChanged;

        public override void Initialize()
        {
            base.Initialize();
            if (this.ContextBase is IRegistrationWorkflowItemToolContext)
            {
                _enabled = false;   // disable by default
                ((IRegistrationWorkflowItemToolContext)this.ContextBase).SelectionChanged += delegate
                {
                    this.Enabled = (((IRegistrationWorkflowItemToolContext)this.ContextBase).SelectedItems != null
                        && ((IRegistrationWorkflowItemToolContext)this.ContextBase).SelectedItems.Count == 1);
                };
            }
            else
            {
                _enabled = true;    // always enabled
            }
        }

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    EventsHelper.Fire(_enabledChanged, this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler EnabledChanged
        {
            add { _enabledChanged += value; }
            remove { _enabledChanged -= value; }
        }

        public void Reconcile()
        {
            if (this.ContextBase is IRegistrationWorkflowItemToolContext)
            {
                IRegistrationWorkflowItemToolContext context = (IRegistrationWorkflowItemToolContext)this.ContextBase;
                RegistrationWorklistItem item = CollectionUtils.FirstElement<RegistrationWorklistItem>(context.SelectedItems);
                if (ShowReconciliationDialog(item.PatientProfileRef, context.DesktopWindow))
                {
                    context.InvalidateSelectedFolder();
                }
            }
            else
            {
                IPatientBiographyToolContext context = (IPatientBiographyToolContext)this.ContextBase;
                ShowReconciliationDialog(context.PatientProfileRef, context.DesktopWindow);
            }
        }

        private bool ShowReconciliationDialog(EntityRef patientProfile, IDesktopWindow desktopWindow)
        {
            try
            {
                return PatientReconciliation.ShowReconciliationDialog(patientProfile, desktopWindow);
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, desktopWindow);
                return false;
            }
        }
    }
}
