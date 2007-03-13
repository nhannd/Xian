using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common.Admin;

namespace ClearCanvas.Ris.Application.Services.Admin
{
    public class TelephoneNumberAssembler
    {
        public TelephoneDetail CreateTelephoneDetail(TelephoneNumber telephoneNumber)
        {
            TelephoneDetail telephoneDetail = new TelephoneDetail();

            telephoneDetail.CountryCode = telephoneNumber.CountryCode;
            telephoneDetail.AreaCode = telephoneNumber.AreaCode;
            telephoneDetail.Number = telephoneNumber.Number;
            telephoneDetail.Extension = telephoneNumber.Extension;

            // TODO Check Enum conversion
            telephoneDetail.Use = telephoneNumber.Use.ToString();
            telephoneDetail.Equipment = telephoneNumber.Equipment;

            telephoneDetail.ValidRangeFrom = telephoneNumber.ValidRange.From;
            telephoneDetail.ValidRangeUntil = telephoneNumber.ValidRange.Until;

            return telephoneDetail;
        }

        public TelephoneNumber CreateTelephoneNumber(TelephoneDetail telephoneDetail)
        {
            TelephoneNumber telephoneNumber = new TelephoneNumber();

            telephoneNumber.CountryCode = telephoneDetail.CountryCode;
            telephoneNumber.AreaCode = telephoneDetail.AreaCode;
            telephoneNumber.Number = telephoneDetail.Number;
            telephoneNumber.Extension = telephoneDetail.Extension;

            // TODO Check Enum conversion
            telephoneNumber.Use = (TelephoneUse)Enum.Parse(typeof(TelephoneUse), telephoneDetail.Use);
            telephoneNumber.Equipment = (TelephoneEquipment)Enum.Parse(typeof(TelephoneEquipment), telephoneDetail.Equipment);

            telephoneNumber.ValidRange.From = telephoneDetail.ValidRangeFrom;
            telephoneNumber.ValidRange.Until = elephoneDetail.ValidRangeUntil;

            return telephoneNumber;
        }

        public void AddTelephoneNumber(TelephoneDetail phoneDetail, List<TelephoneNumber> phoneNumbers)
        {
            //TODO: Check automatic expiration of Telephone functionality
            TelephoneNumber newNumber = CreateTelephoneNumber(phoneDetail);
            
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
