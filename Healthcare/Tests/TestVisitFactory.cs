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
    public static class TestVisitFactory
    {
		public static Visit CreateVisit(Patient patient)
        {
        	return CreateVisit(patient, "10001111");
        }
		public static Visit CreateVisit(Patient patient, string visitNumber)
        {
            Visit visit = new Visit(
                patient,
				new VisitNumber(visitNumber, new InformationAuthorityEnum("UHN", "UHN", "")),
                VisitStatus.AA,
                DateTime.Now - TimeSpan.FromDays(2),
                null,
                null,
                new PatientClassEnum("I", "Inpatient", null),
                new PatientTypeEnum("X", "Whatever", null),
                new AdmissionTypeEnum("A", "Who cares", null),
                TestFacilityFactory.CreateFacility(),
				TestLocationFactory.CreateLocation(),
                null,
                null,
                false,
                null,
                null,
				null);

            return visit;
        }
    }
}
