#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Dicom.Iod.Sequences;

namespace ClearCanvas.Dicom.Iod.Modules
{
    /// <summary>
    /// Scheduled Procedure Step Modole
    /// </summary>
    /// <remarks>As per Dicom Doc 3, Table C.4-10 (pg 246)</remarks>
    public class ScheduledProcedureStepModuleIod : IodBase
    {
        #region Constructors
        /// <summary>
		/// Constructor.
		/// </summary>
        public ScheduledProcedureStepModuleIod()
            :base()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
		public ScheduledProcedureStepModuleIod(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider)
        {
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the scheduled procedure step sequence list.
        /// </summary>
        /// <value>The scheduled procedure step sequence list.</value>
        public SequenceIodList<ScheduledProcedureStepSequenceIod> ScheduledProcedureStepSequenceList
        {
            get 
            {
                return new SequenceIodList<ScheduledProcedureStepSequenceIod>(base.DicomAttributeProvider[DicomTags.ScheduledProcedureStepSequence] as DicomAttributeSQ); 
            }
        }

       #endregion

    }


}
