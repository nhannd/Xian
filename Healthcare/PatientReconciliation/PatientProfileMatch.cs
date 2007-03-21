using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Healthcare.PatientReconciliation
{
    public class PatientProfileMatch
    {
        public enum ScoreValue
        {
            High = 2,
            Moderate = 1,
            Low = 0
        }

        private PatientProfile _patientProfile;
        private ScoreValue _score;

        public PatientProfileMatch(PatientProfile patientProfile, ScoreValue score)
        {
            _patientProfile = patientProfile;
            _score = score;
        }

        public PatientProfile PatientProfile
        {
            get { return _patientProfile; }
        }
	
        public ScoreValue Score
        {
            get { return _score; }
            private set { _score = value; }
        }


        public static IList<PatientProfileMatch> CreateList(PatientProfile self, IList<PatientProfile> profileList, ScoreValue score)
        {
            IList<PatientProfileMatch> matchList = new List<PatientProfileMatch>();
            foreach (PatientProfile profile in profileList)
            {
                bool found = false;
                foreach (PatientProfile existing in self.Patient.Profiles)
                {
                    if (profile.Equals(existing))
                    {
                        found = true;
                        break;
                    }
                }
                if (found == false)
                {
                    matchList.Add(new PatientProfileMatch(profile, score));
                }
            }
            return matchList;
        }

        public static IList<PatientProfileMatch> Combine(IList<PatientProfileMatch> leftSet, IList<PatientProfileMatch> rightSet)
        {
            IList<PatientProfileMatch> result = new List<PatientProfileMatch>();

            foreach (PatientProfileMatch left in leftSet)
            {
                result.Add(left);
            }
            foreach (PatientProfileMatch right in rightSet)
            {
                bool found = false;
                foreach (PatientProfileMatch alreadyAdded in result)
                {
                    if (right.PatientProfile.Equals(alreadyAdded.PatientProfile))
                    {
                        found = true;
                        if (right.Score > alreadyAdded.Score)
                        {
                            alreadyAdded.Score = right.Score;
                        }
                        break;
                    }
                }
                if (found == false)
                {
                    result.Add(right);
                }
            }

            return result;
        }
    }
}
