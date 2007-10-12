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
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Dicom
{
    /// <summary>
    /// Encapsulates the DICOM Person's Name.
    /// </summary>
    public class PersonName : IEquatable<PersonName>
    {
		private string _personsName;
		private string _formattedName;

		private readonly ComponentGroup[] _componentGroups = { ComponentGroup.GetEmptyComponentGroup(), 
                                                        ComponentGroup.GetEmptyComponentGroup(),
                                                        ComponentGroup.GetEmptyComponentGroup() };
		/// <summary>
        /// Constructor for NHibernate.
        /// </summary>
        private PersonName()
        {
        }

        /// <summary>
        /// Mandatory constructor.
        /// </summary>
        /// <param name="personsName">The Person's Name as a string.</param>
        public PersonName(string personsName)
        {
			SetInternalPersonName(personsName);
        }

		/// <summary>
		/// NHibernate Property.
		/// </summary>
        protected virtual string InternalPersonName
        {
            get { return _personsName; }
            set { SetInternalPersonName(value); }
        }

        public String LastName
        {
            get { return this.SingleByte.FamilyName; }
        }

        public String FirstName
        {
            get { return this.SingleByte.GivenName; }
        }

		public String FormattedName
		{
			get { return _formattedName; }
		}

        public ComponentGroup SingleByte
        {
            get 
            {
                return _componentGroups[0];
            }
        }

        public ComponentGroup Ideographic
        {
            get
            {
                return _componentGroups[1];
            }
        }

        public ComponentGroup Phonetic
        {
            get
            {
                return _componentGroups[2];
            }
        }

		/// <summary>
		/// Gets the Person's Name as a string.
		/// </summary>
		/// <returns>A string representation of the Person's Name.</returns>
		public override string ToString()
		{
			return _personsName;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
				return true;

			if (obj is PersonName)
				return this.Equals((PersonName)obj);

			return false;
		}

		#region IEquatable<PersonName> Members

		public bool Equals(PersonName other)
		{
			return InternalPersonName == other.InternalPersonName;
		}

		#endregion
		
		/// <summary>
        /// Implicit cast to a String object, for ease of use.
        /// </summary>
        public static implicit operator String(PersonName pn)
        {
            return pn.ToString();
        }

		private void SetInternalPersonName(string personsName)
		{
			_personsName = personsName ?? "";
			BreakApartIntoComponentGroups();
			SetFormattedName();
		}

    	private void BreakApartIntoComponentGroups()
        {
            // if there's no name, don't do anything
            if (String.IsNullOrEmpty(this.InternalPersonName))
                return;

            string[] componentGroupsStrings = this.InternalPersonName.Split('=');

            if (componentGroupsStrings.GetUpperBound(0) >= 0 && componentGroupsStrings[0] != string.Empty)
                _componentGroups[0] = new ComponentGroup(componentGroupsStrings[0]);

            if (componentGroupsStrings.GetUpperBound(0) > 0 && componentGroupsStrings[1] != string.Empty)
                _componentGroups[1] = new ComponentGroup(componentGroupsStrings[1]);

            if (componentGroupsStrings.GetUpperBound(0) > 1 && componentGroupsStrings[2] != string.Empty)
                _componentGroups[2] = new ComponentGroup(componentGroupsStrings[2]);
		}
		
		private void SetFormattedName()
		{
			//by default, the formatted name is LastName, FirstName
			_formattedName = StringUtilities.Combine<string>(new string[] { this.LastName, this.FirstName }, ", ");
		}
	}
}
