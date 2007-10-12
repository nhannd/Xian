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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Client;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Adt
{
    [MenuAction("edit1", "global-menus/Patient/Edit Patient")]
    [ButtonAction("edit1", "global-toolbars/Patient/EditPatient")]
    [ClickHandler("edit1", "Apply")]
    [EnabledStateObserver("edit1", "Enabled", "EnabledChanged")]
    [Tooltip("edit1", "Edit Patient Information")]
	[IconSet("edit1", IconScheme.Colour, "Icons.EditPatientToolSmall.png", "Icons.EditPatientToolMedium.png", "Icons.EditPatientToolLarge.png")]
    [ActionPermission("edit1", ClearCanvas.Ris.Application.Common.AuthorityTokens.PatientProfileAdmin)]

    [ButtonAction("edit3", "folderexplorer-items-toolbar/Edit")]
    [MenuAction("edit3", "folderexplorer-items-contextmenu/Edit")]
    [ClickHandler("edit3", "Apply")]
    [EnabledStateObserver("edit3", "Enabled", "EnabledChanged")]
    [Tooltip("edit3", "Edit Patient Information")]
    [IconSet("edit3", IconScheme.Colour, "Icons.EditPatientToolSmall.png", "Icons.EditPatientToolMedium.png", "Icons.EditPatientToolLarge.png")]
    [ActionPermission("edit3", ClearCanvas.Ris.Application.Common.AuthorityTokens.PatientProfileAdmin)]

    [ExtensionOf(typeof(PatientOverviewToolExtensionPoint))]
    [ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
    public class PatientEditTool : ToolBase
    {
        private bool _enabled;
        private event EventHandler _enabledChanged;

        public override void Initialize()
        {
            base.Initialize();
            if (this.ContextBase is IRegistrationWorkflowItemToolContext)
            {
                _enabled = false;   // disable by default
                ((IRegistrationWorkflowItemToolContext)this.ContextBase).SelectedItemsChanged += delegate(object sender, EventArgs args)
                {
                    this.Enabled = (((IRegistrationWorkflowItemToolContext)this.ContextBase).SelectedItems != null
                        && ((IRegistrationWorkflowItemToolContext)this.ContextBase).SelectedItems.Count == 1);
                };
            }
            else
            {
                _enabled = true;
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
        
        public void Apply()
        {
            if (this.ContextBase is IRegistrationWorkflowItemToolContext)
            {
                IRegistrationWorkflowItemToolContext context = (IRegistrationWorkflowItemToolContext)this.ContextBase;
                RegistrationWorklistItem item = CollectionUtils.FirstElement<RegistrationWorklistItem>(context.SelectedItems);
                if (Edit(item.PatientProfileRef, context.DesktopWindow))
                {
                    context.SelectedFolder.Refresh();
                }
            }
            else
            {
                IPatientOverviewToolContext context = (IPatientOverviewToolContext)this.ContextBase;
                Edit(context.PatientProfile, context.DesktopWindow);
            }
        }

        private bool Edit(EntityRef profileRef, IDesktopWindow desktopWindow)
        {
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                desktopWindow,
                new PatientProfileEditorComponent(profileRef),
                SR.TitleEditPatient);

            return exitCode == ApplicationComponentExitCode.Normal;
        }
    }
}
