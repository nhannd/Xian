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
                if (_componentGroups.Length > 0)
                    return _componentGroups[0];
                else
                    return ComponentGroup.GetEmptyComponentGroup();
            }
        }

        public virtual ComponentGroup Ideographic
        {
            get
            {
                if (_componentGroups.Length > 1)
                    return _componentGroups[1];
                else
                    return ComponentGroup.GetEmptyComponentGroup();
            }
        }

        public virtual ComponentGroup Phonetic
        {
            get
            {
                if (_componentGroups.Length > 2)
                    return _componentGroups[2];
                else
                    return ComponentGroup.GetEmptyComponentGroup();
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
            string decodedRawData = SpecificCharacterSetParser.Parse(_specificCharacterSet, this.InternalPersonName);
            string[] componentGroupsStrings = decodedRawData.Split('=');

            if (componentGroupsStrings.GetUpperBound(0) >= 0 && componentGroupsStrings[0] != string.Empty)
                _componentGroups[0] = new ComponentGroup(componentGroupsStrings[0]);
            else
                _componentGroups[0] = ComponentGroup.GetEmptyComponentGroup();

            if (componentGroupsStrings.GetUpperBound(0) > 0 && componentGroupsStrings[1] != string.Empty)
                _componentGroups[1] = new ComponentGroup(componentGroupsStrings[1]);
            else
                _componentGroups[1] = ComponentGroup.GetEmptyComponentGroup();

            if (componentGroupsStrings.GetUpperBound(0) > 1 && componentGroupsStrings[2] != string.Empty)
                _componentGroups[2] = new ComponentGroup(componentGroupsStrings[2]);
            else
                _componentGroups[2] = ComponentGroup.GetEmptyComponentGroup();
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
        private ComponentGroup[] _componentGroups = new ComponentGroup[3];
		#endregion
	}
}
