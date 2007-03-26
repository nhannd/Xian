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
using ClearCanvas.Ris.Application.Common.PatientReconciliation;

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
            
            if (_worklistItem != null && _worklistItem.PatientProfileRef != null)
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

        public RegistrationWorklistPreview WorklistPreview
        {
            get { return _worklistPreview; }
        }

        public bool ShowHeader
        {
            get { return _showHeader; }
            set { _showHeader = value; }
        }

        public bool ShowReconciliationAlert
        {
            get
            {
                if (_showReconciliationAlert && _worklistPreview != null)
                {
                    try
                    {
                        bool showAlert = false;
                        Platform.GetService<IPatientReconciliationService>(
                            delegate(IPatientReconciliationService service)
                            {
                                ListPatientReconciliationMatchesResponse response = service.ListPatientReconciliationMatches(new ListPatientReconciliationMatchesRequest(_worklistPreview.PatientProfileRef));
                                showAlert = response.ReconciledProfiles.Count > 0;
                            });

                        return showAlert;
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
            //TODO: PersonNameDetail formatting
            get { return String.Format("{0}, {1}", _worklistPreview.Name.FamilyName, _worklistPreview.Name.GivenName); }
        }

        public string DateOfBirth
        {
            get { return Format.Date(_worklistPreview.DateOfBirth); }
        }

        public string Mrn
        {
            get { return String.Format("{0} {1}", _worklistPreview.MrnAssigningAuthority, _worklistPreview.MrnID); }
        }

        public string Healthcard
        {
            //TODO: HealthcardDetail formatting
            get { return String.Format("{0} {1} {2}", _worklistPreview.Healthcard.AssigningAuthority, _worklistPreview.Healthcard.Id, _worklistPreview.Healthcard.VersionCode); }
        }

        public string Sex
        {
            get { return _worklistPreview.Sex; }
        }

        public string CurrentHomeAddress
        {
            //TODO: AddressDetail formatting
            //get { return Format.Custom(_worklistPreview.CurrentHomeAddress); }
            get
            {
                if (_worklistPreview.CurrentHomeAddress == null)
                    return "";

                StringBuilder sb = new StringBuilder();
                if (!String.IsNullOrEmpty(_worklistPreview.CurrentHomeAddress.Unit))
                {
                    sb.Append(_worklistPreview.CurrentHomeAddress.Unit);
                    sb.Append("-");
                }
                sb.AppendFormat("{0}, {1} {2} {3}",
                    _worklistPreview.CurrentHomeAddress.Street,
                    _worklistPreview.CurrentHomeAddress.City,
                    _worklistPreview.CurrentHomeAddress.Province,
                    _worklistPreview.CurrentHomeAddress.PostalCode);

                return sb.ToString();
            }
        }

        public string CurrentHomePhone
        {
            //TODO: TelephoneDetail formatting
            //get { return Format.Custom(_worklistPreview.CurrentHomePhone); }
            get {
                if (_worklistPreview.CurrentHomePhone == null)
                    return "";

                StringBuilder sb = new StringBuilder();
                if (!String.IsNullOrEmpty(_worklistPreview.CurrentHomePhone.CountryCode))
                {
                    sb.Append(_worklistPreview.CurrentHomePhone.CountryCode);
                    sb.Append(" ");
                }

                sb.Append(string.Format("({0}) {1}-{2}",
                    _worklistPreview.CurrentHomePhone.AreaCode,
                    _worklistPreview.CurrentHomePhone.Number.Substring(0, 3),
                    _worklistPreview.CurrentHomePhone.Number.Substring(3)));

                if (!String.IsNullOrEmpty(_worklistPreview.CurrentHomePhone.Extension))
                {
                    sb.Append(" x");
                    sb.Append(_worklistPreview.CurrentHomePhone.Extension);
                }

                return sb.ToString();
            }
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
