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
	}
}
