#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise;
using ClearCanvas.Ris.Services;

using Iesi.Collections;

namespace ClearCanvas.Utilities.RisDemoDatabaseUtilities
{
    [MenuAction("apply", "global-menus/Demo/Generate Demo Patient Replication Data...")]
    [Tooltip("apply", "Generates 10 Patient Sets that can be replicated")]
    [IconSet("apply", IconScheme.Colour, "Icons.TestPatientReplicationRecordGeneratorToolSmall.png", "Icons.TestPatientReplicationRecordGeneratorToolMedium.png", "Icons.TestPatientReplicationRecordGeneratorToolLarge.png")]
    [ClickHandler("apply", "Apply")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]

    [ExtensionOf(typeof(ClearCanvas.Desktop.DesktopToolExtensionPoint))]
    public class TestPatientReplicationRecordGeneratorTool : Tool<ClearCanvas.Desktop.IDesktopToolContext>
    {
        private bool _enabled;
        private event EventHandler _enabledChanged;

        /// <summary>
        /// Default constructor.  A no-args constructor is required by the
        /// framework.  Do not remove.
        /// </summary>
        public TestPatientReplicationRecordGeneratorTool()
        {
            _enabled = true;
        }

        /// <summary>
        /// Called by the framework to initialize this tool.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            // TODO: add any significant initialization code here rather than in the constructor
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

        /// <summary>
        /// Called by the framework when the user clicks the "apply" menu item or toolbar button.
        /// </summary>
        public void Apply()
        {
            int numberOfPatients = 10;

            RisRandomDataGenerator risGenerator = new RisRandomDataGenerator();
            Patient patient = null;

            string log = "These demo patients have been generated for reconciliation: \n \n";

            for (int i = 1; i < numberOfPatients; i++)
            {
                patient = risGenerator.GenerateReplicationPatient();

                foreach (PatientProfile profile in patient.Profiles)
                {
                    log = log + "\t" + profile.Name.FamilyName + ", " + profile.Name.GivenName + " " + profile.Name.MiddleName + "\n";
                }
            }

            Platform.ShowMessageBox(log);
            Platform.Log(log);
        }
    }
}
