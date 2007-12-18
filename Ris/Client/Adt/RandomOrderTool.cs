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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Client.Adt
{
    [MenuAction("apply", "folderexplorer-items-contextmenu/Random Order", "RandomOrder")]
    [ButtonAction("apply", "folderexplorer-items-toolbar/Random Order", "RandomOrder")]
    [Tooltip("apply", "Random Order")]
    [IconSet("apply", IconScheme.Colour, "AddToolSmall.png", "AddToolMedium.png", "AddToolLarge.png")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]

    [ExtensionOf(typeof(RegistrationMainWorkflowItemToolExtensionPoint))]
    [ExtensionOf(typeof(RegistrationBookingWorkflowItemToolExtensionPoint))]
    public class RandomOrderTool : Tool<IToolContext>
    {
        private bool _enabled;
        private event EventHandler _enabledChanged;

        public RandomOrderTool()
        {
            _enabled = true;
        }

        public bool Enabled
        {
            get { return _enabled; }
            protected set
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

        public void RandomOrder()
        {
            IRegistrationWorkflowItemToolContext context = (IRegistrationWorkflowItemToolContext)this.ContextBase;

            try
            {
                VisitSummary randomVisit;
                RegistrationWorklistItem item = CollectionUtils.FirstElement(context.SelectedItems);
                if (item == null)
                {
                    PatientProfileSummary profile = GetRandomPatient();
                    if (profile == null)
                        profile = RandomUtils.RandomPatientProfile();

                    randomVisit = RandomUtils.RandomVisit(profile.PatientRef, profile.PatientProfileRef, profile.Mrn.AssigningAuthority);
                }
                else
                {
                    randomVisit = RandomUtils.RandomVisit(item.PatientRef, item.PatientRef, item.Mrn.AssigningAuthority);
                }

                RandomUtils.RandomOrder(randomVisit, null);

                context.FolderSystem.InvalidateFolder(typeof(Folders.ScheduledFolder));
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, context.DesktopWindow);
            }
        }

        private static PatientProfileSummary GetRandomPatient()
        {
            char randomChar = RandomUtils.RandomAlphabet;

            PatientProfileSummary randomProfile = null;

            Platform.GetService<IRegistrationWorkflowService>(
                delegate(IRegistrationWorkflowService service)
                {
                    TextQueryRequest request = new TextQueryRequest();
                    request.TextQuery = randomChar.ToString();
                    TextQueryResponse<PatientProfileSummary> response;
                    response = service.ProfileTextQuery(request);
                    if (!response.TooManyMatches)
                        randomProfile = RandomUtils.ChooseRandom(response.Matches);

                    if (randomProfile == null)
                    {
                        // Search for all male patient, slow but works
                        request.TextQuery = "Male Female Unknown";
                        response = service.ProfileTextQuery(request);
                        randomProfile = RandomUtils.ChooseRandom(response.Matches);
                        if (!response.TooManyMatches)
                            randomProfile = RandomUtils.ChooseRandom(response.Matches);
                    }
                });

            return randomProfile;
        }

    }
}
