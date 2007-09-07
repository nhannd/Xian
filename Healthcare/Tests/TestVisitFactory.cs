using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Healthcare.Tests
{
    internal static class TestVisitFactory
    {
        internal static Visit CreateVisit(Patient patient)
        {
            Visit visit = new Visit(
                patient,
                new CompositeIdentifier("10001111", "UHN"),
                VisitStatus.AA,
                DateTime.Now - TimeSpan.FromDays(2),
                new PatientClassEnum("I", "Inpatient", null),
                new PatientTypeEnum("X", "Whatever", null),
                new AdmissionTypeEnum("A", "Who cares", null),
                TestFacilityFactory.CreateFacility(),
                null,
                null,
                null,
                null,
                false,
                null,
                null);

            return visit;
        }
    }
}
