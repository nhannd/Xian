using ClearCanvas.Common;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue.StudyProcess
{
    /// <summary>
    /// Base StudyProcess Preprocessor class.
    /// </summary>
    internal abstract class BasePreprocessor
    {
        #region Private Members
        private StudyStorageLocation _storageLocation;
        private string _description;
        
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of <see cref="AutoReconciler"/> to update
        /// a DICOM file according to the history.
        /// </summary>
        /// <param name="description"></param>
        /// <param name="storageLocation"></param>
        public BasePreprocessor(string description, StudyStorageLocation storageLocation)
        {
            Platform.CheckForEmptyString(description, "description");
            Platform.CheckForNullReference(storageLocation, "storageLocation");

            _storageLocation = storageLocation;
            _description = description;
        }
        
        #endregion

        #region Public Properties

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="StudyStorageLocation"/> of the study which the 
        /// DICOM file(s) belong to.
        /// </summary>
        public StudyStorageLocation StorageLocation
        {
            get
            {
                return _storageLocation;
            }
            set
            {
                _storageLocation = value;
            }
        } 
        #endregion

    }
}