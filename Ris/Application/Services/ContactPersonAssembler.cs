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
            
            ContactPersonRelationshipEnum relationship = context.GetBroker<IContactPersonRelationshipEnumBroker>().Load()[cp.Relationship];
            detail.Relationship = new EnumValueInfo(
                relationship.Code,
                relationship.Value);

            ContactPersonTypeEnum type = context.GetBroker<IContactPersonTypeEnumBroker>().Load()[cp.Type];
            detail.Type = new EnumValueInfo(
                type.Code,
                type.Value);

            return detail;
        }

        public ContactPerson CreateContactPerson(ContactPersonDetail detail)
        {
            ContactPerson cp = new ContactPerson();

            cp.Name = detail.Name;
            cp.Address = detail.Address;
            cp.HomePhone = detail.HomePhoneNumber;
            cp.BusinessPhone = detail.BusinessPhoneNumber;
            cp.Relationship = (ContactPersonRelationship)Enum.Parse(typeof(ContactPersonRelationship), detail.Relationship.Code);
            cp.Type = (ContactPersonType)Enum.Parse(typeof(ContactPersonType), detail.Type.Code);

            return cp;
        }
    }
}
