using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Healthcare.Tests
{
    internal static class TestPatientFactory
    {
        internal static Patient CreatePatient()
        {
            Patient patient = new Patient();
            PatientProfile profile = new PatientProfile(
                new CompositeIdentifier("0000111", "UHN"),
                new HealthcardNumber("1111222333", "ON", null, null),
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
                patient
                );

            patient.AddProfile(profile);

            return patient;
        }
    }
}
