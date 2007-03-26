using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin;

namespace ClearCanvas.Ris.Application.Services
{
    public class PersonNameAssembler
    {
        public PersonNameDetail CreatePersonNameDetail(PersonName personName)
        {
            if (personName == null)
                return new PersonNameDetail();

            PersonNameDetail detail = new PersonNameDetail();
            detail.FamilyName = personName.FamilyName;
            detail.GivenName = personName.GivenName;
            detail.MiddleName = personName.MiddleName;
            detail.Prefix = personName.Prefix;
            detail.Suffix = personName.Suffix;
            detail.Degree = personName.Degree;
            return detail;
        }

        public void UpdatePersonName(PersonNameDetail detail, PersonName personName)
        {
            personName.FamilyName = detail.FamilyName;
            personName.GivenName = detail.GivenName;
            personName.MiddleName = detail.MiddleName;
            personName.Prefix = detail.Prefix;
            personName.Suffix = detail.Suffix;
            personName.Degree = detail.Degree;
        }    
    }
}
