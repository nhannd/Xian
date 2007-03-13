using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common.Admin;

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

            //TODO Check Enum conversion
            addressDetail.Type = address.Type.ToString();
            addressDetail.ValidRangeFrom = address.ValidRange.From;
            addressDetail.ValidRangeUntil = address.ValidRange.Until;

            return addressDetail;
        }

        public Address CreateAddress(AddressDetail addressDetail)
        {
            Address newAddress = new Address();

            newAddress.Street = addressDetail.Street;
            newAddress.Unit = addressDetail.Unit;
            newAddress.City = addressDetail.City;
            newAddress.Province = addressDetail.Province;
            newAddress.PostalCode = addressDetail.PostalCode;
            newAddress.Country = addressDetail.Country;

            //TODO Check Enum conversion
            newAddress.Type = (AddressType)Enum.Parse(typeof(AddressType), addressDetail.Type);
            newAddress.ValidRange.From = addressDetail.ValidRangeFrom;
            newAddress.ValidRange.Until = addressDetail.ValidRangeUntil;

            return newAddress;
        }

        public void AddAddress(AddressDetail addressDetail, List<Address> addresses)
        {
            //TODO: Check automatic expiration of Address functionality
            Address newAddress = CreateAddress(addressDetail);

            foreach (Address address in addresses)
            {
                if (newAddress.Type.Equals(address.Type))
                {
                    // Automatically expired any previous address
                    address.ValidRange.Until = Platform.Time.Date;
                }
            }

            addresses.Add(newAddress);
        }
    }
}
