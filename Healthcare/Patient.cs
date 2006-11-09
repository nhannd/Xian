using System;
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// Patient entity
    /// </summary>
	public partial class Patient : Entity
	{
        private void CustomInitialize()
        {
        }

        public override bool Equals(object obj)
        {
            // according to NHibernate we should be using business-key equality here
            // however, we don't have a business key, so we really don't have a choice
            // we should be ok as long as we don't put transient Patient objects into a Hashtable or Set
            // (and there is really no reason to do so)
            Patient that = obj as Patient;
            return that != null && that.OID == this.OID;
        }

        public override int GetHashCode()
        {
            // according to NHibernate we should be using business-key equality here
            // however, we don't have a business key, so we really don't have a choice
            // we should be ok as long as we don't put transient Patient objects into a Hashtable or Set
            // (and there is really no reason to do so)
            return this.OID.GetHashCode();
        }

        /// <summary>
        /// Adds a profile to this patient, setting the profile's <see cref="PatientPrrofile.Patient"/> property
        /// to refer to this object.  Use this method rather than referring to the <see cref="Patient.Profiles"/>
        /// collection directly.
        /// </summary>
        /// <param name="profile"></param>
        public void AddProfile(PatientProfile profile)
        {
            if (profile.Patient != null)
            {
                //NB: technically we should remove the profile from the other patient's collection, but there
                //seems to be a bug with NHibernate where it deletes the profile if we do this
                //profile.Patient.Profiles.Remove(profile);
            }
            profile.Patient = this;
            this.Profiles.Add(profile);
        }

        /// <summary>
        /// Reconciles the specified patient to this patient
        /// </summary>
        /// <param name="other"></param>
        public void Reconcile(Patient other)
        {
            if (PatientIdentifierConflictsFound(other))
                throw new PatientReconciliationException("assigning authority conflict - cannot reconcile");
            
            // Move profiles from the other patient to this patient
            ArrayList otherProfiles = new ArrayList(other.Profiles);
            foreach (PatientProfile profile in otherProfiles)
            {
                this.AddProfile(profile);
            }
        }

        /// <summary>
        /// Returns true if any profiles for the other patient and any profiles for this patient
        /// have an Mrn with the same assigning authority.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        private bool PatientIdentifierConflictsFound(Patient other)
        {
            foreach (PatientProfile x in this.Profiles)
                foreach (PatientProfile y in other.Profiles)
                    if (x.Mrn.AssigningAuthority.Equals(y.Mrn.AssigningAuthority))
                        return true;

            return false;
        }

	}
}