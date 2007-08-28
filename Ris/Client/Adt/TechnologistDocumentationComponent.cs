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
using ClearCanvas.Ris.Application.Common;

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
                    return PersonNameFormat.Format(staff.Name);
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
        private List<ProcedureStepDetail> _procedureSteps;
        private readonly TechnologistDocumentationTable _documentationTable;

        private readonly ModalityWorklistItem _workListItem;

        private readonly SimpleActionModel _technologistDocumentationActionHandler;

        private string _displayedDocumentationPageUrl;
        private string DisplayedDocumentationPageUrl
        {
            get { return _displayedDocumentationPageUrl; }
            set
            {
                _displayedDocumentationPageUrl = value;
                EventsHelper.Fire(_urlChanged, this, EventArgs.Empty);
            }
        }

        private event EventHandler _urlChanged;
        private event EventHandler _documentationDataChanged;

        private event EventHandler _beforeDocumentationSaved;

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
            _documentationTable.ItemSelected += OnItemSelected;
            _documentationTable.ItemDeselected += OnItemDeselected;
            _documentationTable.ItemSelectionRejected += OnItemSelectionRejected;

            InitialiseDocumentationTableActions();

            LoadProcedureStepsTable();

            base.Start();
        }

        public override void Stop()
        {
            _documentationTable.ItemSelected -= OnItemSelected;
            _documentationTable.ItemDeselected -= OnItemDeselected;
            _documentationTable.ItemSelectionRejected -= OnItemSelectionRejected;

            base.Stop();
        }

        private void InitialiseDocumentationTableActions()
        {
            _technologistDocumentationActionHandler.AddAction("StartNow", "Start Now", "StartToolSmall.png", "Start Now",
                delegate() { StartCheckedProcedures(Platform.Time); });
            _technologistDocumentationActionHandler.AddAction("Start", "Start ...", "StartToolSmall.png", "Start ...",
                delegate()
                {
                    Platform.ShowMessageBox("Prompt for time here");
                    DateTime time = Platform.Time;
                    StartCheckedProcedures(time);
                });

            _technologistDocumentationActionHandler.AddAction("CompleteNow", "Complete Now", "CompleteToolSmall.png", "Complete Now",
                delegate() { CompleteCheckedProcedures(Platform.Time); });
            _technologistDocumentationActionHandler.AddAction("Complete", "Complete ...", "CompleteToolSmall.png", "Complete ...",
                delegate()
                {
                    Platform.ShowMessageBox("Prompt for time here");
                    DateTime time = DateTime.MaxValue;
                    CompleteCheckedProcedures(time);
                });

            ResetActionEnablement();
        }

        private void ResetActionEnablement()
        {
            _technologistDocumentationActionHandler["StartNow"].Enabled = false;
            _technologistDocumentationActionHandler["Start"].Enabled = false;
            _technologistDocumentationActionHandler["CompleteNow"].Enabled = false;
            _technologistDocumentationActionHandler["Complete"].Enabled = false;
        }

        private void UpdateActionEnablement(string checkedItemStatus)
        {
            _technologistDocumentationActionHandler["StartNow"].Enabled = checkedItemStatus == "Scheduled";
            _technologistDocumentationActionHandler["Start"].Enabled = checkedItemStatus == "Scheduled";
            _technologistDocumentationActionHandler["CompleteNow"].Enabled = checkedItemStatus == "In Progress";
            _technologistDocumentationActionHandler["Complete"].Enabled = checkedItemStatus == "In Progress";            
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
                EnumValueInfo schedStatus = new EnumValueInfo("SC", "Scheduled");
                _procedureSteps = new List<ProcedureStepDetail>();
                _procedureSteps.Add(new ProcedureStepDetail("Procedure 1", schedStatus, new DocumentationPageDetail("http://localhost/RIS/nuclearmedicine.htm")));
                _procedureSteps.Add(new ProcedureStepDetail("Procedure 2", schedStatus, new DocumentationPageDetail("http://localhost/RIS/breastimaging.htm")));
                _procedureSteps.Add(new ProcedureStepDetail("Procedure 3", schedStatus, new DocumentationPageDetail("http://localhost/RIS/nuclearmedicine.htm")));
                _procedureSteps.Add(new ProcedureStepDetail("Procedure 4", schedStatus, new DocumentationPageDetail("http://localhost/RIS/breastimaging.htm")));
                _procedureSteps.Add(new ProcedureStepDetail("Procedure 5", schedStatus, new DocumentationPageDetail("http://localhost/RIS/breastimaging2.htm")));
                _procedureSteps.Add(new ProcedureStepDetail("Procedure 6", schedStatus, new DocumentationPageDetail("http://localhost/RIS/breastimaging2.htm")));

                ProcedureStepDetail started =
                    new ProcedureStepDetail("Procedure 7", new EnumValueInfo("IP", "In Progress"), new DocumentationPageDetail("http://localhost/RIS/breastimaging2.htm"));
                started.PerformedProcedureStep = new PerformedProcedureStepDetail();
                started.PerformedProcedureStep.StartTime = Platform.Time;
                _procedureSteps.Add(started);
            }

            foreach (ProcedureStepDetail step in _procedureSteps)
            {
                _documentationTable.Items.Add(new TechnologistDocumentationTableItem(step));
            }
        }

        #region Presentation Model

        public Uri DocumentationPage
        {
            get { return new Uri(_displayedDocumentationPageUrl ?? "about:blank"); }
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

        public ScriptCallback ScriptObject
        {
            get { return _scriptCallback; }
        }

        public event EventHandler BeforeDocumentationSaved
        {
            add { _beforeDocumentationSaved += value; }
            remove { _beforeDocumentationSaved -= value; }
        }

        public void Accept()
        {
            if (this.HasValidationErrors)
            {
                this.ShowValidation(true);
            }
            else
            {
                try
                {
                    SaveChanges();
                    this.ExitCode = ApplicationComponentExitCode.Normal;
                    Host.Exit();
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, "Cannot save documentation", this.Host.DesktopWindow, 
                        delegate 
                        {
                            this.ExitCode = ApplicationComponentExitCode.Error;
                            Host.Exit();
                        });
                }
            }
        }

        private void SaveChanges()
        {
            SaveDocumentation(CheckedItems());

            Platform.GetService<ITechnologistDocumentationService>(
                delegate(ITechnologistDocumentationService service)
                {
                    DocumentProceduresRequest request = new DocumentProceduresRequest(_procedureSteps);
                    service.DocumentProcedures(request);
                });
        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.Cancelled;
            this.Host.Exit();            
        }

        #endregion

        #region TechnologistDocumentationTable Event Handlers

        private void OnItemSelectionRejected(object sender, EventArgs e)
        {
            TechnologistDocumentationTableItem item = ((TechnologistDocumentationTable.ItemCheckedEventArgs) e).Item;
            // just refresh the UI, since it still thinks the item was selected
            _documentationTable.Items.NotifyItemUpdated(item);
        }

        private void OnItemDeselected(object sender, EventArgs e)
        {
            OnItemDeselected(((TechnologistDocumentationTable.ItemCheckedEventArgs)e).Item);
        }

        private void OnItemDeselected(TechnologistDocumentationTableItem checkedItem)
        {
            IList<TechnologistDocumentationTableItem> toBeUpdated = CheckedItems();
            toBeUpdated.Add(checkedItem);
            SaveDocumentation(toBeUpdated);

            ExtendItemSelection(checkedItem, false);
            if(AnyItemsSelected() == false)
            {
                ResetPage();
            }
        }

        private void OnItemSelected(object sender, EventArgs e)
        {
            OnItemSelected(((TechnologistDocumentationTable.ItemCheckedEventArgs) e).Item);
        }

        private void OnItemSelected(TechnologistDocumentationTableItem checkedItem)
        {
            ExtendItemSelection(checkedItem, true);
            RefreshTableItemCheckStatuses(checkedItem);
            UpdateActionEnablement(checkedItem.ProcedureStep.Status.Value);
            LoadDocumentationPage(checkedItem);
        }

        private void LoadDocumentationPage(TechnologistDocumentationTableItem checkedItem)
        {
            if (checkedItem.ProcedureStep.Status.Value == "Scheduled") return;

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

        private bool AnyItemsSelected()
        {
            return CollectionUtils.Contains<TechnologistDocumentationTableItem>(
                _documentationTable.Items,
                delegate(TechnologistDocumentationTableItem item) { return item.Selected; });
        }

        /// <summary>
        /// Extends selection/deselection to any items previously documented with the selected item
        /// </summary>
        /// <param name="checkedItem">The selected item</param>
        /// <param name="selectionState">true to select, false to deselect</param>
        private void ExtendItemSelection(TechnologistDocumentationTableItem checkedItem, bool selectionState)
        {
            foreach (TechnologistDocumentationTableItem item in CoDocumentedItems(checkedItem))
            {
                item.Selected = selectionState;
                _documentationTable.Items.NotifyItemUpdated(item);
            }
        }

        private void RefreshTableItemCheckStatuses(TechnologistDocumentationTableItem checkedItem)
        {
            if (checkedItem == null) return;

            foreach (TechnologistDocumentationTableItem item in _documentationTable.Items)
            {
                item.CanSelect = item.ProcedureStep.CanDocumentWith(checkedItem.ProcedureStep);
                _documentationTable.Items.NotifyItemUpdated(item);
            }

        }

        private void SaveDocumentation(IEnumerable<TechnologistDocumentationTableItem> items)
        {
            EventsHelper.Fire(_beforeDocumentationSaved, this, EventArgs.Empty);
            foreach (TechnologistDocumentationTableItem item in items)
            {
                if (item.ProcedureStep.PerformedProcedureStep != null)
                {
                    item.ProcedureStep.Dirty = true;
                    item.ProcedureStep.PerformedProcedureStep.Blob = CurrentData;
                }
            }
        }

        private void ResetPage()
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

            ResetActionEnablement();

            // remove documentation page
            this.DisplayedDocumentationPageUrl = null;
            this.CurrentData = null;
        }

        #endregion

        // "Complete" action handler
        private void CompleteCheckedProcedures(DateTime time)
        {
            //UpdateSelectedStatuses("Completed");
            foreach (TechnologistDocumentationTableItem item in CheckedItems())
            {
                item.ProcedureStep.Status = new EnumValueInfo("CM", "Completed");
                item.ProcedureStep.Dirty = true;
                item.ProcedureStep.PerformedProcedureStep.EndTime = time;
                _documentationTable.Items.NotifyItemUpdated(item);
            }

            UpdateActionEnablement("Completed");
        }

        // "Start" action handler
        private void StartCheckedProcedures(DateTime time)
        {
            PerformedProcedureStepDetail pps = new PerformedProcedureStepDetail();
            pps.StartTime = time;

            foreach (TechnologistDocumentationTableItem item in CheckedItems())
            {
                item.ProcedureStep.Status = new EnumValueInfo("IP", "In Progress");
                item.ProcedureStep.Dirty = true;
                item.ProcedureStep.PerformedProcedureStep = pps;
                _documentationTable.Items.NotifyItemUpdated(item);
            }

            TechnologistDocumentationTableItem checkedItem =
                CollectionUtils.SelectFirst<TechnologistDocumentationTableItem>(
                    _documentationTable.Items,
                    delegate(TechnologistDocumentationTableItem d) { return d.Selected; });

            RefreshTableItemCheckStatuses(checkedItem);
            LoadDocumentationPage(checkedItem);
            UpdateActionEnablement("In Progress");
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
                        && Equals(item.ProcedureStep.PerformedProcedureStep, documentedItem.ProcedureStep.PerformedProcedureStep);
                });
        }
    }
}
