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

            PatientProfileSearchCriteria high = new PatientProfileSearchCriteria();
            high.Healthcard.Id.EqualTo(targetProfile.Healthcard.Id);

            IList<PatientProfileMatch> highMatches = PatientProfileMatch.CreateList(targetProfile, broker.Find(high), PatientProfileMatch.ScoreValue.High);

            PatientProfileSearchCriteria moderateViaName = new PatientProfileSearchCriteria();

            if (targetProfile.Name.FamilyName != null && targetProfile.Name.FamilyName.Length > 0)
                moderateViaName.Name.FamilyName.EqualTo(targetProfile.Name.FamilyName);

            if (targetProfile.Name.GivenName != null && targetProfile.Name.GivenName.Length > 0)
                moderateViaName.Name.GivenName.EqualTo(targetProfile.Name.GivenName);

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
            String existingMrn = existingReconciledProfile.Mrn.AssigningAuthority;
            foreach (PatientProfileMatch proposedMatch in matches)
            {
                if (proposedMatch.PatientProfile.Mrn.AssigningAuthority == existingMrn)
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
