#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Core
{
    /// <summary>
    /// Base StudyProcess Preprocessor class.
    /// </summary>
    public abstract class BasePreprocessor
    {
        #region Private Members

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

            StorageLocation = storageLocation;
            Description = description;
        }
        
        #endregion

        #region Public Properties

        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="StudyStorageLocation"/> of the study which the 
        /// DICOM file(s) belong to.
        /// </summary>
        public StudyStorageLocation StorageLocation { get; set; }

        #endregion

    }
}