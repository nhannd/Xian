using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Ris.Application.Services
{
    public class PersonNameAssembler
    {
        public PersonNameDetail CreatePersonNameDetail(PersonName personName)
        {
            PersonNameDetail detail = new PersonNameDetail();
            detail.FamilyName = personName.FamilyName;
            detail.GivenName = personName.GivenName;
            detail.MiddleName = personName.MiddleName;
            detail.Prefix = personName.Prefix;
            detail.Suffix = personName.Suffix;
            detail.Degree = personName.Degree;
            return detail;
        }
    }
}
