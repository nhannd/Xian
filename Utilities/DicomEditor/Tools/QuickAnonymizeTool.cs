using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom;

namespace ClearCanvas.Utilities.DicomEditor.Tools
{
    [ButtonAction("activate", "dicomeditor-contextmenu/ToolbarQuickAnonymize")]
	[MenuAction("activate", "dicomeditor-toolbar/MenuQuickAnonymize")]
	[Tooltip("activate", "TooltipQuickAnonymize")]
	[IconSet("activate", IconScheme.Colour, "Icons.AnonymizeToolSmall.png", "Icons.AnonymizeToolSmall.png", "Icons.AnonymizeToolSmall.png")]
    [ClickHandler("activate", "Apply")]
    [EnabledStateObserver("activate", "Enabled", "EnabledChanged")]

    [ExtensionOf(typeof(DicomEditorToolExtensionPoint))]
    public class QuickAnonymizeTool : Tool<DicomEditorComponent.DicomEditorToolContext>
    {
        private bool _enabled;
        private event EventHandler _enabledChanged;
        private bool _promptForAll;

        /// <summary>
        /// Default constructor.  A no-args constructor is required by the
        /// framework.  Do not remove.
        /// </summary>
        public QuickAnonymizeTool()
        {
            _enabled = true;
        }

        /// <summary>
        /// Called by the framework to initialize this tool.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            this.Enabled = true;
            this.Context.DisplayedDumpChanged += new EventHandler<DisplayedDumpChangedEventArgs>(OnDisplayedDumpChanged);
        }

        /// <summary>
        /// Called to determine whether this tool is enabled/disabled in the UI.
        /// </summary>
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

        /// <summary>
        /// Notifies that the Enabled state of this tool has changed.
        /// </summary>
        public event EventHandler EnabledChanged
        {
            add { _enabledChanged += value; }
            remove { _enabledChanged -= value; }
        }

        protected void OnDisplayedDumpChanged(object sender, DisplayedDumpChangedEventArgs e)
        {
            _promptForAll = !e.IsCurrentTheOnly;
        }

        public void Apply()
        {
            

            if (_promptForAll)
            {
                if (this.Context.DesktopWindow.ShowMessageBox(SR.MessageConfirmAnonymizeAllFiles, MessageBoxActions.YesNo) == DialogBoxAction.Yes)
                {
                    this.Anonymize(true);
                }
                else
                {
                    this.Anonymize(false);
                }
            }
            else
            {
                this.Anonymize(false);
            }
            
            this.Context.UpdateDisplay();
        }

