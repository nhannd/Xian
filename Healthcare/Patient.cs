using System;
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// Patient entity
    /// </summary>
	public partial class Patient : Entity
	{
		/// <summary>
		/// Factory method
		/// </summary>
		public static Patient New()
		{
			// add any object initialization code here
			// the signature of the New() method may be freely changed as needed
			return new Patient();
		}

        /// <summary>
        /// Adds a profile to this patient, setting the profile's <see cref="PatientProfile.Patient"/> property
        /// to refer to this object.  Use this method rather than referring to the <see cref="Patient.Profiles"/>
        /// collection directly.
        /// </summary>
        /// <param name="profile"></param>
        public void AddProfile(PatientProfile profile)
        {
            Profiles.Add(profile);
            profile.Patient = this;
        }

        /// <summary>
        /// Overridden to compare OIDs. According to NHibernate docs this is a very bad thing to do,
        /// because the OID will change if the object goes from a transient to a persistent state.
        /// However, given that the Patient object has no domain fields, we have no other choice.
        /// Also, because the application does not use transient Patient objects, we should be able
        /// to get away with it.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            // according to NHibernate docs this is a very bad thing to do,
            // because the OID will change if the object goes from a transient to a persistent state
            // however, because the application does not use transient Patient objects, we should be ok
            Patient that = obj as Patient;
            return that != null && this.OID == that.OID;
        }

        /// <summary>
        /// Overridden to return hash code of OID. According to NHibernate docs this is a very bad thing to do,
        /// because the OID will change if the object goes from a transient to a persistent state.
        /// However, given that the Patient object has no domain fields, we have no other choice.
        /// Also, because the application does not use transient Patient objects, we should be able
        /// to get away with it.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            // according to NHibernate docs this is a very bad thing to do,
            // because the OID will change if the object goes from a transient to a persistent state
            // however, because the application does not use transient Patient objects, we should be ok
            return this.OID.GetHashCode();
        }
	}
}