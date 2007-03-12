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
        public ClearCanvas.Ris.Application.Common.Admin.TelephoneNumber CreateTelephoneNumber(ClearCanvas.Healthcare.TelephoneNumber domainTelephoneNumber)
        {
            ClearCanvas.Ris.Application.Common.Admin.TelephoneNumber dtoTelephoneNumber = new ClearCanvas.Ris.Application.Common.Admin.TelephoneNumber();

            dtoTelephoneNumber.CountryCode = domainTelephoneNumber.CountryCode;
            dtoTelephoneNumber.AreaCode = domainTelephoneNumber.AreaCode;
            dtoTelephoneNumber.Number = domainTelephoneNumber.Number;
            dtoTelephoneNumber.Extension = domainTelephoneNumber.Extension;

            // TODO Enum fields
            //dtoTelephoneNumber.Use = domainTelephoneNumber.Use;
            //dtoTelephoneNumber.Equipment = domainTelephoneNumber.Equipment;

            dtoTelephoneNumber.ValidRangeFrom = domainTelephoneNumber.ValidRange.From;
            dtoTelephoneNumber.ValidRangeUntil = domainTelephoneNumber.ValidRange.Until;

            return dtoTelephoneNumber;
        }

        public void AddTelephoneNumber(ClearCanvas.Healthcare.TelephoneNumber domainTelephoneNumber
            , List<ClearCanvas.Ris.Application.Common.Admin.TelephoneNumber> phoneNumbers)
        {
            ClearCanvas.Ris.Application.Common.Admin.TelephoneNumber dtoTelephoneNumber = CreateAddress(domainTelephoneNumber);

            //TODO:  for each new telephone number, automatically expired any previous numbers based on the telephone use enum value

            phoneNumbers.Add(dtoTelephoneNumber);
        }
    }
}
