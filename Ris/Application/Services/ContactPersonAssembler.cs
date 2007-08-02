using System;
using System.Collections.Generic;

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Ris.Application.Services
{
    public class ContactPersonAssembler
    {
        public ContactPersonDetail CreateContactPersonDetail(ContactPerson cp, IPersistenceContext context)
        {
            ContactPersonDetail detail = new ContactPersonDetail();

            detail.Name = cp.Name;
            detail.Address = cp.Address;
            detail.HomePhoneNumber = cp.HomePhone;
            detail.BusinessPhoneNumber = cp.BusinessPhone;
            detail.Relationship = EnumUtils.GetEnumValueInfo(cp.Relationship, context);
            detail.Type = EnumUtils.GetEnumValueInfo(cp.Type, context);

            return detail;
        }

        public ContactPerson CreateContactPerson(ContactPersonDetail detail)
        {
            ContactPerson cp = new ContactPerson();

            cp.Name = detail.Name;
            cp.Address = detail.Address;
            cp.HomePhone = detail.HomePhoneNumber;
            cp.BusinessPhone = detail.BusinessPhoneNumber;
            cp.Relationship = EnumUtils.GetEnumValue<ContactPersonRelationship>(detail.Relationship);
            cp.Type = EnumUtils.GetEnumValue<ContactPersonType>(detail.Type);

            return cp;
        }
    }
}
