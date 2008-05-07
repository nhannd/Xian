#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Search
{
    /// <summary>
    /// Model used in study list grid control <see cref="Study"/>.
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class StudySummary
    {
        #region Private members
        private ServerEntityKey _ref;
        private string _patientID;
        private string _patientName;
        private string _studyDate;
        private string _accessionNumber;
        private string _studyDescription;
        private int _numberOfRelatedSeries;
        private int _numberOfRelatedInstances;

        #endregion Private members

        
        #region Public Properties

        public ServerEntityKey GUID
        {
            get { return _ref; }
            set { _ref = value; }
        }

        public string PatientID
        {
            get { return _patientID; }
            set { _patientID = value; }
        }

        public string PatientsName
        {
            get { return _patientName; }
            set { _patientName = value; }
        }

        public string StudyDate
        {
            get { return _studyDate; }
            set { _studyDate = value; }
        }

        public string AccessionNumber
        {
            get { return _accessionNumber; }
            set { _accessionNumber = value; }
        }

        public string StudyDescription
        {
            get { return _studyDescription; }
            set { _studyDescription = value; }
        }

        public int NumberOfRelatedSeries
        {
            get { return _numberOfRelatedSeries; }
            set { _numberOfRelatedSeries = value; }
        }

        public int NumberOfRelatedInstances
        {
            get { return _numberOfRelatedInstances; }
            set { _numberOfRelatedInstances = value; }
        }

        #endregion Public Properties
    }
}
