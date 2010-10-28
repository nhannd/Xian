#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Dicom.Iod.Macros;

namespace ClearCanvas.Dicom.Iod.Sequences
{
    /// <summary>
    /// Film Consumption Sequence.  
    /// </summary>
    /// <remarks>As per Part 3, Table C4.17, pg 260</remarks>
    public class BillingSuppliesAndDevicesSequenceIod : SequenceIodBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BillingSuppliesAndDevicesSequenceIod"/> class.
        /// </summary>
        public BillingSuppliesAndDevicesSequenceIod()
            :base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BillingSuppliesAndDevicesSequenceIod"/> class.
        /// </summary>
        /// <param name="dicomSequenceItem">The dicom sequence item.</param>
        public BillingSuppliesAndDevicesSequenceIod(DicomSequenceItem dicomSequenceItem)
            : base(dicomSequenceItem)
        {
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Code values of chemicals, supplies or devices required for billing. The sequence may have zero or one Items.
        /// </summary>
        /// <value>The billing item sequence list.</value>
        public SequenceIodList<CodeSequenceMacro> BillingItemSequenceList
        {
            get
            {
                return new SequenceIodList<CodeSequenceMacro>(base.DicomAttributeProvider[DicomTags.BillingItemSequence] as DicomAttributeSQ);
            }
        }

        /// <summary>
        /// Sequence containing the quantity of used chemicals or devices. The sequence may have zero or one Items.
        /// </summary>
        /// <value>The quantity sequence list.</value>
        public SequenceIodList<QuantitySequenceIod> QuantitySequenceList
        {
            get
            {
                return new SequenceIodList<QuantitySequenceIod>(base.DicomAttributeProvider[DicomTags.QuantitySequence] as DicomAttributeSQ);
            }
        }
        
                
        #endregion
    }

    
}
