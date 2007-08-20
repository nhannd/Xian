using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.VisitAdmin;
using System.Collections;
using ClearCanvas.Ris.Application.Common.Admin;

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
        private Random _randomizer;

        public RandomOrderTool()
        {
            _enabled = true;
            _randomizer = new Random(Platform.Time.Millisecond);
        }

        public override void Initialize()
        {
            base.Initialize();
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
            List<RegistrationWorklistItem> worklistItems = new List<RegistrationWorklistItem>();

            while (worklistItems.Count == 0)
            {
                char ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * _randomizer.NextDouble() + 65)));

                PatientProfileSearchData searchData = new PatientProfileSearchData();
                searchData.GivenName = ch.ToString();

                Platform.GetService<IRegistrationWorkflowService>(
                    delegate(IRegistrationWorkflowService service)
                    {
                        SearchPatientResponse response = service.SearchPatient(new SearchPatientRequest(searchData));
                        worklistItems = response.WorklistItems;
                    });
            }
               
            return ChooseRandom<RegistrationWorklistItem>(worklistItems);
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
                        ListActiveVisitsForPatientResponse visitResponse = service.ListActiveVisitsForPatient(new ListActiveVisitsForPatientRequest(item.PatientProfileRef));
                        GetOrderEntryFormDataResponse formChoicesResponse = service.GetOrderEntryFormData(new GetOrderEntryFormDataRequest());

                        VisitSummary randomVisit = ChooseRandom<VisitSummary>(visitResponse.Visits);
                        DiagnosticServiceSummary randomDS = ChooseRandom<DiagnosticServiceSummary>(formChoicesResponse.DiagnosticServiceChoices);
                        FacilitySummary randomFacility = ChooseRandom<FacilitySummary>(formChoicesResponse.OrderingFacilityChoices);
                        StaffSummary randomPhysician = ChooseRandom<StaffSummary>(formChoicesResponse.OrderingPhysicianChoices);
                        EnumValueInfo randomPriority = ChooseRandom<EnumValueInfo>(formChoicesResponse.OrderPriorityChoices);

                        PlaceOrderResponse response = service.PlaceOrder(new PlaceOrderRequest(
                            randomVisit.Patient,
                            randomVisit.entityRef,
                            randomDS.DiagnosticServiceRef,
                            randomPriority,
                            randomPhysician.StaffRef,
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
