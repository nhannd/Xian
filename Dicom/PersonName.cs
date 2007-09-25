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
