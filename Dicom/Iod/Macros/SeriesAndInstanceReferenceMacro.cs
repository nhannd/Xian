#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Dicom.Iod.Sequences;

namespace ClearCanvas.Dicom.Iod.Macros
{
    /// <summary>
    /// Series and Instance Reference Macro Attributes
    /// </summary>
    /// <remarks>As per Dicom Doc 3, Table 10.4 (pg 78)</remarks>
    public class SeriesAndInstanceReferenceMacro : SequenceIodBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SeriesAndInstanceReferenceMacro"/> class.
        /// </summary>
        public SeriesAndInstanceReferenceMacro()
            :base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SeriesAndInstanceReferenceMacro"/> class.
        /// </summary>
        /// <param name="dicomSequenceItem">The dicom sequence item.</param>
        public SeriesAndInstanceReferenceMacro(DicomSequenceItem dicomSequenceItem)
            : base(dicomSequenceItem)
        {
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Sequence of Items each of which includes the Attributes of one Series. 
        /// One or more Items shall be present. (0008,1115)
        /// </summary>
        /// <value>The referenced series sequence list.</value>
        public SequenceIodList<ReferencedSeriesSequenceIod> ReferencedSeriesSequenceList
        {
            get
            {
                return new SequenceIodList<ReferencedSeriesSequenceIod>(base.DicomAttributeProvider[DicomTags.ReferencedSeriesSequence] as DicomAttributeSQ);
            }
        } 
        #endregion

    }
}
