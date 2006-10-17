using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Client.Adt
{
    public class PatientProfileEditorDocument : Document
    {
        private PatientProfile _profile;

        public PatientProfileEditorDocument(PatientProfile profile, IDesktopWindow window)
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
            return new PatientProfileEditorComponent(_profile);
        }
   }
}
