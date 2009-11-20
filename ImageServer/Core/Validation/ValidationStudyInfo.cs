#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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