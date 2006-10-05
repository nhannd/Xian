using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise;
using ClearCanvas.Ris.Services;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Ris.Client.Adt
{
    /// <summary>
    /// Extension point for views onto <see cref="ConfirmReconciliationComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class ConfirmReconciliationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// ConfirmReconciliationComponent class
    /// </summary>
    [AssociateView(typeof(ConfirmReconciliationComponentViewExtensionPoint))]
    public class ConfirmReconciliationComponent : ApplicationComponent
    {
        private PatientProfileTableData _sourceProfiles;
        private PatientProfileTableData _targetProfiles;


        /// <summary>
        /// Constructor
        /// </summary>
        public ConfirmReconciliationComponent(Patient target, IList<Patient> sources)
        {
            IAdtService service = ApplicationContext.GetService<IAdtService>();

            _sourceProfiles = new PatientProfileTableData(service);
            foreach (Patient patient in sources)
            {
                service.LoadPatientProfiles(patient);
                _sourceProfiles.Items.AddRange(patient.Profiles);
            }

            _targetProfiles = new PatientProfileTableData(service);
            _targetProfiles.Items.AddRange(target.Profiles);
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        public ITable SourcePatientData
        {
            get { return _sourceProfiles; }
        }

        public ITable TargetPatientData
        {
            get { return _targetProfiles; }
        }

        public void Continue()
        {
            this.ExitCode = ApplicationComponentExitCode.Normal;
            this.Host.Exit();
        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.Cancelled;
            this.Host.Exit();
        }

    }
}
