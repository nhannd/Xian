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
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Helpers;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Edit;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Core
{
    /// <summary>
    /// Helper class to update the patient's name 
    /// in the DICOM file based on the rules for patient name.
    /// </summary>
    public class PatientNameRules
    {
        readonly Study _theStudy;

        public PatientNameRules(Study study)
        {
            _theStudy = study;
        }

        #region IStudyPreProcessor Members

        /// <summary>
        /// Updates the Patient's Name tag in the specified <see cref="DicomFile"/>
        /// based on the specified <see cref="StudyStorageLocation"/>. Normalization
        /// may occur.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public UpdateItem Apply(DicomFile file)
        {
            Platform.CheckForNullReference(file, "file");
            
            string orginalPatientsNameInFile = file.DataSet[DicomTags.PatientsName].ToString();

			// Note: only apply the name rules if we can't update it to match the study
            if (!UpdateNameBasedOnTheStudy(file))
                UpdateNameBasedOnRules(file);

            string newPatientName = file.DataSet[DicomTags.PatientsName].ToString();
            UpdateItem change = null;

            if (!newPatientName.Equals(orginalPatientsNameInFile, StringComparison.InvariantCultureIgnoreCase))
            {
                change = new UpdateItem(DicomTags.PatientsName, orginalPatientsNameInFile, newPatientName);

                StringBuilder log = new StringBuilder();
                log.AppendLine(String.Format("AUTO-CORRECTION: SOP {0}", file.MediaStorageSopInstanceUid));
                log.AppendLine(String.Format("\tPatient's Name: {0} ==> {1}. ",
                                             change.OriginalValue, change.NewValue));
                Platform.Log(LogLevel.Info, log.ToString());
            }

            return change;
        }

        private bool UpdateNameBasedOnTheStudy(DicomFile file)
        {
            bool updated = false;
            string orginalPatientsNameInFile = file.DataSet[DicomTags.PatientsName].ToString();

            if (_theStudy==null)
            {
                return false;
            }

            StudyComparer comparer = new StudyComparer();
            ServerPartition partition = ServerPartitionMonitor.Instance.FindPartition(_theStudy.ServerPartitionKey);
            DifferenceCollection list = comparer.Compare(file, _theStudy, partition.GetComparisonOptions());

            if (list.Count == 1)
            {
                ComparisionDifference different = list[0];
                if (different.DicomTag.TagValue == DicomTags.PatientsName)
                {
                    if (DicomNameUtils.LookLikeSameNames(orginalPatientsNameInFile, _theStudy.PatientsName))
                    {
                        using (ServerCommandProcessor processor = new ServerCommandProcessor("Update Patient's Name"))
                        {
                            SetTagCommand command = new SetTagCommand(file, DicomTags.PatientsName, orginalPatientsNameInFile, _theStudy.PatientsName);
                            processor.AddCommand(command);

                            if (!processor.Execute())
                            {
                                throw new ApplicationException(String.Format("AUTO-CORRECTION Failed: Unable to correct the patient's name in the image. Reason: {0}",
                                                                             processor.FailureReason), processor.FailureException);
                            }

                            updated = true;
                        }
                    }
                }
            }
            return updated;
        }

        private static void UpdateNameBasedOnRules(DicomFile file)
        {
            string orginalPatientsNameInFile = file.DataSet[DicomTags.PatientsName].ToString();
            
            if (String.IsNullOrEmpty(orginalPatientsNameInFile))
                return;

            using (ServerCommandProcessor processor = new ServerCommandProcessor("Update Patient's Name"))
            {
                string normPatName = GetAcceptableName(orginalPatientsNameInFile);

                if (!orginalPatientsNameInFile.Equals(normPatName, StringComparison.InvariantCultureIgnoreCase))
                {
                    processor.AddCommand(new SetTagCommand(file, DicomTags.PatientsName, orginalPatientsNameInFile, normPatName));

                    if (!processor.Execute())
                    {
                        throw new ApplicationException(String.Format("AUTO-CORRECTION Failed: Unable to correct the patient's name in the image. Reason: {0}",
                                                                     processor.FailureReason), processor.FailureException);
                    }
                }
            }

            return;
        }

        /// <summary>
        /// Returns an acceptable name for the given name.
        /// </summary>
        /// <param name="originalName"></param>
        /// <returns></returns>
        static public string GetAcceptableName(string originalName)
        {
            return DicomNameUtils.Normalize(originalName, DicomNameUtils.NormalizeOptions.TrimSpaces | DicomNameUtils.NormalizeOptions.TrimEmptyEndingComponents);
        }

        #endregion
    }
}