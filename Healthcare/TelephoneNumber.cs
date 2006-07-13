using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Healthcare
{
    public class TelephoneNumber
    {
        private string _countryCode;
        private string _areaCode;
        private string _number;
        private string _extension;
        private TelephoneUse _use;
        private TelephoneEquipment _equipment;

        public TelephoneUse Use
        {
            get { return _use; }
            set { _use = value; }
        }

        public TelephoneEquipment Equipment
        {
            get { return _equipment; }
            set { _equipment = value; }
        }

        public string CountryCode
        {
            get { return _countryCode; }
            set { _countryCode = value; }
        }

        public string AreaCode
        {
            get { return _areaCode; }
            set { _areaCode = value; }
        }

        public string Number
        {
            get { return _number; }
            set { _number = value; }
        }

        public string Extension
        {
            get { return _extension; }
            set { _extension = value; }
        }
    }
}
