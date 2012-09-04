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
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.ImageViewer.StudyManagement.Core
{
    public interface IStudy : IStudyRootData
    {
        IEnumerable<ISeries> GetSeries();
        IEnumerable<ISopInstance> GetSopInstances();
        DateTime? GetStoreTime();

        string SpecificCharacterSet { get; }

        new PersonName PatientsName { get; }
        new PersonName ReferringPhysiciansName { get; }

        string ProcedureCodeSequenceCodeValue { get; }
        string ProcedureCodeSequenceCodingSchemeDesignator { get; }

        new int NumberOfStudyRelatedSeries { get; }
        new int NumberOfStudyRelatedInstances { get; }
    }
}