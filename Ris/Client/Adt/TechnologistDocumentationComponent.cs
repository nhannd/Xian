using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Adt
{
    /// <summary>
    /// Extension point for views onto <see cref="TechnologistDocumentationComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class TechnologistDocumentationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// TechnologistDocumentationComponent class
    /// </summary>
    [AssociateView(typeof(TechnologistDocumentationComponentViewExtensionPoint))]
    public class TechnologistDocumentationComponent : ApplicationComponent
    {
        /// <summary>
        /// The script callback is an object that is made available to the web browser so that
        /// the javascript code can invoke methods on the host.  It must be COM-visible.
        /// </summary>
        [ComVisible(true)]
        public class ScriptCallback
        {
            private readonly TechnologistDocumentationComponent _component;

            public ScriptCallback(TechnologistDocumentationComponent component)
            {
                _component = component;
            }

            public bool Confirm(string message, string type)
            {
                if (string.IsNullOrEmpty(type))
                    type = "okcancel";
                type = type.ToLower();

                if (type == MessageBoxActions.OkCancel.ToString().ToLower())
                {
                    return _component.Host.ShowMessageBox(message, MessageBoxActions.OkCancel) == DialogBoxAction.Ok;
                }
                else if (type == MessageBoxActions.YesNo.ToString().ToLower())
                {
                    return _component.Host.ShowMessageBox(message, MessageBoxActions.YesNo) == DialogBoxAction.Yes;
                }
                else
                {
                    throw new NotSupportedException("Type must be YesNo or OkCancel");
                }
            }

            public void Alert(string message)
            {
                _component.Host.ShowMessageBox(message, MessageBoxActions.Ok);
            }

            public string ResolveStaffName(string search)
            {
                StaffSummary staff;
                if (StaffFinder.ResolveNameInteractive(search, _component.Host.DesktopWindow, out staff))
                {
                    return PersonNameFormat.Format(staff.PersonNameDetail);
                }
                return null;
            }

            public string DateFormat
            {
                get { return Format.DateFormat; }
            }

            public string TimeFormat
            {
                get { return Format.TimeFormat; }
            }

            public string DateTimeFormat
            {
                get { return Format.DateTimeFormat; }
            }

            public string GetData(string tag)
            {
                return _component.CurrentData;
            }

            public void SetData(string tag, string data)
            {
                _component.CurrentData = data;
            }

        }

        private readonly ScriptCallback _scriptCallback;
        private IList<ProcedureStepDetail> _procedureSteps;
        private readonly TechnologistDocumentationTable _documentationTable;
        private TechnologistDocumentationTableItem _selectedItem;

        private readonly ModalityWorklistItem _workListItem;

        private readonly SimpleActionModel _technologistDocumentationActionHandler;

        private string _displatedDocumentationPageUrl;
        private string DisplayedDocumentationPageUrl
        {
            get { return _displatedDocumentationPageUrl; }
            set
            {
                _displatedDocumentationPageUrl = value;
                EventsHelper.Fire(_urlChanged, this, EventArgs.Empty);
            }
        }

        private event EventHandler _urlChanged;
        private event EventHandler _documentationDataChanged;

        private event EventHandler _beforeAccept;

        private string _currentData;

        public string CurrentData
        {
            get { return _currentData; }
            set { _currentData = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public TechnologistDocumentationComponent(ModalityWorklistItem workListItem)
        {
            _documentationTable = new TechnologistDocumentationTable();

            _scriptCallback = new ScriptCallback(this);

            _technologistDocumentationActionHandler = new SimpleActionModel(new ResourceResolver(this.GetType().Assembly));

            _workListItem = workListItem;
        }

        public TechnologistDocumentationComponent()
            : this(null)
        {
        }

        public override void Start()
        {
            _documentationTable.ItemCheckAccepted += ItemCheckAccepted;
            _documentationTable.ItemCheckRejected += ItemCheckRejected;

            _technologistDocumentationActionHandler.AddAction("Start", "Start", "StartToolSmall.png", "Start",
                delegate() { StartCheckedProcedures(); });
            _technologistDocumentationActionHandler.AddAction("Complete", "Complete", "CompleteToolSmall.png", "Complete",
                delegate() { CompleteCheckedProcedures(); });

            LoadProcedureStepsTable();

            base.Start();
        }

        public override void Stop()
        {
            _documentationTable.ItemCheckAccepted -= ItemCheckAccepted;
            _documentationTable.ItemCheckRejected -= ItemCheckRejected;

            base.Stop();
        }

        #region Presentation Model

        public Uri DocumentationPage
        {
            get { return new Uri(_displatedDocumentationPageUrl ?? "about:blank"); }
        }

        public event EventHandler DocumentationPageChanged
        {
            add { _urlChanged += value; }
            remove { _urlChanged -= value; }
        }

        public event EventHandler DocumentationDataChanged
        {
            add { _documentationDataChanged += value; }
            remove { _documentationDataChanged -= value; }
        }

        public ITable ProcedureSteps
        {
            get { return _documentationTable; }
        }

        public ActionModelNode DocumentationActionModel
        {
            get { return _technologistDocumentationActionHandler; }
        }

        public ISelection SelectedProcedureStep
        {
            get { return new Selection(_selectedItem); }
            set { _selectedItem = (TechnologistDocumentationTableItem)value.Item; }
        }

        public ScriptCallback ScriptObject
        {
            get { return _scriptCallback; }
        }

        public event EventHandler BeforeAccept
        {
            add { _beforeAccept += value; }
            remove { _beforeAccept -= value; }
        }

        #endregion

        private void ItemCheckRejected(object sender, EventArgs e)
        {
            TechnologistDocumentationTableItem item = ((TechnologistDocumentationTable.ItemCheckedEventArgs) e).Item;
            _documentationTable.Items.NotifyItemUpdated(item);
        }

        private void ItemCheckAccepted(object sender, EventArgs e)
        {
            TechnologistDocumentationTableItem checkedItem = ((TechnologistDocumentationTable.ItemCheckedEventArgs) e).Item;

            if(checkedItem.Selected)            
            {
                SelectCoDocumentedItems(checkedItem);
                UpdateTableItemCheckStatuses(checkedItem);

                if (this.DisplayedDocumentationPageUrl == null)
                {
                    this.DisplayedDocumentationPageUrl = checkedItem.ProcedureStep.DocumentationPage.Url;
                }

                if (checkedItem.ProcedureStep.PerformedProcedureStep != null)
                {
                    this.CurrentData = checkedItem.ProcedureStep.PerformedProcedureStep.Blob;
                    if (this.CurrentData != null)
                    {
                        EventsHelper.Fire(_documentationDataChanged, this, EventArgs.Empty);
                    }
                }
            }
            else if (checkedItem.Selected == false)
            {
                SaveDocumentation();
                DeselectCoDocumentedItems(checkedItem);
                ConfirmDocumentationUrlStillApplies();
            }
            else
            {
                // Do nothing - some remaining checked items specify the proper page to display
            }
        }

        private void SaveDocumentation()
        {
            EventsHelper.Fire(_beforeAccept, this, EventArgs.Empty);
            foreach (TechnologistDocumentationTableItem item in CheckedItems())
            {
                item.ProcedureStep.PerformedProcedureStep.Blob = CurrentData;
            }
        }

        private void SelectCoDocumentedItems(TechnologistDocumentationTableItem checkedItem)
        {
            foreach (TechnologistDocumentationTableItem item in CoDocumentedItems(checkedItem))
            {
                item.Selected = true;
                _documentationTable.Items.NotifyItemUpdated(item);
            }   
        }

        private void UpdateTableItemCheckStatuses(TechnologistDocumentationTableItem checkedItem)
        {
            foreach (TechnologistDocumentationTableItem item in _documentationTable.Items)
            {
                item.CanSelect = item.ProcedureStep.CanDocumentWith(checkedItem.ProcedureStep);
                _documentationTable.Items.NotifyItemUpdated(item);
            }
        }

        private void DeselectCoDocumentedItems(TechnologistDocumentationTableItem checkedItem)
        {
            foreach (TechnologistDocumentationTableItem item in CoDocumentedItems(checkedItem))
            {
                item.Selected = false;
                _documentationTable.Items.NotifyItemUpdated(item);
            }
        }

        private void ConfirmDocumentationUrlStillApplies()
        {
            bool anyItemsStillSelected = 
                CollectionUtils.Contains<TechnologistDocumentationTableItem>(
                    _documentationTable.Items,
                    delegate(TechnologistDocumentationTableItem item) { return item.Selected; });

            if(anyItemsStillSelected == false)
            {
                // make all items selectable and update display
                foreach (TechnologistDocumentationTableItem item in _documentationTable.Items)
                {
                    if (item.CanSelect == false)
                    {
                        item.CanSelect = true;
                        _documentationTable.Items.NotifyItemUpdated(item);
                    }                   
                }

                // remove documentation page
                this.DisplayedDocumentationPageUrl = null;
                this.CurrentData = null;
            }
        }

        private void LoadProcedureStepsTable()
        {
            // TODO remove dummy stuff
            if (_workListItem != null)
            {
                Platform.GetService<ITechnologistDocumentationService>(
                    delegate(ITechnologistDocumentationService service)
                        {
                            GetProcedureStepsForWorklistItemResponse response =
                                service.GetProcedureStepsForWorklistItem(
                                    new GetProcedureStepsForWorklistItemRequest(_workListItem));

                            _procedureSteps = response.ProcedureSteps;
                        });
            }
            else
            {
                _procedureSteps = new List<ProcedureStepDetail>();
                _procedureSteps.Add(new ProcedureStepDetail("Scheduled", new DocumentationPageDetail("http://localhost/RIS/nuclearmedicine.htm")));
                _procedureSteps.Add(new ProcedureStepDetail("Scheduled", new DocumentationPageDetail("http://localhost/RIS/breastimaging.htm")));
                _procedureSteps.Add(new ProcedureStepDetail("Scheduled", new DocumentationPageDetail("http://localhost/RIS/nuclearmedicine.htm")));
                _procedureSteps.Add(new ProcedureStepDetail("Scheduled", new DocumentationPageDetail("http://localhost/RIS/breastimaging.htm")));
                _procedureSteps.Add(new ProcedureStepDetail("Scheduled", new DocumentationPageDetail("http://localhost/RIS/breastimaging2.htm")));
                _procedureSteps.Add(new ProcedureStepDetail("Scheduled", new DocumentationPageDetail("http://localhost/RIS/breastimaging2.htm")));
                _procedureSteps.Add(new ProcedureStepDetail("Started", new DocumentationPageDetail("http://localhost/RIS/breastimaging2.htm")));
            }

            foreach (ProcedureStepDetail step in _procedureSteps)
            {
                _documentationTable.Items.Add(new TechnologistDocumentationTableItem(step));
            }
        }

        // "Complete" action handler
        private void CompleteCheckedProcedures()
        {
            //UpdateSelectedStatuses("Completed");
            foreach (TechnologistDocumentationTableItem item in CheckedItems())
            {
                item.ProcedureStep.Status = "Completed";
                _documentationTable.Items.NotifyItemUpdated(item);
            }
        }

        // "Start" action handler
        private void StartCheckedProcedures()
        {
            PerformedProcedureStepDetail pps = new PerformedProcedureStepDetail();

            foreach (TechnologistDocumentationTableItem item in CheckedItems())
            {
                item.ProcedureStep.Status = "Started";
                item.ProcedureStep.PerformedProcedureStep = pps;
                _documentationTable.Items.NotifyItemUpdated(item);
            }

            TechnologistDocumentationTableItem checkedItem = 
                CollectionUtils.SelectFirst<TechnologistDocumentationTableItem>(
                    _documentationTable.Items,
                    delegate(TechnologistDocumentationTableItem d) { return d.Selected; });

            UpdateTableItemCheckStatuses(checkedItem);
        }

        private IList<TechnologistDocumentationTableItem> CheckedItems()
        {
            return CollectionUtils.Select<TechnologistDocumentationTableItem, List<TechnologistDocumentationTableItem>>(
                _documentationTable.Items,
                delegate(TechnologistDocumentationTableItem d) { return d.Selected; }
                );            
        }

        private IList<TechnologistDocumentationTableItem> CoDocumentedItems(TechnologistDocumentationTableItem documentedItem)
        {
            if (documentedItem.ProcedureStep.PerformedProcedureStep == null)
            {
                return new List<TechnologistDocumentationTableItem>();
            }

            return CollectionUtils.Select<TechnologistDocumentationTableItem, List<TechnologistDocumentationTableItem>>(
                _documentationTable.Items,
                delegate(TechnologistDocumentationTableItem item)
                {
                    return item != documentedItem
                        && item.ProcedureStep.PerformedProcedureStep == documentedItem.ProcedureStep.PerformedProcedureStep;
                });
        }
    }
}
