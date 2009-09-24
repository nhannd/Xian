using System;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Helpers;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Edit;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue.StudyProcess
{
    /// <summary>
    /// Class implementing <see cref="IStudyPreProcessor"/> to correct the patient's name
    /// in the DICOM file if it doesn't match what is in the system.
    /// </summary>
    class PatientNameAutoCorrection: BasePreprocessor, IStudyPreProcessor
    {
        class PatientNameAutoCorrectionResult : PreProcessingResult
        {
            private string _oldPatientsName;
            private string _newPatientsName;

            public string NewPatientsName
            {
                get { return _newPatientsName; }
                set { _newPatientsName = value; }
            }

            public string OldPatientsName
            {
                get { return _oldPatientsName; }
                set { _oldPatientsName = value; }
            }

        }
        
        #region Private Members
        private string _contextID; 
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of <see cref="PatientNameAutoCorrection"/> to fix
        /// the patient's name in a DICOM file to match a study.
        /// </summary>
        /// <param name="contextID"></param>
        /// <param name="storageLocation"></param>
        public PatientNameAutoCorrection(String contextID, StudyStorageLocation storageLocation)
            : base("AUTO-CORRECTION : PATIENT'S NAME", storageLocation)
        {
            _contextID = contextID;
        }
        #endregion

        #region IStudyPreProcessor Members

        public PreProcessingResult Process(DicomFile file)
        {

            PatientNameAutoCorrectionResult preProcessingResult = null;

            if (StorageLocation.Study!=null)
            {
                StudyComparer comparer = new StudyComparer();
                DifferenceCollection list = comparer.Compare(file, StorageLocation.Study, StorageLocation.ServerPartition.GetComparisonOptions());

                if (list.Count == 1)
                {
                    ComparisionDifference different = list[0];
                    if (different.DicomTag.TagValue == DicomTags.PatientsName)
                    {
                        #region Fix the patient's name if possible
                        string patientsNameInFile = file.DataSet[DicomTags.PatientsName].ToString();
                        if (DicomNameUtils.LookLikeSameNames(patientsNameInFile, StorageLocation.Study.PatientsName))
                        {
                            using (ServerCommandProcessor processor = new ServerCommandProcessor("Update Image According to History"))
                            {
                                SetTagCommand command = new SetTagCommand(file, DicomTags.PatientsName, patientsNameInFile, StorageLocation.Study.PatientsName);
                                processor.AddCommand(command);
                                
                                if (!processor.Execute())
                                {
                                    if (processor.FailureException != null)
                                    {
                                        throw processor.FailureException;
                                    }
                                    else
                                        throw new ApplicationException(String.Format("AUTO-RECONCILE Failed: Unable to correct the patient's name in the image. Reason: {0}", processor.FailureReason));
                                }

                                preProcessingResult = new PatientNameAutoCorrectionResult();
                                preProcessingResult.OldPatientsName = patientsNameInFile;
                                preProcessingResult.NewPatientsName = StorageLocation.Study.PatientsName;

                                
                            }
                        }
                        else
                        {
                            // patients name does not look like what's in the db.. will be copied to reconcile queue later.
                        }
                        #endregion
                    }
                }
            }

            if (preProcessingResult != null)
            {
                StringBuilder log = new StringBuilder();
                log.AppendLine(
                    String.Format("AUTO-CORRECTION: Patient's Name: {0} ==> {1}. SOP {2}", 
                                  preProcessingResult.OldPatientsName,
                                  preProcessingResult.NewPatientsName,
                                  file.MediaStorageSopInstanceUid));
                Platform.Log(LogLevel.Info, log.ToString());

            }
            return preProcessingResult;
        }


       

        #endregion
    }
}