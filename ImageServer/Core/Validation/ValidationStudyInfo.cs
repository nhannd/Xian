#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Text;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Core.Validation
{
    /// <summary>
    /// Information associated with a validation failure.
    /// </summary>
    public class ValidationStudyInfo
    {
        #region Public Properties

    	public string ServerAE { get; set; }

    	public string PatientsName { get; set; }

    	public string PatientsId { get; set; }

    	public string StudyInstaneUid { get; set; }

    	public string AccessionNumber { get; set; }

    	public string StudyDate { get; set; }

    	#endregion

        public ValidationStudyInfo(){}

        public ValidationStudyInfo(Study theStudy, ServerPartition partition)
        {
            ServerAE = partition.AeTitle;
            PatientsName = theStudy.PatientsName;
            PatientsId = theStudy.PatientId;
            StudyInstaneUid = theStudy.StudyInstanceUid;
            AccessionNumber = theStudy.AccessionNumber;
            StudyDate = theStudy.StudyDate;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(String.Format("Partition : {0}", ServerAE));
            sb.AppendLine(String.Format("Patient   : {0}", PatientsName));
            sb.AppendLine(String.Format("Patient ID: {0}", PatientsId));
            sb.AppendLine(String.Format("Study UID : {0}", StudyInstaneUid));
            sb.AppendLine(String.Format("Accession#: {0}", AccessionNumber));
            sb.AppendLine(String.Format("Study Date: {0}", StudyDate));

            return sb.ToString();
        }
    }
}