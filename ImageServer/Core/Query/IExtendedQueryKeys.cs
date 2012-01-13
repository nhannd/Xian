#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Core.Query
{
    public interface IExtendedQueryKeys
    {
        void OnReceivePatientLevelQuery(DicomMessage message, DicomTag tag, PatientSelectCriteria criteria, StudySelectCriteria studyCriteria, out bool studySubSelect);
        void OnReceiveStudyLevelQuery(DicomMessage message, DicomTag tag, StudySelectCriteria criteria);
        void OnReceiveSeriesLevelQuery(DicomMessage message, DicomTag tag, SeriesSelectCriteria criteria);

        void PopulatePatient(DicomMessageBase response, DicomTag tag, Patient row, Study theStudy);
        void PopulateStudy(DicomMessageBase response, DicomTag tag, Study row);
        void PopulateSeries(DicomMessageBase response, DicomTag tag, Series row);
    }

    public class QueryKeysExtensionPoint : ExtensionPoint<IExtendedQueryKeys>
    {
    }
}
