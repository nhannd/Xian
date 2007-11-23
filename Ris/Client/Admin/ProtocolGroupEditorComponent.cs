using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.ProtocolAdmin;

namespace ClearCanvas.Ris.Client.Admin
{
    /// <summary>
    /// Extension point for views onto <see cref="ProtocolGroupEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class ProtocolGroupEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// ProtocolGroupEditorComponent class
    /// </summary>
    [AssociateView(typeof(ProtocolGroupEditorComponentViewExtensionPoint))]
    public class ProtocolGroupEditorComponent : ApplicationComponent
    {
        #region Private fields

        private EntityRef _protocolGroupRef;
        private ProtocolGroupSummary _protocolGroupSummary;
        private ProtocolGroupDetail _protocolGroupDetail;

        private readonly bool _isNew;

        private ProtocolCodeTable _availableProtocolCodes;
        private ProtocolCodeTable _selectedProtocolCodes;

        private RequestedProcedureTypeGroupSummaryTable _availableReadingGroups;
        private RequestedProcedureTypeGroupSummaryTable _selectedReadingGroups;


        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public ProtocolGroupEditorComponent()
        {
            _isNew = true;
        }

        public ProtocolGroupEditorComponent(EntityRef protocolGroupRef)
        {
            _isNew = false;
            _protocolGroupRef = protocolGroupRef;
        }

        #endregion

        #region ApplicationComponent overrides

        public override void Start()
        {
            _availableProtocolCodes = new ProtocolCodeTable();
            _selectedProtocolCodes = new ProtocolCodeTable();

            _availableReadingGroups = new RequestedProcedureTypeGroupSummaryTable();
            _selectedReadingGroups = new RequestedProcedureTypeGroupSummaryTable();

            Platform.GetService<IProtocolAdminService>(
                delegate(IProtocolAdminService service)
                    {
                        GetProtocolGroupEditFormDataRequest request = new GetProtocolGroupEditFormDataRequest();
                        GetProtocolGroupEditFormDataResponse editFormDataResponse = service.GetProtocolGroupEditFormData(request);
                        
                        _availableProtocolCodes.Items.AddRange(editFormDataResponse.ProtocolCodes);
                        _availableReadingGroups.Items.AddRange(editFormDataResponse.ReadingGroups);

                        if(_isNew)
                        {
                            _protocolGroupDetail = new ProtocolGroupDetail();
                        }
                        else
                        {
                            LoadProtocolGroupForEditResponse response =
                                service.LoadProtocolGroupForEdit(new LoadProtocolGroupForEditRequest(_protocolGroupRef));

                            _protocolGroupDetail = response.Detail;

                            _selectedProtocolCodes.Items.AddRange(_protocolGroupDetail.Codes);
                            _selectedReadingGroups.Items.AddRange(_protocolGroupDetail.ReadingGroups);
                        }

                        foreach (ProtocolCodeDetail item in _selectedProtocolCodes.Items)
                        {
                            _availableProtocolCodes.Items.Remove(item);
                        }

                        foreach (RequestedProcedureTypeGroupSummary item in _selectedReadingGroups.Items)
                        {
                            _availableReadingGroups.Items.Remove(item);
                        }
                    });

            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        #endregion

        #region Public Properties

        public ProtocolGroupSummary ProtocolGroupSummary
        {
            get { return _protocolGroupSummary; }
        }
        #endregion

        #region Presentation Model

        public string Name
        {
            get { return _protocolGroupDetail.Name; }
            set
            {
                _protocolGroupDetail.Name = value;
                this.Modified = true;
            }
        }

        public string Description
        {
            get { return _protocolGroupDetail.Description; }
            set
            {
                _protocolGroupDetail.Description = value;
                this.Modified = true;
            }
        }

        public ITable AvailableProtocolCodes
        {
            get { return _availableProtocolCodes; }
        }

        public ITable SelectedProtocolCodes
        {
            get { return _selectedProtocolCodes; }
        }

        public ITable AvailableReadingGroups
        {
            get { return _availableReadingGroups; }
        }

        public ITable SelectedReadingGroups
        {
            get { return _selectedReadingGroups; }
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
                    this.Exit(ApplicationComponentExitCode.Accepted);
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, SR.ExceptionSaveProtocolGroup, this.Host.DesktopWindow,
                        delegate()
                        {
                            this.ExitCode = ApplicationComponentExitCode.Error;
                            this.Host.Exit();
                        });
                }
            }
        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.None;
            Host.Exit();
        }

        public void ItemsAddedOrRemoved()
        {
            this.Modified = true;
        }

        public bool AcceptEnabled
        {
            get { return this.Modified; }
        }

        #endregion

        #region Private methods

        private void SaveChanges()
        {
            _protocolGroupDetail.Codes.Clear();
            _protocolGroupDetail.Codes.AddRange(_selectedProtocolCodes.Items);

            _protocolGroupDetail.ReadingGroups.Clear();
            _protocolGroupDetail.ReadingGroups.AddRange(_selectedReadingGroups.Items);

            Platform.GetService<IProtocolAdminService>(
                delegate(IProtocolAdminService service)
                {
                    if (_isNew)
                    {
                        AddProtocolGroupResponse response = service.AddProtocolGroup(new AddProtocolGroupRequest(_protocolGroupDetail));
                        _protocolGroupRef = response.Summary.EntityRef;
                        _protocolGroupSummary = response.Summary;
                    }
                    else
                    {
                        UpdateProtocolGroupResponse response = service.UpdateProtocolGroup(new UpdateProtocolGroupRequest(_protocolGroupRef, _protocolGroupDetail));
                        _protocolGroupRef = response.Summary.EntityRef;
                        _protocolGroupSummary = response.Summary;
                    }
                });
        }

        public event EventHandler AcceptEnabledChanged
        {
            add { this.ModifiedChanged += value; }
            remove { this.ModifiedChanged -= value; }
        }

        #endregion

        public void AddNewCode()
        {
            try
            {
                ProtocolCodeEditorComponent editor = new ProtocolCodeEditorComponent();
                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                    this.Host.DesktopWindow, editor, SR.TitleAddProtocolCode);
                if (exitCode == ApplicationComponentExitCode.Accepted)
                {
                    _selectedProtocolCodes.Items.Add(editor.ProtocolCode);
                    this.Modified = true;
                }
            }
            catch (Exception e)
            {
                // could not launch editor
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
            ;
        }
    }
}
