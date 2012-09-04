#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Healthcare.Tests
{
	public static class TestPatientFactory
    {
		public static Patient CreatePatient()
        {
        	return CreatePatient("0000111");
        }

		public static Patient CreatePatient(string mrn)
        {
            Patient patient = new Patient();
            PatientProfile profile = new PatientProfile(
                new PatientIdentifier(mrn, new InformationAuthorityEnum("UHN", "UHN", "")),
                null,
                new HealthcardNumber("1111222333", new InsuranceAuthorityEnum("OHIP", "OHIP", ""), null, null),
                new PersonName("Roberts", "Bob", null, null, null, null),
                DateTime.Now - TimeSpan.FromDays(4000),
                Sex.M,
                new SpokenLanguageEnum("en", "English", null),
                new ReligionEnum("X", "unknown", null),
                false,
				null,
                null,
                null,
                null,
                null,
                null,
                null,
                patient
                );

            patient.AddProfile(profile);

            return patient;
        }
    }
}
