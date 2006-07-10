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
    public class PatientDetailComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    public class PatientDetailComponent : ApplicationComponent
    {
        private Patient _patient;

        public PatientDetailComponent()
        {
            _patient = Patient.New();

        }
        public void Complete()
        {
            this.Host.Complete();
        }

        public void Cancel()
        {
            this.Host.Complete();
        }
    }
}
