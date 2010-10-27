#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
    public class ContactPersonAssembler
    {
        public ContactPersonDetail CreateContactPersonDetail(ContactPerson cp)
        {
            ContactPersonDetail detail = new ContactPersonDetail();

            detail.Name = cp.Name;
            detail.Address = cp.Address;
            detail.HomePhoneNumber = cp.HomePhone;
            detail.BusinessPhoneNumber = cp.BusinessPhone;
            detail.Relationship = EnumUtils.GetEnumValueInfo(cp.Relationship);
            detail.Type = EnumUtils.GetEnumValueInfo(cp.Type);

            return detail;
        }

        public ContactPerson CreateContactPerson(ContactPersonDetail detail, IPersistenceContext context)
        {
            ContactPerson cp = new ContactPerson();

            cp.Name = detail.Name;
            cp.Address = detail.Address;
            cp.HomePhone = detail.HomePhoneNumber;
            cp.BusinessPhone = detail.BusinessPhoneNumber;
            cp.Relationship = EnumUtils.GetEnumValue<ContactPersonRelationshipEnum>(detail.Relationship, context);
            cp.Type = EnumUtils.GetEnumValue<ContactPersonTypeEnum>(detail.Type, context);

            return cp;
        }
    }
}
