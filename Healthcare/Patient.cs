#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
        public virtual void AddProfile(PatientProfile profile)
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
