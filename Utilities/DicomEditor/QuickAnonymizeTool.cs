using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Utilities.DicomEditor
{
    [MenuAction("activate", "dicomeditor-toolbar/Quick Anonymize")]
    [ButtonAction("activate", "dicomeditor-contextmenu/Quick Anonymize")]
    [Tooltip("activate", "Anonymize all loaded files with one click")]
    [IconSet("activate", IconScheme.Colour, "Icons.QuickAnonymizeToolSmall.png", "Icons.QuickAnonymizeToolMedium.png", "Icons.QuickAnonymizeToolLarge.png")]
    [ClickHandler("activate", "Apply")]
    [EnabledStateObserver("activate", "Enabled", "EnabledChanged")]

    [ExtensionOf(typeof(DicomEditorToolExtensionPoint))]
    public class QuickAnonymizeTool : Tool<DicomEditorComponent.DicomEditorToolContext>
    {
        private bool _enabled;
        private event EventHandler _enabledChanged;        

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
            FillSupplement55AnonymizeList();
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

        public void Apply()
        {
            //maybe there's a more elegant way to do this....
            this.Context.DumpManagement.RemoveAllPrivateTags();

            foreach (DicomEditorTag tag in _supplement55AnonymizeTagList)
            {
                if (this.Context.DisplayedDump.TagExists(tag.Key))
                {
                    //UGLY EXCEPTION - Since PatientID is a Type 1 Attribute - it cannot be null
                    if (tag.Key.DisplayKey == "(0010,0020)")
                    {
                        tag.Value = "PatientID ";
                        tag.Length = 10;
                    }

                    this.Context.DumpManagement.ApplyEdit(tag, EditType.Update, true);
                }
            }
            this.Context.UpdateDisplay();
        }

        private void FillSupplement55AnonymizeList()
        {
            //hardcoded from Supplement 55
            _supplement55AnonymizeTagList = new List<DicomEditorTag>();
            _supplement55AnonymizeTagList.Add(new DicomEditorTag(ushort.Parse("8", System.Globalization.NumberStyles.HexNumber),ushort.Parse("50", System.Globalization.NumberStyles.HexNumber), "AccessionNumber", "SH",0, null, null, DisplayLevel.Attribute));
            _supplement55AnonymizeTagList.Add(new DicomEditorTag(ushort.Parse("8", System.Globalization.NumberStyles.HexNumber),ushort.Parse("80", System.Globalization.NumberStyles.HexNumber), "InstitutionName", "LO",0, null, null, DisplayLevel.Attribute));
            _supplement55AnonymizeTagList.Add(new DicomEditorTag(ushort.Parse("8", System.Globalization.NumberStyles.HexNumber),ushort.Parse("81", System.Globalization.NumberStyles.HexNumber), "InstitutionAddress", "ST",0, null, null, DisplayLevel.Attribute));
            _supplement55AnonymizeTagList.Add(new DicomEditorTag(ushort.Parse("8", System.Globalization.NumberStyles.HexNumber),ushort.Parse("90", System.Globalization.NumberStyles.HexNumber), "ReferringPhysiciansName", "PN",0, null, null, DisplayLevel.Attribute));
            _supplement55AnonymizeTagList.Add(new DicomEditorTag(ushort.Parse("8", System.Globalization.NumberStyles.HexNumber),ushort.Parse("1010", System.Globalization.NumberStyles.HexNumber), "StationName", "SH",0, null, null, DisplayLevel.Attribute));
            _supplement55AnonymizeTagList.Add(new DicomEditorTag(ushort.Parse("8", System.Globalization.NumberStyles.HexNumber),ushort.Parse("1030", System.Globalization.NumberStyles.HexNumber), "StudyDescription", "LO",0, null, null, DisplayLevel.Attribute));
            _supplement55AnonymizeTagList.Add(new DicomEditorTag(ushort.Parse("8", System.Globalization.NumberStyles.HexNumber),ushort.Parse("1040", System.Globalization.NumberStyles.HexNumber), "InstitutionalDepartmentName", "LO",0, null, null, DisplayLevel.Attribute));
            _supplement55AnonymizeTagList.Add(new DicomEditorTag(ushort.Parse("8", System.Globalization.NumberStyles.HexNumber),ushort.Parse("1048", System.Globalization.NumberStyles.HexNumber), "PhysiciansOfRecord", "PN",0, null, null, DisplayLevel.Attribute));
            _supplement55AnonymizeTagList.Add(new DicomEditorTag(ushort.Parse("8", System.Globalization.NumberStyles.HexNumber),ushort.Parse("1050", System.Globalization.NumberStyles.HexNumber), "PerformingPhysiciansName", "PN",0, null, null, DisplayLevel.Attribute));
            _supplement55AnonymizeTagList.Add(new DicomEditorTag(ushort.Parse("8", System.Globalization.NumberStyles.HexNumber),ushort.Parse("1060", System.Globalization.NumberStyles.HexNumber), "NameofPhysiciansReadingStudy", "PN",0, null, null, DisplayLevel.Attribute));
            _supplement55AnonymizeTagList.Add(new DicomEditorTag(ushort.Parse("8", System.Globalization.NumberStyles.HexNumber),ushort.Parse("1070", System.Globalization.NumberStyles.HexNumber), "OperatorsName", "PN",0, null, null, DisplayLevel.Attribute));
            _supplement55AnonymizeTagList.Add(new DicomEditorTag(ushort.Parse("8", System.Globalization.NumberStyles.HexNumber),ushort.Parse("1080", System.Globalization.NumberStyles.HexNumber), "AdmittingDiagnosesDescription", "LO",0, null, null, DisplayLevel.Attribute));
            _supplement55AnonymizeTagList.Add(new DicomEditorTag(ushort.Parse("8", System.Globalization.NumberStyles.HexNumber),ushort.Parse("2111", System.Globalization.NumberStyles.HexNumber), "DerivationDescription", "ST",0, null, null, DisplayLevel.Attribute));
            _supplement55AnonymizeTagList.Add(new DicomEditorTag(ushort.Parse("8", System.Globalization.NumberStyles.HexNumber),ushort.Parse(" 103E", System.Globalization.NumberStyles.HexNumber), "SeriesDescription", "LO",0, null, null, DisplayLevel.Attribute));
            _supplement55AnonymizeTagList.Add(new DicomEditorTag(ushort.Parse("10", System.Globalization.NumberStyles.HexNumber),ushort.Parse("10", System.Globalization.NumberStyles.HexNumber), "PatientsName", "PN",0, null, null, DisplayLevel.Attribute));
            _supplement55AnonymizeTagList.Add(new DicomEditorTag(ushort.Parse("10", System.Globalization.NumberStyles.HexNumber),ushort.Parse("20", System.Globalization.NumberStyles.HexNumber), "PatientID", "LO",0, null, null, DisplayLevel.Attribute));
            _supplement55AnonymizeTagList.Add(new DicomEditorTag(ushort.Parse("10", System.Globalization.NumberStyles.HexNumber),ushort.Parse("30", System.Globalization.NumberStyles.HexNumber), "PatientsBirthDate", "DA",0, null, null, DisplayLevel.Attribute));
            _supplement55AnonymizeTagList.Add(new DicomEditorTag(ushort.Parse("10", System.Globalization.NumberStyles.HexNumber),ushort.Parse("32", System.Globalization.NumberStyles.HexNumber), "PatientsBirthTime", "TM",0, null, null, DisplayLevel.Attribute));
            _supplement55AnonymizeTagList.Add(new DicomEditorTag(ushort.Parse("10", System.Globalization.NumberStyles.HexNumber),ushort.Parse("40", System.Globalization.NumberStyles.HexNumber), "PatientsSex", "CS",0, null, null, DisplayLevel.Attribute));
            _supplement55AnonymizeTagList.Add(new DicomEditorTag(ushort.Parse("10", System.Globalization.NumberStyles.HexNumber),ushort.Parse("1000", System.Globalization.NumberStyles.HexNumber), "OtherPatientIDs", "LO",0, null, null, DisplayLevel.Attribute));
            _supplement55AnonymizeTagList.Add(new DicomEditorTag(ushort.Parse("10", System.Globalization.NumberStyles.HexNumber),ushort.Parse("1001", System.Globalization.NumberStyles.HexNumber), "OtherPatientNames", "PN",0, null, null, DisplayLevel.Attribute));
            _supplement55AnonymizeTagList.Add(new DicomEditorTag(ushort.Parse("10", System.Globalization.NumberStyles.HexNumber),ushort.Parse("1010", System.Globalization.NumberStyles.HexNumber), "PatientsAge", "AS",0, null, null, DisplayLevel.Attribute));
            _supplement55AnonymizeTagList.Add(new DicomEditorTag(ushort.Parse("10", System.Globalization.NumberStyles.HexNumber),ushort.Parse("1020", System.Globalization.NumberStyles.HexNumber), "PatientsSize", "DS",0, null, null, DisplayLevel.Attribute));
            _supplement55AnonymizeTagList.Add(new DicomEditorTag(ushort.Parse("10", System.Globalization.NumberStyles.HexNumber),ushort.Parse("1030", System.Globalization.NumberStyles.HexNumber), "PatientsWeight", "DS",0, null, null, DisplayLevel.Attribute));
            _supplement55AnonymizeTagList.Add(new DicomEditorTag(ushort.Parse("10", System.Globalization.NumberStyles.HexNumber),ushort.Parse("2160", System.Globalization.NumberStyles.HexNumber), "EthnicGroup", "SH",0, null, null, DisplayLevel.Attribute));
            _supplement55AnonymizeTagList.Add(new DicomEditorTag(ushort.Parse("10", System.Globalization.NumberStyles.HexNumber),ushort.Parse("2180", System.Globalization.NumberStyles.HexNumber), "Occupation", "SH",0, null, null, DisplayLevel.Attribute));
            _supplement55AnonymizeTagList.Add(new DicomEditorTag(ushort.Parse("10", System.Globalization.NumberStyles.HexNumber),ushort.Parse("4000", System.Globalization.NumberStyles.HexNumber), "PatientComments", "LT",0, null, null, DisplayLevel.Attribute));
            _supplement55AnonymizeTagList.Add(new DicomEditorTag(ushort.Parse("10", System.Globalization.NumberStyles.HexNumber),ushort.Parse(" 21B0", System.Globalization.NumberStyles.HexNumber), "AdditionalPatientsHistory", "LT",0, null, null, DisplayLevel.Attribute));
            _supplement55AnonymizeTagList.Add(new DicomEditorTag(ushort.Parse("18", System.Globalization.NumberStyles.HexNumber),ushort.Parse("1000", System.Globalization.NumberStyles.HexNumber), "DeviceSerialNumber", "LO",0, null, null, DisplayLevel.Attribute));
            _supplement55AnonymizeTagList.Add(new DicomEditorTag(ushort.Parse("18", System.Globalization.NumberStyles.HexNumber),ushort.Parse("1030", System.Globalization.NumberStyles.HexNumber), "ProtocolName", "LO",0, null, null, DisplayLevel.Attribute));
            _supplement55AnonymizeTagList.Add(new DicomEditorTag(ushort.Parse("20", System.Globalization.NumberStyles.HexNumber),ushort.Parse("10", System.Globalization.NumberStyles.HexNumber), "StudyID", "SH",0, null, null, DisplayLevel.Attribute));
            _supplement55AnonymizeTagList.Add(new DicomEditorTag(ushort.Parse("20", System.Globalization.NumberStyles.HexNumber),ushort.Parse("4000", System.Globalization.NumberStyles.HexNumber), "ImageComments", "LT",0, null, null, DisplayLevel.Attribute));
        }

        private List<DicomEditorTag> _supplement55AnonymizeTagList;
        
    }
}
