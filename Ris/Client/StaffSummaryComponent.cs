using System;
using System.Collections.Generic;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Tables;

using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.StaffAdmin;

namespace ClearCanvas.Ris.Client
{
    [MenuAction("launch", "global-menus/Admin/Staff")]
    [ClickHandler("launch", "Launch")]
    [ActionPermission("launch", ClearCanvas.Ris.Application.Common.AuthorityTokens.StaffAdmin)]

    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class StaffSummaryTool : Tool<IDesktopToolContext>
    {
        private IWorkspace _workspace;

        public void Launch()
        {
            if (_workspace == null)
            {
                try
                {
                    StaffSummaryComponent component = new StaffSummaryComponent();

                    _workspace = ApplicationComponent.LaunchAsWorkspace(
                        this.Context.DesktopWindow,
                        component,
                        SR.TitleStaff,
                        delegate(IApplicationComponent c) { _workspace = null; });

                }
                catch (Exception e)
                {
                    // failed to launch component
                    ExceptionHandler.Report(e, this.Context.DesktopWindow);
                }
            }
            else
            {
                _workspace.Activate();
            }
        }
    }

    /// <summary>
    /// Extension point for views onto <see cref="StaffSummaryComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class StaffSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// StaffSummaryComponent class
    /// </summary>
    [AssociateView(typeof(StaffSummaryComponentViewExtensionPoint))]
    public class StaffSummaryComponent : ApplicationComponent
    {
        private StaffSummary _selectedStaff;
        private StaffTable _staffTable;

        private SimpleActionModel _staffActionHandler;
        private readonly string _addStaffKey = "AddStaff";
        private readonly string _updateStaffKey = "UpdateStaff";

        private PagingController<StaffSummary> _pagingController;

        private ListStaffRequest _listRequest;
        private string _firstName;
        private string _lastName;

        private bool _dialogMode;


        /// <summary>
        /// Constructor
        /// </summary>
        public StaffSummaryComponent()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dialogMode">Indicates whether the component will be shown in a dialog box or not</param>
        public StaffSummaryComponent(bool dialogMode)
        {
            _dialogMode = dialogMode;
        }

        public override void Start()
        {
            _staffTable = new StaffTable();

            _staffActionHandler = new SimpleActionModel(new ResourceResolver(this.GetType().Assembly));
            _staffActionHandler.AddAction(_addStaffKey, SR.TitleAddStaff, "Icons.AddToolSmall.png", SR.TitleAddStaff, AddStaff, ClearCanvas.Ris.Application.Common.AuthorityTokens.StaffAdmin);
            _staffActionHandler.AddAction(_updateStaffKey, SR.TitleUpdateStaff, "Icons.EditToolSmall.png", SR.TitleUpdateStaff, UpdateSelectedStaff, ClearCanvas.Ris.Application.Common.AuthorityTokens.StaffAdmin);

            InitialisePaging(_staffActionHandler);

            _listRequest = new ListStaffRequest();

            // if the last name or first name properties are valued, generate an initial search
            if (!string.IsNullOrEmpty(_lastName) || !string.IsNullOrEmpty(_firstName))
            {
                // do not handle exceptions here - allow to propagate to caller
                DoSearch();
            }

            base.Start();
        }

        private void InitialisePaging(ActionModelNode actionModelNode)
        {
            _pagingController = new PagingController<StaffSummary>(
                delegate(int firstRow, int maxRows)
                {
                    ListStaffResponse listResponse = null;

                    Platform.GetService<IStaffAdminService>(
                        delegate(IStaffAdminService service)
                        {
                            _listRequest.PageRequest.FirstRow = firstRow;
                            _listRequest.PageRequest.MaxRows = maxRows;

                            listResponse = service.ListStaff(_listRequest);
                        });

                    return listResponse.Staffs;
                }
            );

            if (actionModelNode != null)
            {
                actionModelNode.Merge(new PagingActionModel<StaffSummary>(_pagingController, _staffTable, Host.DesktopWindow));
                
            }
        }

        public override void Stop()
        {
            base.Stop();
        }

        #region Event Handler


        #endregion

        #region Presentation Model

        public bool ShowAcceptCancelButtons
        {
            get { return _dialogMode; }
            set { _dialogMode = value; }
        }

        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }

        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }

        public ITable Staffs
        {
            get { return _staffTable; }
        }

        public ActionModelNode StaffListActionModel
        {
            get { return _staffActionHandler; }
        }

        public ISelection SelectedStaff
        {
            get { return _selectedStaff == null ? Selection.Empty : new Selection(_selectedStaff); }
            set
            {
                _selectedStaff = (StaffSummary)value.Item;
                StaffSelectionChanged();
            }
        }

        public void AddStaff()
        {
            try
            {
                StaffEditorComponent editor = new StaffEditorComponent();
                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                    this.Host.DesktopWindow, editor, SR.TitleAddStaff);
                if (exitCode == ApplicationComponentExitCode.Normal)
                {
                    _staffTable.Items.Add(editor.StaffSummary);
                }
            }
            catch (Exception e)
            {
                // failed to launch editor
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        public void UpdateSelectedStaff()
        {
            try
            {
                // can occur if user double clicks while holding control
                if (_selectedStaff == null) return;

                StaffEditorComponent editor = new StaffEditorComponent(_selectedStaff.StaffRef);
                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                    this.Host.DesktopWindow, editor, SR.TitleUpdateStaff);
                if (exitCode == ApplicationComponentExitCode.Normal)
                {
                    _staffTable.Items.Replace(
                        delegate(StaffSummary s) { return s.StaffRef.Equals(editor.StaffSummary.StaffRef); },
                        editor.StaffSummary);
                }
            }
            catch (Exception e)
            {
                // failed to launch editor
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        public void DoubleClickSelectedStaff()
        {
            // double-click behaviour is different depending on whether we're running as a dialog box or not
            if (_dialogMode)
                Accept();
            else
                UpdateSelectedStaff();
        }


        public void Search()
        {
            try
            {
                DoSearch();
            }
            catch (Exception e)
            {
                // search failed
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        public bool AcceptEnabled
        {
            get { return _selectedStaff != null; }
        }

        public void Accept()
        {
            this.ExitCode = ApplicationComponentExitCode.Normal;
            this.Host.Exit();
        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.Cancelled;
            this.Host.Exit();
        }

        #endregion

        private void DoSearch()
        {
            _listRequest.FirstName = _firstName;
            _listRequest.LastName = _lastName;

            _staffTable.Items.Clear();
            _staffTable.Items.AddRange(_pagingController.GetFirst());
        }

        private void StaffSelectionChanged()
        {
            if (_selectedStaff != null)
                _staffActionHandler[_updateStaffKey].Enabled = true;
            else
                _staffActionHandler[_updateStaffKey].Enabled = false;
        }
    }
}
