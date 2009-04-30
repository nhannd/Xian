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
using ClearCanvas.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.PatientReconciliation
{
    [ExtensionOf(typeof(PatientReconciliationStrategyExtensionPoint))]
    public class DefaultPatientReconciliationStrategy : IPatientReconciliationStrategy
    {
        #region IPatientReconciliationStrategy Members

        public IList<PatientProfileMatch> FindReconciliationMatches(PatientProfile targetProfile, IPersistenceContext context)
        {
            /* User needs to resort to manual linking of patient records from multiple HIS when automatic MPI fails. 
             * 
             * Allow user to to select 2 or more patient records from different hospitals and merge into an MPI.  
             * 
             * Display High-Probability match/search results from muliple HIS of patients with various Mrns when 
             * field: healthcard # is matched/identical.
             * 
             * Display Moderate-Probability match/search results from multiple HIS of patients with various Mrns when fields: surname, 
             * given name, DOB, gender are matched/identical.  
             * 
             */
            IPatientProfileBroker broker = context.GetBroker<IPatientProfileBroker>();

            IList<PatientProfileMatch> matches = new List<PatientProfileMatch>();

            IList<PatientProfileMatch> highMatches = new List<PatientProfileMatch>();
			if (targetProfile.Healthcard != null && !string.IsNullOrEmpty(targetProfile.Healthcard.Id))
			{
				PatientProfileSearchCriteria high = new PatientProfileSearchCriteria();
				high.Healthcard.Id.EqualTo(targetProfile.Healthcard.Id);

				highMatches = PatientProfileMatch.CreateList(targetProfile, broker.Find(high), PatientProfileMatch.ScoreValue.High);
			}

        	PatientProfileSearchCriteria moderateViaName = new PatientProfileSearchCriteria();

            if (targetProfile.Name.FamilyName != null && !string.IsNullOrEmpty(targetProfile.Name.FamilyName))
                moderateViaName.Name.FamilyName.EqualTo(targetProfile.Name.FamilyName);

            if (targetProfile.Name.GivenName != null && !string.IsNullOrEmpty(targetProfile.Name.GivenName))
                moderateViaName.Name.GivenName.EqualTo(targetProfile.Name.GivenName);

			if (targetProfile.DateOfBirth != null)
	            moderateViaName.DateOfBirth.EqualTo(targetProfile.DateOfBirth);

            moderateViaName.Sex.EqualTo(targetProfile.Sex);

            IList<PatientProfileMatch> moderateMatchesViaName = PatientProfileMatch.CreateList(targetProfile, broker.Find(moderateViaName), PatientProfileMatch.ScoreValue.Moderate);

            matches = PatientProfileMatch.Combine(highMatches, moderateMatchesViaName);

            RemoveConflicts(targetProfile.Patient, matches);

            return matches;
        }

        #endregion

        private void RemoveConflicts(Patient patient, IList<PatientProfileMatch> matches)
        {
            IList<PatientProfileMatch> conflicts = new List<PatientProfileMatch>();
            foreach (PatientProfile existingReconciledProfile in patient.Profiles)
            {
                IdentifyConflictsForSiteFromProposedMatches(existingReconciledProfile, matches, conflicts);
            }
            foreach (PatientProfileMatch conflict in conflicts)
            {
                matches.Remove(conflict);
            }
        }

        private static void IdentifyConflictsForSiteFromProposedMatches(PatientProfile existingReconciledProfile, IList<PatientProfileMatch> matches, IList<PatientProfileMatch> conflicts)
        {
            String existingMrn = existingReconciledProfile.Mrn.AssigningAuthority.Code;
            foreach (PatientProfileMatch proposedMatch in matches)
            {
                if (proposedMatch.PatientProfile.Mrn.AssigningAuthority.Code == existingMrn)
                {
                    conflicts.Add(proposedMatch);
                    RemoveAllProfilesRelatedToConflict(proposedMatch, matches, conflicts);
                }
            }
        }

        private static void RemoveAllProfilesRelatedToConflict(PatientProfileMatch proposedMatch, IList<PatientProfileMatch> matches, IList<PatientProfileMatch> conflicts)
        {
            foreach (PatientProfileMatch otherProfilesInConflictingPatient in matches)
            {
                if (otherProfilesInConflictingPatient.PatientProfile.Patient == proposedMatch.PatientProfile.Patient)
                {
                    conflicts.Add(otherProfilesInConflictingPatient);
                }
            }
        }

    }
}
