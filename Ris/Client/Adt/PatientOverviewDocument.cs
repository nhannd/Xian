using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Ris.Client.Adt
{
    public class PatientOverviewDocument : Document
    {
        private EntityRef<PatientProfile> _profileRef;

        public PatientOverviewDocument(EntityRef<PatientProfile> profileRef, IDesktopWindow window)
            :base(profileRef.ToString(), window)
        {
            _profileRef = profileRef;
        }

        protected override string GetTitle()
        {
            return SR.TitlePatientProfile;   // doesn't matter, cause the component will set the title when it starts
        }

        protected override IApplicationComponent GetComponent()
        {
            return new PatientOverviewComponent(_profileRef);
        }
    }
}
