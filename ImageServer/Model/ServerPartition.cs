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
using System.Collections.Generic;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.ImageServer.Model
{
    public class StudyCompareOptions
    {
        private bool _matchIssuerOfPatientId;
        private bool _matchPatientId;
        private bool _matchPatientsName;
        private bool _matchPatientsBirthDate;
        private bool _matchPatientsSex;
        private bool _matchAccessionNumber;

        public bool MatchIssuerOfPatientId
        {
            get { return _matchIssuerOfPatientId; }
            set { _matchIssuerOfPatientId = value; }
        }

        public bool MatchPatientId
        {
            get { return _matchPatientId; }
            set { _matchPatientId = value; }
        }

        public bool MatchPatientsName
        {
            get { return _matchPatientsName; }
            set { _matchPatientsName = value; }
        }

        public bool MatchPatientsBirthDate
        {
            get { return _matchPatientsBirthDate; }
            set { _matchPatientsBirthDate = value; }
        }

        public bool MatchPatientsSex
        {
            get { return _matchPatientsSex; }
            set { _matchPatientsSex = value; }
        }

        public bool MatchAccessionNumber
        {
            get { return _matchAccessionNumber; }
            set { _matchAccessionNumber = value; }
        }
    }

    public partial class ServerPartition
    {
        public StudyCompareOptions GetComparisonOptions()
        {
            StudyCompareOptions options = new StudyCompareOptions();
            options.MatchAccessionNumber = MatchAccessionNumber;
            options.MatchIssuerOfPatientId = MatchIssuerOfPatientId;
            options.MatchPatientId = MatchPatientId;
            options.MatchPatientsBirthDate = MatchPatientsBirthDate;
            options.MatchPatientsName = MatchPatientsName;
            options.MatchPatientsSex = MatchPatientsSex;

            return options;
        }
    }
}
