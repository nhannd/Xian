using System;
using System.Data;
using System.Configuration;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.StudyDetails
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
        static public PatientSummary CreatePatientSummary(Model.Study study)
        {
            PatientSummary patient = new PatientSummary();

            DateTime bdate;
            if (DateTime.TryParseExact(study.PatientsBirthDate, new string[] {"yyyy", "yyyyMM", "yyyyMMdd"}, null, DateTimeStyles.AssumeLocal,
                                   out bdate))
            {
                patient.Birthdate = bdate;
            }

            patient.PatientId = study.PatientId;

            patient.PatientName = study.PatientsName;
            patient.Sex = study.PatientsSex;

            PatientAdaptor adaptor = new PatientAdaptor();
            Model.Patient pat = adaptor.Get(study.PatientKey);
            patient.IssuerOfPatientId = pat.IssuerOfPatientId;
            return patient;
        }
    }
}
