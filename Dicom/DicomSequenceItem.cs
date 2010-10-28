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
    /// A class representing a DICOM Sequence Item.
    /// </summary>
    public class DicomSequenceItem : DicomAttributeCollection
    {
        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        public DicomSequenceItem() : base(0x00000000,0xFFFFFFFF)
        {
        }

        /// <summary>
        /// Internal constructor used when making a copy of a <see cref="DicomAttributeCollection"/>.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="copyBinary"></param>
        /// <param name="copyPrivate"></param>
        /// <param name="copyUnknown"></param>
        internal DicomSequenceItem(DicomAttributeCollection source, bool copyBinary, bool copyPrivate, bool copyUnknown)
            : base(source, copyBinary, copyPrivate, copyUnknown)
        {
        }
        #endregion

        #region Public Overridden Methods
        /// <summary>
        /// Create a copy of this DicomSequenceItem.
        /// </summary>
        /// <returns>The copied DicomSequenceItem.</returns>
        public override DicomAttributeCollection Copy()
        {
        	return Copy(true, true, true);
        }

    	/// <summary>
    	/// Creates a copy of this DicomSequenceItem.
    	/// </summary>
    	/// <param name="copyBinary">When set to false, the copy will not include <see cref="DicomAttribute"/>
    	/// instances that are of type <see cref="DicomAttributeOB"/>, <see cref="DicomAttributeOW"/>,
    	/// or <see cref="DicomAttributeOF"/>.</param>
    	/// <param name="copyPrivate">When set to false, the copy will not include Private tags</param>
    	/// <param name="copyUnknown">When set to false, the copy will not include UN VR tags</param>
    	/// <returns>The copied DicomSequenceItem.</returns>
    	public override DicomAttributeCollection Copy(bool copyBinary, bool copyPrivate, bool copyUnknown)
        {
            return new DicomSequenceItem(this,copyBinary,copyPrivate,copyUnknown);
        }
        #endregion
    }
}
