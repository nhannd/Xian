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
using ClearCanvas.Common;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageServer.Common.CommandProcessor;
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

            using (ExecutionContext scope = new ExecutionContext())
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
