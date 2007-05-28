using System;
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise.Core;
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

        ///<summary>
        /// Overridden to compare OIDs. According to NHibernate docs this is a very bad thing to do,
        /// because the OID will change if the object goes from a transient to a persistent state.
        /// However, given that the Patient object has no domain fields, we have no other choice.
        /// Also, because the application does not use transient Patient objects, we should be able
        /// to get away with it.
        ///</summary>
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
	}
}
