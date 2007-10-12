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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.VisitAdmin;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;

namespace ClearCanvas.Ris.Client.Adt
{
    [MenuAction("apply", "folderexplorer-items-contextmenu/Random Order")]
    [ButtonAction("apply", "folderexplorer-items-toolbar/Random Order")]
    [Tooltip("apply", "Random Order")]
    [IconSet("apply", IconScheme.Colour, "AddToolSmall.png", "AddToolMedium.png", "AddToolLarge.png")]
    [ClickHandler("apply", "RandomOrder")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]

    [ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
    [ExtensionOf(typeof(RegistrationPreviewToolExtensionPoint))]
    public class RandomOrderTool : Tool<IToolContext>
    {
        private bool _enabled;
        private event EventHandler _enabledChanged;
        private readonly Random _randomizer;

        public RandomOrderTool()
        {
            _enabled = true;
            _randomizer = new Random(Platform.Time.Millisecond);
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

        private TItem ChooseRandom<TItem>(IList<TItem> target)
        {
            int randomIndex = _randomizer.Next(target.Count);
            return target[randomIndex];
        }

        private RegistrationWorklistItem GetRandomPatient()
        {
            int retryCount = 0;
            List<RegistrationWorklistItem> worklistItems = new List<RegistrationWorklistItem>();
            while (worklistItems.Count == 0 && retryCount < 10)
            {
                char ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * _randomizer.NextDouble() + 65)));

                SearchData searchData = new SearchData();
                searchData.GivenName = ch.ToString();

                Platform.GetService<IRegistrationWorkflowService>(
                    delegate(IRegistrationWorkflowService service)
                    {
                        SearchResponse response = service.Search(new SearchRequest(searchData));
                        worklistItems = response.WorklistItems;
                    });

                retryCount++;
            }
               
            return ChooseRandom(worklistItems);
        }

        public void RandomOrder()
        {
            IRegistrationWorkflowItemToolContext context = (IRegistrationWorkflowItemToolContext)this.ContextBase;

            try
            {
                RegistrationWorklistItem item = CollectionUtils.FirstElement<RegistrationWorklistItem>(context.SelectedItems);
                if (item == null)
                    item = GetRandomPatient();

                Platform.GetService<IOrderEntryService>(
                    delegate(IOrderEntryService service)
                    {
                        ListActiveVisitsForPatientRequest request = new ListActiveVisitsForPatientRequest();
                        request.PatientProfileRef = item.PatientProfileRef;

                        ListActiveVisitsForPatientResponse visitResponse = service.ListActiveVisitsForPatient(request);
                        GetOrderEntryFormDataResponse formChoicesResponse = service.GetOrderEntryFormData(new GetOrderEntryFormDataRequest());

                        VisitSummary randomVisit = ChooseRandom(visitResponse.Visits);
                        DiagnosticServiceSummary randomDS = ChooseRandom(formChoicesResponse.DiagnosticServiceChoices);
                        FacilitySummary randomFacility = ChooseRandom(formChoicesResponse.OrderingFacilityChoices);
                        ExternalPractitionerSummary randomPhysician = ChooseRandom(formChoicesResponse.OrderingPhysicianChoices);
                        EnumValueInfo randomPriority = ChooseRandom(formChoicesResponse.OrderPriorityChoices);

                        service.PlaceOrder(new PlaceOrderRequest(
                            randomVisit.Patient,
                            randomVisit.entityRef,
                            randomDS.DiagnosticServiceRef,
                            randomPriority,
                            randomPhysician.PractitionerRef,
                            randomFacility.FacilityRef,
                            true,
                            Platform.Time));
                    });

                // Refresh the schedule folder is a new folder is placed
                IFolder scheduledFolder = CollectionUtils.SelectFirst<IFolder>(context.Folders,
                    delegate(IFolder f) { return f is Folders.ScheduledFolder; });

                if (scheduledFolder != null)
                {
                    if (scheduledFolder.IsOpen)
                        scheduledFolder.Refresh();
                    else
                        scheduledFolder.RefreshCount();
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, context.DesktopWindow);
            }
        }
    }
}
