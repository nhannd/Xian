#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Dicom.ServiceModel.Query;

#if UNIT_TESTS

namespace ClearCanvas.ImageViewer.Common.StudyManagement.Tests
{
    public class StudyStoreTestServiceProvider : IServiceProvider
    {
        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            if (typeof(IStudyStoreQuery) == serviceType)
                return new TestStudyStoreQuery();
            if (typeof(IStorageConfiguration) == serviceType)
                return new TestStorageConfiguration();
            return null;
        }

        #endregion
    }

    public class TestStorageConfiguration : IStorageConfiguration
    {
        public static StorageConfiguration Configuration;

        static TestStorageConfiguration()
        {
            Configuration = new StorageConfiguration
                                {
                                    FileStoreDirectory = @"c:\filestore",
                                    MinimumFreeSpaceBytes = 5*1024L*1024L*1024L
                                };
        }

        #region IStorageConfiguration Members

        public GetStorageConfigurationResult GetConfiguration(GetStorageConfigurationRequest request)
        {
            return new GetStorageConfigurationResult
            {
                Configuration = Configuration
            };
        }

        public UpdateStorageConfigurationResult UpdateConfiguration(UpdateStorageConfigurationRequest request)
        {
            Configuration = request.Configuration;
            return new UpdateStorageConfigurationResult();
        }

        #endregion
    }
    public class TestStudyStoreQuery : IStudyStoreQuery
    {
        public static readonly List<StudyEntry> StudyEntries;

        static TestStudyStoreQuery()
        {
            StudyEntries = new List<StudyEntry>
                               {
                                   new StudyEntry
                                       {
                                           Study = new StudyRootStudyIdentifier
                                                       {
                                                           PatientId = "123",
                                                           StudyInstanceUid = "1.2.3",
                                                           PatientsName = "Test^Patient",
                                                           AccessionNumber = "a1",
                                                           ModalitiesInStudy = new string[] {"CR", "KO", "PR"}
                                                       },
                                           Data = new StudyEntryData {DeleteTime = null, StoreTime = null}
                                       },

                                   new StudyEntry
                                       {
                                           Study = new StudyRootStudyIdentifier
                                                       {
                                                           PatientId = "223",
                                                           StudyInstanceUid = "2.2.3",
                                                           PatientsName = "Test^Patient2",
                                                           AccessionNumber = "a2",
                                                           ModalitiesInStudy = new string[] {"CT", "DOC"}
                                                       },
                                           Data = new StudyEntryData {DeleteTime = null, StoreTime = null}
                                       },
                               };
        }

        #region IStudyStoreQuery Members

        public GetStudyCountResult GetStudyCount(GetStudyCountRequest request)
        {
            return new GetStudyCountResult {StudyCount = StudyEntries.Count};
        }

        public GetStudyEntriesResult GetStudyEntries(GetStudyEntriesRequest request)
        {
            return new GetStudyEntriesResult {StudyEntries = StudyEntries};
        }

        public GetSeriesEntriesResult GetSeriesEntries(GetSeriesEntriesRequest request)
        {
            throw new NotImplementedException();
        }

        public GetImageEntriesResult GetImageEntries(GetImageEntriesRequest request)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

#endif