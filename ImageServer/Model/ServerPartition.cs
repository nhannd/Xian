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
