using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom
{
    /// <summary>
    /// Encapsulates the DICOM Person's Name.
    /// </summary>
    public class PersonName
    {
        /// <summary>
        /// Constructor for NHibernate.
        /// </summary>
        public PersonName()
        {
        }

        /// <summary>
        /// Mandatory constructor.
        /// </summary>
        /// <param name="personsName">The Person's Name as a string.</param>
        public PersonName(string personsName)
        {
            // validate the input
            if (null == personsName)
                throw new System.ArgumentNullException("personsName", SR.ExceptionGeneralPersonsNameNull);

            _personsName = personsName;
        }

        protected virtual string InternalPersonName
        {
            get { return _personsName; }
            set
            {
                _personsName = value;
            }
        }

        public virtual String LastName
        {
            get { return this.SingleByte.FamilyName; }
        }

        public virtual String FirstName
        {
            get { return this.SingleByte.GivenName; }
        }

        public virtual ComponentGroup SingleByte
        {
            get 
            {
                return _componentGroups[0];
            }
        }

        public virtual ComponentGroup Ideographic
        {
            get
            {
                return _componentGroups[1];
            }
        }

        public virtual ComponentGroup Phonetic
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

        /// <summary>
        /// Implicit cast to a String object, for ease of use.
        /// </summary>
        public static implicit operator String(PersonName pn)
        {
            return pn.ToString();
        }

        protected void BreakApartIntoComponentGroups()
        {
            // if there's no name, don't do anything
            if (null == this.InternalPersonName || "" == this.InternalPersonName)
                return;

            string decodedRawData = SpecificCharacterSetParser.Parse(_specificCharacterSet, this.InternalPersonName);
            string[] componentGroupsStrings = decodedRawData.Split('=');

            if (componentGroupsStrings.GetUpperBound(0) >= 0 && componentGroupsStrings[0] != string.Empty)
                _componentGroups[0] = new ComponentGroup(componentGroupsStrings[0]);

            if (componentGroupsStrings.GetUpperBound(0) > 0 && componentGroupsStrings[1] != string.Empty)
                _componentGroups[1] = new ComponentGroup(componentGroupsStrings[1]);

            if (componentGroupsStrings.GetUpperBound(0) > 1 && componentGroupsStrings[2] != string.Empty)
                _componentGroups[2] = new ComponentGroup(componentGroupsStrings[2]);
        }

        #region Properties
        private string _specificCharacterSet = "ISO 2022 IR 6";     // this is the default

        public string SpecificCharacterSet
        {
            get { return _specificCharacterSet; }
            set 
            { 
                _specificCharacterSet = value;
                BreakApartIntoComponentGroups();
            }
        }
	
        #endregion

        #region Private fields
        private string _personsName;
        private string _lastName;
        private string _firstName;
        private ComponentGroup[] _componentGroups = { ComponentGroup.GetEmptyComponentGroup(), 
                                                        ComponentGroup.GetEmptyComponentGroup(),
                                                        ComponentGroup.GetEmptyComponentGroup() };
		#endregion
	}
}
