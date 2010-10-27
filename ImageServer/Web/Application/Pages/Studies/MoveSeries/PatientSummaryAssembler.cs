#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.MoveSeries
{
    static public class PatientSummaryAssembler
    {
        /// <summary>
        /// Returns an instance of <see cref="PatientSummary"/> for a <see cref="Study"/>.
        /// </summary>
        /// <param name="study"></param>
        /// <returns></returns>
        /// <remark>
        /// 
        /// </remark>
        static public PatientSummary CreatePatientSummary(Study study)
        {
            PatientSummary patient = new PatientSummary();

            patient.PatientId = study.PatientId;
            patient.Birthdate = study.PatientsBirthDate;
            patient.PatientName = study.PatientsName;
            patient.Sex = study.PatientsSex;

            PatientAdaptor adaptor = new PatientAdaptor();
            Patient pat = adaptor.Get(study.PatientKey);
            patient.IssuerOfPatientId = pat.IssuerOfPatientId;
            return patient;
        }
    }
}
