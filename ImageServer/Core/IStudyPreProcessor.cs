#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Core
{
    /// <summary>
    /// Defines the interface of the Pre-Processors to execute on a file
    /// </summary>
    internal interface IStudyPreProcessor
    {
        /// <summary>
        /// Called to process a DICOM file.
        /// </summary>
        /// <param name="file"></param>
        /// <returns>An instance of <see cref="InstancePreProcessingResult"/> containing the result of the processing. NULL if 
        /// the change has been made to the file.</returns>
        InstancePreProcessingResult Process(DicomFile file);

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
}