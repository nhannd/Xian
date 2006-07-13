using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Healthcare
{
    /// <summary>
    /// Implements a simplified version of the HL7 XPN (Extended Person Name) data type
    /// </summary>
    public class PersonName
    {
        private string _familyName;
        private string _givenName;
        private string _middleName;
        private string _prefix;
        private string _suffix;
        private string _degree;

        public string FamilyName
        {
            get { return _familyName; }
            set { _familyName = value; }
        }

        public string GivenName
        {
            get { return _givenName; }
            set { _givenName = value; }
        }

        public string MiddleName
        {
            get { return _middleName; }
            set { _middleName = value; }
        }

        public string Prefix
        {
            get { return _prefix; }
            set { _prefix = value; }
        }

        public string Suffix
        {
            get { return _suffix; }
            set { _suffix = value; }
        }

        public string Degree
        {
            get { return _degree; }
            set { _degree = value; }
        }

        /// <summary>
        /// Returns a string formatting the person name for purpose of display.
        /// NB. should probably create an overload that takes a format string for more
        /// flexibility
        /// </summary>
        /// <returns></returns>
        public string Format()
        {
            return string.Format("{0}, {1} {2}", FamilyName, GivenName, MiddleName).Trim();
        }
    }
}
