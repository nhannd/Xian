using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Ris.Application.Services.Admin
{
    public class TelephoneNumberAssembler
    {
        public TelephoneDetail CreateTelephoneDetail(TelephoneNumber telephoneNumber, IPersistenceContext context)
        {
            if (telephoneNumber == null)
                return new TelephoneDetail();

            TelephoneDetail telephoneDetail = new TelephoneDetail();

            telephoneDetail.CountryCode = telephoneNumber.CountryCode;
            telephoneDetail.AreaCode = telephoneNumber.AreaCode;
            telephoneDetail.Number = telephoneNumber.Number;
            telephoneDetail.Extension = telephoneNumber.Extension;

            SimplifiedPhoneTypeAssembler simplePhoneTypeAssembler = new SimplifiedPhoneTypeAssembler();
            telephoneDetail.Type = simplePhoneTypeAssembler.GetSimplifiedPhoneType(telephoneNumber);

            telephoneDetail.ValidRangeFrom = telephoneNumber.ValidRange.From;
            telephoneDetail.ValidRangeUntil = telephoneNumber.ValidRange.Until;

            return telephoneDetail;
        }

        public TelephoneNumber CreateTelephoneNumber(TelephoneDetail telephoneDetail)
        {
            if (telephoneDetail == null)
                return null;

            TelephoneNumber telephoneNumber = new TelephoneNumber();

            telephoneNumber.CountryCode = telephoneDetail.CountryCode;
            telephoneNumber.AreaCode = telephoneDetail.AreaCode;
            telephoneNumber.Number = telephoneDetail.Number;
            telephoneNumber.Extension = telephoneDetail.Extension;
            telephoneNumber.ValidRange.From = telephoneDetail.ValidRangeFrom;
            telephoneNumber.ValidRange.Until = telephoneDetail.ValidRangeUntil;

            SimplifiedPhoneTypeAssembler simplePhoneTypeAssembler = new SimplifiedPhoneTypeAssembler();
            simplePhoneTypeAssembler.UpdatePhoneNumber(telephoneDetail.Type, telephoneNumber);

            return telephoneNumber;
        }

        public void AddTelephoneNumber(TelephoneDetail telephoneDetail, IList phoneNumbers)
        {
            //TODO: Check automatic expiration of Telephone functionality
            TelephoneNumber newNumber = CreateTelephoneNumber(telephoneDetail);
            
            foreach (TelephoneNumber phone in phoneNumbers)
            {
                if (newNumber.Equipment.Equals(phone.Equipment) &&
                    newNumber.Use.Equals(phone.Use))
                {
                    // Automatically expired any previous phone numbers
                    phone.ValidRange.Until = Platform.Time.Date;
                }
            }

            phoneNumbers.Add(newNumber);
        }
    }
}
