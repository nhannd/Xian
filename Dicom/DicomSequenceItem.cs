using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom
{
    /// <summary>
    /// A class representing a DICOM Sequence Item.
    /// </summary>
    public class DicomSequenceItem : DicomAttributeCollection
    {
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
        internal DicomSequenceItem(DicomAttributeCollection source, bool copyBinary)
            : base(source, copyBinary)
        {
        }

        /// <summary>
        /// Create a copy of this DicomSequenceItem.
        /// </summary>
        /// <returns>The copied DicomSequenceItem.</returns>
        public override DicomAttributeCollection Copy()
        {
            return Copy(true);
        }

        /// <summary>
        /// Creates a copy of this DicomSequenceItem.
        /// </summary>
        /// <param name="copyBinary">When set to false, the copy will not include <see cref="DicomAttribute"/>
        /// instances that are of type <see cref="DicomAttributeOB"/>, <see cref="DicomAttributeOW"/>,
        /// or <see cref="DicomAttributeOF"/>.</param>
        /// <returns>The copied DicomSequenceItem.</returns>
        public override DicomAttributeCollection Copy(bool copyBinary)
        {
            return new DicomSequenceItem(this,copyBinary);
        }

    }
}
