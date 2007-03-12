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
        public AddressDetail CreateAddressDetail(Address address)
        {
            AddressDetail addressDetail = new AddressDetail();

            addressDetail.Street = address.Street;
            addressDetail.Unit = address.Unit;
            addressDetail.City = address.City;
            addressDetail.Province = address.Province;
            addressDetail.PostalCode = address.PostalCode;
            addressDetail.Country = address.Country;

            //TODO Enum field
            //addressDetail.Type = address.Type;

            addressDetail.ValidRangeFrom = address.ValidRange.From;
            addressDetail.ValidRangeUntil = address.ValidRange.Until;

            return addressDetail;
        }

        public void AddTelephoneNumber(Address address, List<AddressDetail> addresses)
        {
            AddressDetail addressDetail = CreateAddressDetail(address);

            //TODO:  for each new address, automatically expired any previous address based on the AddressType enum value

            addresses.Add(addressDetail);
        }
    }
}
