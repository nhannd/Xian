using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Healthcare
{
    public class PatientProfileMatch
    {
        public PatientProfileMatch(PatientProfile patientProfile, int score)
        {
            _patientProfile = patientProfile;
            _score = score;
        }

        private PatientProfile _patientProfile;
        private int _score;

        public PatientProfile PatientProfile
        {
            get { return _patientProfile; }
            set { _patientProfile = value; }
        }
	
        public int Score
        {
            get { return _score; }
            set { _score = value; }
        }
    }
}
