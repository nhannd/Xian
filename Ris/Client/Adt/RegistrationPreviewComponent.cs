using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common.Scripting;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Client;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.PatientAdmin;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Client.Adt
{
    [ExtensionPoint]
    public class RegistrationPreviewToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    public interface IRegistrationPreviewToolContext : IToolContext
    {
        RegistrationWorklistItem WorklistItem { get; }
        IDesktopWindow DesktopWindow { get; }
    }
    
    /// <summary>
    /// Extension point for views onto <see cref="RegistrationPreviewComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class RegistrationPreviewComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// RegistrationPreviewComponent class
    /// </summary>
    [AssociateView(typeof(RegistrationPreviewComponentViewExtensionPoint))]
    public class RegistrationPreviewComponent : ApplicationComponent
    {
        class RegistrationPreviewToolContext : ToolContext, IRegistrationPreviewToolContext
        {
            private RegistrationPreviewComponent _component;
            public RegistrationPreviewToolContext(RegistrationPreviewComponent component)
            {
                _component = component;
            }

            public RegistrationWorklistItem WorklistItem
            {
                get { return _component.WorklistItem; }
            }

            public IDesktopWindow DesktopWindow
            {
                get { return _component.Host.DesktopWindow; }
            }
        }

        private bool _showHeader;
        private bool _showReconciliationAlert;

        private RegistrationWorklistItem _worklistItem;
        private RegistrationWorklistPreview _worklistPreview;

        private int _maxRICDisplay;
        private RICTable _RICTable;

        private ToolSet _toolSet;

        /// <summary>
        /// Constructor
        /// </summary>
        public RegistrationPreviewComponent()
            :this(true, true)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public RegistrationPreviewComponent(bool showHeader, bool showReconciliationAlert)
        {
            _showHeader = showHeader;
            _showReconciliationAlert = showReconciliationAlert;
            _maxRICDisplay = 5;
        }

        public RegistrationWorklistItem WorklistItem
        {
            get { return _worklistItem; }
            set
            {
                _worklistItem = value;
                if (this.IsStarted)
                {
                    UpdateDisplay();
                }
            }
        }

        public int MaxRIC
        {
            get { return _maxRICDisplay; }
            set { _maxRICDisplay = value; } 
        }

        public override void Start()
        {
            _RICTable = new RICTable();
            _toolSet = new ToolSet(new RegistrationPreviewToolExtensionPoint(), new RegistrationPreviewToolContext(this));

            UpdateDisplay();
            
            base.Start();
        }

        public override void Stop()
        {
            _toolSet.Dispose();

            base.Stop();
        }

        private void UpdateDisplay()
        {
            _RICTable.Items.Clear();
            
            if (_worklistItem != null && _worklistItem.PatientProfile != null)
            {
                try
                {
                    Platform.GetService<IRegistrationWorkflowService>(
                        delegate(IRegistrationWorkflowService service)
                        {
                            LoadWorklistPreviewResponse response = service.LoadWorklistPreview(new LoadWorklistPreviewRequest(_worklistItem));
                            _worklistPreview = response.WorklistPreview;
                        });
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, this.Host.DesktopWindow);
                }

                foreach (RICSummary summary in _worklistPreview.RICs)
                {
                    _RICTable.Items.Add(summary);
                    if (_RICTable.Items.Count >= this._maxRICDisplay)
                        break;
                }
            }

            NotifyAllPropertiesChanged();
        }

        #region Presentation Model

        public bool ShowHeader
        {
            get { return _showHeader; }
            set { _showHeader = value; }
        }

        public bool ShowReconciliationAlert
        {
            get
            {
                if (_showReconciliationAlert)
                {
                    try
                    {
                        Platform.GetService<IPatientReconciliationService>(
                            delegate(IPatientReconciliationService service)
                            {
                                ListPatientReconciliationMatchesResponse response = service.ListPatientReconciliationMatches(new ListPatientReconciliationMatchesRequest(_worklistItem.PatientProfileRef));
                                return response.ReconciledProfiles.Count > 0;

                            });
                    }
                    catch (Exception e)
                    {
                        ExceptionHandler.Report(e, this.Host.DesktopWindow);
                    }
                }

                return false;
            }
            set { _showReconciliationAlert = value; }
        }

        public string Name
        {
            get { return Format.Custom(_worklistPreview.Name); }
        }

        public string DateOfBirth
        {
            get { return Format.Date(_worklistPreview.DateOfBirth); }
        }

        public string Mrn
        {
            get { return Format.Custom(_worklistPreview.Mrn); }
        }

        public string Healthcard
        {
            get { return Format.Custom(_worklistPreview.Healthcard); }
        }

        public string Sex
        {
            get { return Format.Custom(_worklistPreview.Sex.Value); }
        }

        public string CurrentHomeAddress
        {
            get { return Format.Custom(_worklistPreview.CurrentHomeAddress); }
        }

        public string CurrentHomePhone
        {
            get { return Format.Custom(_worklistPreview.CurrentHomePhone); }
        }

        public bool HasMoreBasicInfo
        {
            get 
            {
                int moreAddresses = _worklistPreview.Addresses.Count - (_worklistPreview.CurrentHomeAddress == null ? 0 : 1);
                int morePhoneNumbers = _worklistPreview.TelephoneNumbers.Count - (_worklistPreview.CurrentHomePhone == null ? 0 : 1);
                return (moreAddresses > 0 || morePhoneNumbers > 0); 
            }
        }

        public ITable RIC
        {
            get { return _RICTable; }
        }

        public int MoreRICCount
        {
            get { return _worklistPreview.RICs.Count - _RICTable.Items.Count; }
        }

        public bool HasAlert
        {
            get { return true; }
        }

        public ActionModelNode MenuModel
        {
            get { return ActionModelRoot.CreateModel(this.GetType().FullName, "RegistrationPreview-menu", _toolSet.Actions); }
        }

        #endregion
    }
}
