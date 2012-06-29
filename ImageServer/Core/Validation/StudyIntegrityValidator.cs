#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageServer.Common.Command;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Core.Validation
{
    public class StudyIntegrityValidator
    {
        /// <summary>
        /// Validates the state of the study.
        /// </summary>
        /// <param name="context">Name of the application</param>
        /// <param name="studyStorage">The study to validate</param>
        /// <param name="modes">Specifying what validation to execute</param>
        public void ValidateStudyState(String context, StudyStorageLocation studyStorage, StudyIntegrityValidationModes modes)
        {
            Platform.CheckForNullReference(studyStorage, "studyStorage");
            if (modes == StudyIntegrityValidationModes.None)
                return;

            using (ServerExecutionContext scope = new ServerExecutionContext())
            {

                Study study = studyStorage.LoadStudy(scope.PersistenceContext);
                if (study!=null)
                {
                    StudyXml studyXml = studyStorage.LoadStudyXml();

                    if (modes == StudyIntegrityValidationModes.Default ||
                        (modes & StudyIntegrityValidationModes.InstanceCount) == StudyIntegrityValidationModes.InstanceCount)
                    {
                        if (studyXml != null && studyXml.NumberOfStudyRelatedInstances != study.NumberOfStudyRelatedInstances)
                        {
                            ValidationStudyInfo validationStudyInfo = new ValidationStudyInfo(study, studyStorage.ServerPartition);
                            
                            throw new StudyIntegrityValidationFailure(
                                ValidationErrors.InconsistentObjectCount, validationStudyInfo,
                                String.Format("Number of instances in database and xml do not match: {0} vs {1}.",
                                    study.NumberOfStudyRelatedInstances,
                                    studyXml.NumberOfStudyRelatedInstances
                                ));
                        }
                    }

                }
                
            }
        }
        
    }
}
