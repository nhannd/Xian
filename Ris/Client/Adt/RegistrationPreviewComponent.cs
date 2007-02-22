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
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Workflow.Registration;
using ClearCanvas.Enterprise;
using ClearCanvas.Ris.Client.Common;
using ClearCanvas.Ris.Services;

namespace ClearCanvas.Ris.Client.Adt
{
    [ExtensionPoint]
    public class RegistrationPreviewToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    public interface IRegistrationPreviewToolContext : IToolContext
    {
        EntityRef<PatientProfile> PatientProfileRef { get; }
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

            public EntityRef<PatientProfile> PatientProfileRef
            {
                get { return _component.PatientProfileRef; }
            }

            public IDesktopWindow DesktopWindow
            {
                get { return _component.Host.DesktopWindow; }
            }
        }

        private bool _showHeader;
        private bool _showReconciliationAlert;

        private WorklistItem _worklistItem;
        private PatientProfile _patientProfile;

        private IWorklistService _worklistService;
        private IAdtService _adtService;

        private int _numberOfRIC;
        private int _maxRICDisplay;

        private RICTable _RIC;
        private SexEnumTable _sexChoices;

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

        public WorklistItem WorklistItem
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

        public EntityRef<PatientProfile> PatientProfileRef
        {
            get { return (_worklistItem == null ? null : _worklistItem.PatientProfile); }
        }

        public override void Start()
        {
            _worklistService = ApplicationContext.GetService<IWorklistService>();
            _adtService = ApplicationContext.GetService<IAdtService>();
            _sexChoices = _adtService.GetSexEnumTable();

            _RIC = new RICTable();
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
            _RIC.Items.Clear();
            
            if (_worklistItem != null && _worklistItem.PatientProfile != null)
            {
                int count = 0;
                int maximumItemDisplay = 5;

                _patientProfile = _adtService.LoadPatientProfile(_worklistItem.PatientProfile, true);
                IList<WorklistQueryResult> listQueryResult = (IList<WorklistQueryResult>)_worklistService.GetQueryResultForWorklistItem(_worklistItem.WorkClassName, _worklistItem);
                _numberOfRIC = listQueryResult.Count;
                
                foreach (WorklistQueryResult queryResult in listQueryResult)
                {
                    _RIC.Items.Add(queryResult);
                    count++;
                    if (count > maximumItemDisplay)
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
                return _showReconciliationAlert &&
                    _adtService.FindPatientReconciliationMatches(_worklistItem.PatientProfile).Count > 0;
            }
            set { _showReconciliationAlert = value; }
        }

        public string Name
        {
            get { return Format.Custom(_patientProfile.Name); }
        }

        public string DateOfBirth
        {
            get { return ClearCanvas.Desktop.Format.Date(_patientProfile.DateOfBirth); }
        }

        public string Mrn
        {
            get { return Format.Custom(_patientProfile.Mrn); }
        }

        public string Healthcard
        {
            get { return Format.Custom(_patientProfile.Healthcard); }
        }

        public string Sex
        {
            get { return _sexChoices[_patientProfile.Sex].Value; }
        }

        public string CurrentHomeAddress
        {
            get
            {
                Address address = _patientProfile.CurrentHomeAddress;
                return (address == null) ? SR.TextUnknownValue : Format.Custom(address);
            }
        }

        public string CurrentHomePhone
        {
            get
            {
                TelephoneNumber phone = _patientProfile.CurrentHomePhone;
                return (phone == null) ? SR.TextUnknownValue : Format.Custom(phone);
            }
        }

        public bool HasMoreBasicInfo
        {
            get 
            {
                int moreAddresses = _patientProfile.Addresses.Count - (_patientProfile.CurrentHomeAddress == null ? 0 : 1);
                int morePhoneNumbers = _patientProfile.TelephoneNumbers.Count - (_patientProfile.CurrentHomePhone == null ? 0 : 1);
                return (moreAddresses > 0 || morePhoneNumbers > 0); 
            }
        }

        public ITable RIC
        {
            get { return _RIC; }
        }

        public int MoreRICCount
        {
            get { return _numberOfRIC - _RIC.Items.Count; }
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
