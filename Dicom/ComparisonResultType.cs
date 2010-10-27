#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Dicom
{
    /// <summary>
    /// Types of differences when two sets of attributes are compared using <see cref="DicomAttributeCollection.Equals()"/>.
    /// </summary>
    public enum ComparisonResultType
    {
        /// <summary>
        /// Cannot be compared with the target because of its type.
        /// </summary>
        InvalidType,

        /// <summary>
        /// Source and target does not have the same set of attributes.
        /// </summary>
        DifferentAttributeSet,

        /// <summary>
        /// Attributes in the source and target have different values.
        /// </summary>
        DifferentValues
    }
}