        private void Anonymize(bool applyToAll)
        {
            IDicomEditorDumpManagement dump = this.Context.DumpManagement;

            dump.RemoveAllPrivateTags(applyToAll);
            
            if (dump.TagExists(DicomTags.AccessionNumber)) 
                dump.EditTag(DicomTags.AccessionNumber, String.Empty, applyToAll);
            if (dump.TagExists(DicomTags.InstitutionName)) 
                dump.EditTag(DicomTags.InstitutionName, String.Empty, applyToAll);
            if (dump.TagExists(DicomTags.InstitutionAddress)) 
                dump.EditTag(DicomTags.InstitutionAddress, String.Empty, applyToAll);
            if (dump.TagExists(DicomTags.ReferringPhysiciansName)) 
                dump.EditTag(DicomTags.ReferringPhysiciansName, String.Empty, applyToAll);
            if (dump.TagExists(DicomTags.StationName)) 
                dump.EditTag(DicomTags.StationName, String.Empty, applyToAll);
            if (dump.TagExists(DicomTags.StudyDescription)) 
                dump.EditTag(DicomTags.StudyDescription, String.Empty, applyToAll);
            if (dump.TagExists(DicomTags.InstitutionalDepartmentName)) 
                dump.EditTag(DicomTags.InstitutionalDepartmentName, String.Empty, applyToAll);
            if (dump.TagExists(DicomTags.PhysiciansOfRecord)) 
                dump.EditTag(DicomTags.PhysiciansOfRecord, String.Empty, applyToAll);           
            if (dump.TagExists(DicomTags.PerformingPhysiciansName)) 
                dump.EditTag(DicomTags.PerformingPhysiciansName, String.Empty, applyToAll);
            if (dump.TagExists(DicomTags.NameOfPhysiciansReadingStudy)) 
                dump.EditTag(DicomTags.NameOfPhysiciansReadingStudy, String.Empty, applyToAll);
            if (dump.TagExists(DicomTags.OperatorsName)) 
                dump.EditTag(DicomTags.OperatorsName, String.Empty, applyToAll);
            if (dump.TagExists(DicomTags.AdmittingDiagnosesDescription)) 
                dump.EditTag(DicomTags.AdmittingDiagnosesDescription , String.Empty, applyToAll);
            if (dump.TagExists(DicomTags.DerivationDescription)) 
                dump.EditTag(DicomTags.DerivationDescription , String.Empty, applyToAll);
            if (dump.TagExists(DicomTags.SeriesDescription)) 
                dump.EditTag(DicomTags.SeriesDescription, String.Empty, applyToAll);
            if (dump.TagExists(DicomTags.PatientsName)) 
                dump.EditTag(DicomTags.PatientsName, String.Empty, applyToAll);
            if (dump.TagExists(DicomTags.PatientId)) 
                dump.EditTag(DicomTags.PatientId, "PatientID", applyToAll);
            if (dump.TagExists(DicomTags.PatientsBirthDate)) 
                dump.EditTag(DicomTags.PatientsBirthDate, String.Empty, applyToAll);
            if (dump.TagExists(DicomTags.PatientsBirthTime)) 
                dump.EditTag(DicomTags.PatientsBirthTime, String.Empty, applyToAll);
            if (dump.TagExists(DicomTags.PatientsSex)) 
                dump.EditTag(DicomTags.PatientsSex, String.Empty, applyToAll);
            if (dump.TagExists(DicomTags.OtherPatientIds)) 
                dump.EditTag(DicomTags.OtherPatientIds, String.Empty, applyToAll);
            if (dump.TagExists(DicomTags.OtherPatientNames)) 
                dump.EditTag(DicomTags.OtherPatientNames, String.Empty, applyToAll);
            if (dump.TagExists(DicomTags.PatientsAge)) 
                dump.EditTag(DicomTags.PatientsAge, String.Empty, applyToAll);
            if (dump.TagExists(DicomTags.PatientsSize)) 
                dump.EditTag(DicomTags.PatientsSize, String.Empty, applyToAll);
            if (dump.TagExists(DicomTags.PatientsWeight)) 
                dump.EditTag(DicomTags.PatientsWeight, String.Empty, applyToAll);
            if (dump.TagExists(DicomTags.EthnicGroup)) 
                dump.EditTag(DicomTags.EthnicGroup, String.Empty, applyToAll);
            if (dump.TagExists(DicomTags.Occupation)) 
                dump.EditTag(DicomTags.Occupation, String.Empty, applyToAll);
            if (dump.TagExists(DicomTags.PatientComments)) 
                dump.EditTag(DicomTags.PatientComments, String.Empty, applyToAll);
            if (dump.TagExists(DicomTags.AdditionalPatientHistory)) 
                dump.EditTag(DicomTags.AdditionalPatientHistory, String.Empty, applyToAll);
            if (dump.TagExists(DicomTags.DeviceSerialNumber)) 
                dump.EditTag(DicomTags.DeviceSerialNumber, String.Empty, applyToAll);
            if (dump.TagExists(DicomTags.ProtocolName)) 
                dump.EditTag(DicomTags.ProtocolName, String.Empty, applyToAll);
            if (dump.TagExists(DicomTags.StudyId)) 
                dump.EditTag(DicomTags.StudyId, String.Empty, applyToAll);
            if (dump.TagExists(DicomTags.ImageComments)) 
                dump.EditTag(DicomTags.ImageComments, String.Empty, applyToAll);
        }
    }
}
