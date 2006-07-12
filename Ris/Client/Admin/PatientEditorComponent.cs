using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Presentation;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Client.Admin
{
    [ExtensionPoint()]
    public class PatientEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [ApplicationComponentView(typeof(PatientEditorComponentViewExtensionPoint))]
    public class PatientEditorComponent : ApplicationComponent
    {
        private Patient _patient;

        public PatientEditorComponent()
        {
            _patient = Patient.New();

        }

        public void Accept()
        {
            SaveChanges();
            Host.Exit();
        }

        public void Cancel()
        {
            DiscardChanges();
            Host.Exit();
        }

        public override bool CanExit()
        {
            MessageBoxResult result = this.Host.ShowMessageBox("Save changes before closing?", MessageBoxButtons.YesNoCancel);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    SaveChanges();
                    return true;
                case MessageBoxResult.No:
                    DiscardChanges();
                    return true;
                default:
                    return false;
            }
        }

        private void SaveChanges()
        {
            // TODO save data here

            this.ExitCode = ApplicationComponentExitCode.Normal;
        }

        private void DiscardChanges()
        {
            this.ExitCode = ApplicationComponentExitCode.Cancelled;
        }
    }
}
