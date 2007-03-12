using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Core;

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

            // TODO Enum fields
            //telephoneDetail.Use = telephoneNumber.Use;
            //telephoneDetail.Equipment = telephoneNumber.Equipment;

            telephoneDetail.ValidRangeFrom = telephoneNumber.ValidRange.From;
            telephoneDetail.ValidRangeUntil = telephoneNumber.ValidRange.Until;

            return telephoneDetail;
        }

        public void AddTelephoneNumber(TelephoneNumber telephoneNumber, List<TelephoneDetail> phoneNumbers)
        {
            TelephoneDetail telephoneDetail = CreateTelephoneDetail(telephoneNumber);

            //TODO:  for each new telephone number, automatically expired any previous numbers based on the telephone use enum value

            phoneNumbers.Add(telephoneDetail);
        }
    }
}
