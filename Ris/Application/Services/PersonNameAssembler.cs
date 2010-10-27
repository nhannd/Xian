#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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
            personName.FamilyName = TrimDetail(detail.FamilyName);
            personName.GivenName = TrimDetail(detail.GivenName);
            personName.MiddleName = TrimDetail(detail.MiddleName);
            personName.Prefix = TrimDetail(detail.Prefix);
            personName.Suffix = TrimDetail(detail.Suffix);
            personName.Degree = TrimDetail(detail.Degree);
        }

		private static string TrimDetail(string detail)
		{
			return string.IsNullOrEmpty(detail) ? detail : detail.Trim();
		}
    }
}
