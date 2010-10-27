#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using Iesi.Collections;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
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

        /// <summary>
        /// Adds a <see cref="PatientProfile"/> to this patient.
        /// </summary>
        /// <remarks>
        /// Use this method rather than adding to the <see cref="Profiles"/>
        /// collection directly.
        /// </remarks>
        /// <param name="profile"></param>
        public virtual void AddProfile(PatientProfile profile)
        {
            if (profile.Patient != null)
            {
                //NB: technically we should remove the profile from the other patient's collection, but there
                //seems to be a bug with NHibernate where it deletes the profile if we do this
                //profile.Patient.Profiles.Remove(profile);
            }
            profile.Patient = this;
            _profiles.Add(profile);
        }

		/// <summary>
		/// Adds a <see cref="PatientNote"/> to this patient.
		/// </summary>
		/// <remarks>
		/// Use this method rather than adding to the <see cref="Patient.Notes"/>
		/// collection directly.
		/// </remarks>
		/// <param name="note"></param>
		public virtual void AddNote(PatientNote note)
		{
			if (note.Patient != null)
			{
				//NB: technically we should remove the profile from the other patient's collection, but there
				//seems to be a bug with NHibernate where it deletes the profile if we do this
				//profile.Patient.Profiles.Remove(profile);
			}
			note.Patient = this;
			_notes.Add(note);
		}

		/// <summary>
		/// Allows getting at a particular PatientProfile
		/// </summary>
		/// <param name="patientIdentifier"></param>
		/// <param name="authority"></param>
		public virtual PatientProfile GetProfile(string patientIdentifier, string authority)
		{
			Platform.CheckForEmptyString(patientIdentifier, "mrn");
			Platform.CheckForEmptyString(authority, "assigningAuthority");

			foreach (PatientProfile profile in this.Profiles)
			{
				if (profile.Mrn.Id == patientIdentifier && profile.Mrn.AssigningAuthority.Value == authority)
					return profile;
			}

			return null;
		}

		/// <summary>
		/// Gets the profile corresponding to the specified facility, or null if no such profile exists.
		/// </summary>
		/// <param name="facility"></param>
		/// <returns></returns>
		public virtual PatientProfile GetProfile(Facility facility)
		{
			return CollectionUtils.SelectFirst(_profiles,
				profile => Equals(profile.Mrn.AssigningAuthority, facility.InformationAuthority));
		}
	}
}
