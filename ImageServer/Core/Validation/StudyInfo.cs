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

namespace ClearCanvas.ImageServer.Core.Validation
{
    /// <summary>
    /// Information associated with a validation failure.
    /// </summary>
    public class StudyInfo
    {
        #region Private Members
        private string _serverAE;
        private string _patientsName;
        private string _patientsId;
        private string _studyInstaneUid;
        private string _accessionNumber;
        private string _studyDate;
        #endregion
        #region Public Properties

        public string ServerAE
        {
            get { return _serverAE; }
            set { _serverAE = value; }
        }

        public string PatientsName
        {
            get { return _patientsName; }
            set { _patientsName = value; }
        }

        public string PatientsId
        {
            get { return _patientsId; }
            set { _patientsId = value; }
        }

        public string StudyInstaneUid
        {
            get { return _studyInstaneUid; }
            set { _studyInstaneUid = value; }
        }

        public string AccessionNumber
        {
            get { return _accessionNumber; }
            set { _accessionNumber = value; }
        }

        public string StudyDate
        {
            get { return _studyDate; }
            set { _studyDate = value; }
        }

        #endregion

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(String.Format("Partition : {0}", _serverAE));
            sb.AppendLine(String.Format("Patient   : {0}", _patientsName));
            sb.AppendLine(String.Format("Patient ID: {0}", _patientsId));
            sb.AppendLine(String.Format("Study UID : {0}", _studyInstaneUid));
            sb.AppendLine(String.Format("Accession#: {0}", _accessionNumber));
            sb.AppendLine(String.Format("Study Date: {0}", _studyDate));

            return sb.ToString();
        }
    }
}