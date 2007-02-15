using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom
{
    // this class assumes that the ComponentGroup has already been decoded 
    // from any native character set into a Unicode string
    public class ComponentGroup
    {
        public ComponentGroup(string rawComponentGroupString)
        {
            _rawString = rawComponentGroupString;
            BreakApartIntoComponents();
        }

        public static ComponentGroup GetEmptyComponentGroup()
        {
            return _emptyComponentGroup;
        }

        public override string ToString()
        {
            return _rawString;
        }

        public static implicit operator String(ComponentGroup componentGroup)
        {
            return componentGroup.ToString();
        }

        protected void BreakApartIntoComponents()
        {
            string[] components = _rawString.Split('^');

            if (components.GetUpperBound(0) >= 0 && components[0] != string.Empty)
            {
                _familyName = components[0];
            }

            if (components.GetUpperBound(0) > 0 && components[1] != string.Empty)
            {
                _givenName = components[1];
            }

            if (components.GetUpperBound(0) > 1 && components[2] != string.Empty)
            {
                _middleName = components[2];
            }

            if (components.GetUpperBound(0) > 2 && components[3] != string.Empty)
            {
                _prefix = components[3];
            }

            if (components.GetUpperBound(0) > 3 && components[4] != string.Empty)
            {
                _suffix = components[4];
            }
        }

        #region Private fields
        private static ComponentGroup _emptyComponentGroup = new ComponentGroup("");
        #endregion
        #region Properties
        private string _rawString;
        private string _familyName;
        private string _givenName;
        private string _middleName;
        private string _prefix;
        private string _suffix;
	
        public string Suffix
        {
            get { return _suffix; }
            set { _suffix = value; }
        }

        public string Prefix
        {
            get { return _prefix; }
            set { _prefix = value; }
        }

        public string MiddleName
        {
            get { return _middleName; }
            set { _middleName = value; }
        }

        public string GivenName
        {
            get { return _givenName; }
            set { _givenName = value; }
        }

        public string FamilyName
        {
            get { return _familyName; }
            set { _familyName = value; }
        }


        public string RawString
        {
            get { return _rawString; }
            set { _rawString = value; }
        }
        #endregion
    }
}
