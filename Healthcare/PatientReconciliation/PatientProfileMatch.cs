#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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
