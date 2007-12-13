using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Extension point for views onto <see cref="PatientSearchComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class PatientSearchComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [ExtensionPoint]
    public class PatientSearchToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    public interface IPatientSearchToolContext : IToolContext
    {
        event EventHandler SelectedProfileChanged;
        PatientProfileSummary SelectedProfile { get; }
        IDesktopWindow DesktopWindow { get; }
    }

    /// <summary>
    /// PatientSearchComponent class
    /// </summary>
    [AssociateView(typeof(PatientSearchComponentViewExtensionPoint))]
    public class PatientSearchComponent : ApplicationComponent
    {
        class PatientSearchToolContext : ToolContext, IPatientSearchToolContext
        {
            private readonly PatientSearchComponent _component;

            public PatientSearchToolContext(PatientSearchComponent component)
            {
                _component = component;
            }

            public event EventHandler SelectedProfileChanged
            {
                add { _component.SelectedProfileChanged += value; }
                remove { _component.SelectedProfileChanged -= value; }
            }

            public PatientProfileSummary SelectedProfile
            {
                get { return (PatientProfileSummary)_component.SelectedProfile.Item; }
            }

            public IDesktopWindow DesktopWindow
            {
                get { return _component.Host.DesktopWindow; }
            }
        }

        private string _searchString;

        private PatientProfileTable _profileTable;
        private PatientProfileSummary _selectedProfile;
        private event EventHandler _selectedProfileChanged;

        private PagingController<PatientProfileSummary> _pagingController;
        private PagingActionModel<PatientProfileSummary> _pagingActionHandler;

        private ToolSet _toolSet;

        public override void Start()
        {
            _profileTable = new PatientProfileTable();
            _toolSet = new ToolSet(new PatientSearchToolExtensionPoint(), new PatientSearchToolContext(this));

            InitialisePaging();
            
            base.Start();
        }

        public override void Stop()
        {
            _toolSet.Dispose();

            base.Stop();
        }

        private void InitialisePaging()
        {
            _pagingController = new PagingController<PatientProfileSummary>(
                delegate(int firstRow, int maxRows)
                {
                    TextQueryResponse<PatientProfileSummary> response = null;

                    Platform.GetService<IRegistrationWorkflowService>(
                        delegate(IRegistrationWorkflowService service)
                        {
                            TextQueryRequest request = new TextQueryRequest();
                            request.TextQuery = _searchString;
                            request.Page.FirstRow = firstRow;
                            request.Page.MaxRows = maxRows;
                            response = service.ProfileTextQuery(request);
                        });


                    return response.Matches;
                }
            );

            _pagingActionHandler = new PagingActionModel<PatientProfileSummary>(_pagingController, _profileTable, Host.DesktopWindow);
        }

        #region Presentation Model

        public string SearchString
        {
            get { return _searchString; }
            set { _searchString = value; }
        }

        public bool SearchEnabled
        {
            get { return !String.IsNullOrEmpty(_searchString); }
        }

        public ITable Profiles
        {
            get { return _profileTable; }
        }

        public ActionModelNode ItemsContextMenuModel
        {
            get
            {
                ActionModelRoot model = ActionModelRoot.CreateModel(this.GetType().FullName, "patientsearch-items-contextmenu", _toolSet.Actions);
                model.Merge(_pagingActionHandler);
                return model;
            }
        }

        public ActionModelNode ItemsToolbarModel
        {
            get
            {
                ActionModelRoot model = ActionModelRoot.CreateModel(this.GetType().FullName, "patientsearch-items-toolbar", _toolSet.Actions);
                model.Merge(_pagingActionHandler);
                return model;
            }
        }

        public override IActionSet ExportedActions
        {
            get { return _toolSet.Actions; }
        }

        public ISelection SelectedProfile
        {
            get { return new Selection(_selectedProfile); }
            set
            {
                if (!Equals(_selectedProfile, value.Item))
                {
                    _selectedProfile = (PatientProfileSummary)value.Item;
                    EventsHelper.Fire(_selectedProfileChanged, this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler SelectedProfileChanged
        {
            add { _selectedProfileChanged += value; }
            remove { _selectedProfileChanged -= value; }
        }

        public void Search()
        {
            try
            {
                _profileTable.Items.Clear();
                _profileTable.Items.AddRange(_pagingController.GetFirst());
                this.SelectedProfile = new Selection(_profileTable.Items.Count > 0 ? _profileTable.Items[0] : null);
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        public void OpenPatient()
        {
            try
            {
                Workspace workspace = DocumentManager.Get<PatientBiographyDocument>(_selectedProfile.ProfileRef);
                if (workspace == null)
                {
                    Document doc = new PatientBiographyDocument(_selectedProfile.PatientRef, _selectedProfile.ProfileRef, this.Host.DesktopWindow);
                    doc.Open();
                }
                else
                {
                    workspace.Activate();
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        #endregion
    }
}
