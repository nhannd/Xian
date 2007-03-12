using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Ris.Application.Services.Admin
{
    public class AddressAssembler
    {
        public ClearCanvas.Ris.Application.Common.Admin.Address CreateAddress(ClearCanvas.Healthcare.Address domainAddress)
        {
            ClearCanvas.Ris.Application.Common.Admin.Address dtoAddress = new ClearCanvas.Ris.Application.Common.Admin.Address();

            dtoAddress.Street = domainAddress.Street;
            dtoAddress.Unit = domainAddress.Unit;
            dtoAddress.City = domainAddress.City;
            dtoAddress.Province = domainAddress.Province;
            dtoAddress.PostalCode = domainAddress.PostalCode;
            dtoAddress.Country = domainAddress.Country;

            //TODO Enum field
            //dtoAddress.Type = domainAddress.Type;

            dtoAddress.ValidRangeFrom = domainAddress.ValidRange.From;
            dtoAddress.ValidRangeUntil = domainAddress.ValidRange.Until;

            return dtoAddress;
        }

        public void AddTelephoneNumber(ClearCanvas.Healthcare.Address domainAddress
            , List<ClearCanvas.Ris.Application.Common.Admin.Address> addresses)
        {
            ClearCanvas.Ris.Application.Common.Admin.Address dtoAddress = CreateAddress(domainAddress);

            //TODO:  for each new address, automatically expired any previous address based on the AddressType enum value

            addresses.Add(dtoAddress);
        }
    }
}
