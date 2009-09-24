using System;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Core.Edit;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue.StudyProcess
{
    /// <summary>
    /// Defines the interface of the Pre-Processors that is called
    /// by the <see cref="StudyProcessItemProcessor"/> 
    /// </summary>
    internal interface IStudyPreProcessor
    {
        /// <summary>
        /// Called to process a DICOM file.
        /// </summary>
        /// <param name="file"></param>
        /// <returns>An instance of <see cref="PreProcessingResult"/> containing the result of the processing. NULL if 
        /// the change has been made to the file.</returns>
        PreProcessingResult Process(DicomFile file);

        /// <summary>
        /// Gets or sets the <see cref="StudyStorageLocation"/> of the study which the 
        /// DICOM file(s) belong to.
        /// </summary>
        StudyStorageLocation StorageLocation { get; set;}

        /// <summary>
        /// Gets or sets the description of the pre-processor.
        /// </summary>
        string Description
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Represents the result of the DICOM file preprocessing.
    /// </summary>
    public class PreProcessingResult
    {
        #region Private Members
        private bool _autoReconciled;
        private bool _discardImage;
        #endregion

        /// <summary>
        /// Indicates whether the file has been updated 
        /// as part of auto-reconciliation process.
        /// </summary>
        public bool AutoReconciled
        {
            get { return _autoReconciled; }
            set { _autoReconciled = value; }
        }

        /// <summary>
        /// Indicates whether or not the file should be discarded.
        /// </summary>
        public bool DiscardImage
        {
            get { return _discardImage; }
            set { _discardImage = value; }
        }
    }
    
    /// <summary>
    /// Represents changes applied to a DICOM
    /// </summary>
    public class UpdateItem
    {
        #region Private Members
        private readonly DicomTag _tag;
        private readonly string _originalValue;
        private readonly string _newValue;
        
        #endregion
        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="UpdateItem"/>
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="originalValue"></param>
        /// <param name="newValue"></param>
        public UpdateItem(uint tag, string originalValue, string newValue)
        {
            _tag = DicomTagDictionary.GetDicomTag(tag);
            _originalValue = originalValue;
            _newValue = newValue;
        }

        /// <summary>
        /// Creates an instance of <see cref="UpdateItem"/>
        /// </summary>
        /// <param name="command"></param>
        /// <param name="file"></param>
        public UpdateItem(IUpdateImageTagCommand command, DicomFile file)
        {
            _tag = command.UpdateEntry.TagPath.Tag;
            _originalValue = file.DataSet[Tag].ToString();
            _newValue = command.UpdateEntry.Value != null ? command.UpdateEntry.Value.ToString() : String.Empty;
        }
        
        #endregion
        #region Public Properties

        /// <summary>
        /// Gets the DICOM tag being updated.
        /// </summary>
        public DicomTag Tag
        {
            get { return _tag; }
        }

        /// <summary>
        /// Gets the original value of the DICOM tag being updated.
        /// </summary>
        public string OriginalValue
        {
            get { return _originalValue; }
        }

        /// <summary>
        /// Gets the new value of the DICOM tag being updated.
        /// </summary>
        public string NewValue
        {
            get { return _newValue; }
        } 
        #endregion
    }
}
