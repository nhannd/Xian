using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client.Adt
{
    public class PatientOverviewDocument : Document
    {
        private PatientProfile _profile;

        public PatientOverviewDocument(PatientProfile profile, IDesktopWindow window)
            :base(profile.OID, window)
        {
            _profile = profile;
        }

        protected override string GetTitle()
        {
            return string.Format(SR.PatientComponentTitle, _profile.Name.Format(), _profile.MRN.Format());
        }

        protected override IApplicationComponent GetComponent()
        {
            return new PatientOverviewComponent(_profile);
        }
    }
}
