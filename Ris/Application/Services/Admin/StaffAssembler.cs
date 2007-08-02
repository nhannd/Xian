using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Ris.Application.Services.Admin
{
    public class StaffAssembler
    {
        public StaffSummary CreateStaffSummary(Staff staff, IPersistenceContext context)
        {
            if (staff == null)
                return null;

            StaffSummary summary = new StaffSummary();
            summary.StaffRef = staff.GetRef();
            summary.PersonNameDetail = new PersonNameAssembler().CreatePersonNameDetail(staff.Name);
            summary.StaffType = EnumUtils.GetEnumValueInfo<StaffType>(staff.Type, context);
            summary.LicenseNumber = staff.Is<Practitioner>() ? staff.As<Practitioner>().LicenseNumber : "";
            return summary;
        }

        public StaffDetail CreateStaffDetail(Staff staff, IPersistenceContext context)
        {
            PersonNameAssembler assembler = new PersonNameAssembler();
            TelephoneNumberAssembler telephoneNumberAssembler = new TelephoneNumberAssembler();
            AddressAssembler addressAssembler = new AddressAssembler();

            StaffDetail detail = new StaffDetail();
            detail.StaffType = EnumUtils.GetEnumValueInfo<StaffType>(staff.Type, context);
            detail.PersonNameDetail = assembler.CreatePersonNameDetail(staff.Name);
            detail.TelephoneNumbers = CollectionUtils.Map<TelephoneNumber, TelephoneDetail, List<TelephoneDetail>>(
                    staff.TelephoneNumbers,
                    delegate(TelephoneNumber phone)
                    {
                        return telephoneNumberAssembler.CreateTelephoneDetail(phone, context);
                    });
            detail.Addresses = CollectionUtils.Map<Address, AddressDetail, List<AddressDetail>>(
                    staff.Addresses,
                    delegate(Address address)
                    {
                        return addressAssembler.CreateAddressDetail(address, context);
                    });
            detail.LicenseNumber = staff.Is<Practitioner>() ? staff.As<Practitioner>().LicenseNumber : "";

            return detail;
        }

        public void UpdateStaff(StaffDetail detail, Staff staff)
        {
            PersonNameAssembler assembler = new PersonNameAssembler();
            TelephoneNumberAssembler telephoneNumberAssembler = new TelephoneNumberAssembler();
            AddressAssembler addressAssembler = new AddressAssembler();

            assembler.UpdatePersonName(detail.PersonNameDetail, staff.Name);

            foreach (TelephoneDetail phoneDetail in detail.TelephoneNumbers)
            {
                telephoneNumberAssembler.AddTelephoneNumber(phoneDetail, staff.TelephoneNumbers);
            }

            foreach (AddressDetail addressDetail in detail.Addresses)
            {
                addressAssembler.AddAddress(addressDetail, staff.Addresses);
            }

            staff.Type = EnumUtils.GetEnumValue<StaffType>(detail.StaffType);
            if (staff.Is<Practitioner>())
            {
                // JR: this is the wrong place to be doing this validation (should be in model)
                // but it seems like the entire Staff-Practitioner model is wrong and needs refactoring
                if (staff.Type == StaffType.REF || staff.Type == StaffType.RAD || staff.Type == StaffType.RES)
                {
                    (staff.As<Practitioner>()).LicenseNumber = detail.LicenseNumber;
                }
                else
                {
                    throw new RequestValidationException("Staff type must be a physician type");
                }
            }
            else
            {
                // JR: this is the wrong place to be doing this validation (should be in model)
                // but it seems like the entire Staff-Practitioner model is wrong and needs refactoring
                if (staff.Type == StaffType.REF || staff.Type == StaffType.RAD || staff.Type == StaffType.RES)
                {
                    throw new RequestValidationException("Staff type must not be a physician type.");
                }
            }
        }
    }
}
