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
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
using AuthorityTokens=ClearCanvas.Ris.Application.Common.AuthorityTokens;

namespace ClearCanvas.Ris.Client.Workflow
{
    [MenuAction("apply", "folderexplorer-items-contextmenu/Random Order", "RandomOrder")]
    [ButtonAction("apply", "folderexplorer-items-toolbar/Random Order", "RandomOrder")]
    [Tooltip("apply", "Random Order")]
    [IconSet("apply", IconScheme.Colour, "AddToolSmall.png", "AddToolMedium.png", "AddToolLarge.png")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Development.CreateTestOrder)]

    [ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
    [ExtensionOf(typeof(BookingWorkflowItemToolExtensionPoint))]
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
                RegistrationWorklistItem item = CollectionUtils.FirstElement(context.SelectedItems);
                if (item == null)
                {
                    PatientProfileSummary profile = GetRandomPatient();
                    if (profile == null)
                        profile = RandomUtils.CreatePatient();

                    PlaceRandomOrderForPatient(profile.PatientRef, profile.PatientProfileRef, profile.Mrn.AssigningAuthority);
                }
                else
                {
                    PlaceRandomOrderForPatient(item.PatientRef, item.PatientProfileRef, item.Mrn.AssigningAuthority);
                }

				// invalidate the scheduled worklist folders
                context.InvalidateFolders(typeof(Folders.Registration.ScheduledFolder));
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, context.DesktopWindow);
            }
        }

        private void PlaceRandomOrderForPatient(EntityRef patientRef, EntityRef profileRef, EnumValueInfo informationAuthority)
        {
            // find a random active visit, or create one
            VisitSummary randomVisit = GetActiveVisitForPatient(patientRef, informationAuthority);
            if(randomVisit == null)
                randomVisit = RandomUtils.CreateVisit(patientRef, informationAuthority, 0);

            // create the order
            RandomUtils.RandomOrder(randomVisit, informationAuthority, null, 0);
        }

        private static PatientProfileSummary GetRandomPatient()
        {
            char randomChar = RandomUtils.GetRandomAlphaChar();

            PatientProfileSummary randomProfile = null;

            Platform.GetService<IRegistrationWorkflowService>(
                delegate(IRegistrationWorkflowService service)
                {
                    TextQueryRequest request = new TextQueryRequest();
                    request.TextQuery = randomChar.ToString();
                    TextQueryResponse<PatientProfileSummary> response;
                    response = service.PatientProfileTextQuery(request);
                    if (!response.TooManyMatches)
                        randomProfile = RandomUtils.ChooseRandom(response.Matches);

                    if (randomProfile == null)
                    {
                        // Search for all male patient, slow but works
                        request.TextQuery = "Male Female Unknown";
                        response = service.PatientProfileTextQuery(request);
                        randomProfile = RandomUtils.ChooseRandom(response.Matches);
                        if (!response.TooManyMatches)
                            randomProfile = RandomUtils.ChooseRandom(response.Matches);
                    }
                });

            return randomProfile;
        }

        private static VisitSummary GetActiveVisitForPatient(EntityRef patientRef, EnumValueInfo assigningAuthority)
        {
            VisitSummary visit = null;

            // choose from existing visits
            Platform.GetService<IOrderEntryService>(
                delegate(IOrderEntryService service)
                {
                    ListVisitsForPatientRequest request = new ListVisitsForPatientRequest(patientRef);

                    ListVisitsForPatientResponse visitResponse = service.ListVisitsForPatient(request);
                    visit = RandomUtils.ChooseRandom(CollectionUtils.Select(visitResponse.Visits,
                        delegate(VisitSummary summary)
                        {
                            return Equals(summary.VisitNumber.AssigningAuthority, assigningAuthority);
                        }));
                });

            return visit;
        }

    }
}
