#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.Dicom.Iod.Modules
{
    /// <summary>
    /// Print Job Module as per Part 3 Table C.13-8 page 873
    /// </summary>
    public class PrintJobModuleIod : IodBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PrintJobModuleIod"/> class.
        /// </summary>
        public PrintJobModuleIod()
            :base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrintJobModuleIod"/> class.
        /// </summary>
        /// <param name="dicomAttributeCollection">The dicom attribute collection.</param>
        public PrintJobModuleIod(DicomAttributeCollection dicomAttributeCollection)
            :base(dicomAttributeCollection)
        {
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the execution status of print job.
        /// </summary>
        /// <value>The execution status.</value>
        public ExecutionStatus ExecutionStatus
        {
            get { return IodBase.ParseEnum<ExecutionStatus>(base.DicomAttributeCollection[DicomTags.ExecutionStatus].GetString(0, String.Empty), ExecutionStatus.None); }
            set { IodBase.SetAttributeFromEnum(base.DicomAttributeCollection[DicomTags.ExecutionStatus], value, false); }
        }

        /// <summary>
        /// Gets or sets the execution status info.
        /// <para> Additional information about <see cref="ExecutionStatus"/> (2100,0020). </para>
        /// <para>Defined Terms when the Execution Status is DONE or PRINTING: NORMAL</para>
        /// <para>Defined Terms when the Execution Status is FAILURE: </para>
        /// <para>INVALID PAGE DES = The specified page layout cannot be printed or other page description errors have been detected.</para>
        /// <para>INSUFFIC MEMORY = There is not enough memory available to complete this job.</para>
        /// See Section C.13.9.1 for additional Defined Terms when the Execution Status is PENDING or FAILURE.</para>
        /// </summary>
        /// <value>The execution status info.</value>
        public string ExecutionStatusInfo
        {
            get { return base.DicomAttributeCollection[DicomTags.ExecutionStatusInfo].GetString(0, String.Empty); }
            set { base.DicomAttributeCollection[DicomTags.ExecutionStatusInfo].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the date of print job creation.
        /// </summary>
        /// <value>The creation date.</value>
        public DateTime? CreationDate
        {
            get { return DateTimeParser.ParseDateAndTime(base.DicomAttributeCollection, 0, DicomTags.CreationDate, DicomTags.CreationTime); }
        
          set { DateTimeParser.SetDateTimeAttributeValues(value, base.DicomAttributeCollection, 0, DicomTags.CreationDate, DicomTags.CreationTime); }
        }

        /// <summary>
        /// Gets or sets the print priority.
        /// </summary>
        /// <value>The print priority.</value>
        public PrintPriority PrintPriority
        {
            get { return IodBase.ParseEnum<PrintPriority>(base.DicomAttributeCollection[DicomTags.PrintPriority].GetString(0, String.Empty), PrintPriority.None); }
            set { IodBase.SetAttributeFromEnum(base.DicomAttributeCollection[DicomTags.PrintPriority], value, false); }
        }

        /// <summary>
        /// Gets or sets the user defined name identifying the printer.
        /// </summary>
        /// <value>The name of the printer.</value>
        public string PrinterName
        {
            get { return base.DicomAttributeCollection[DicomTags.PrinterName].GetString(0, String.Empty); }
            set { base.DicomAttributeCollection[DicomTags.PrinterName].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the DICOM Application Entity Title that issued the print operation.
        /// </summary>
        /// <value>The originator.</value>
        public string Originator
        {
            get { return base.DicomAttributeCollection[DicomTags.Originator].GetString(0, String.Empty); }
            set { base.DicomAttributeCollection[DicomTags.Originator].SetString(0, value); }
        }
        
        #endregion

        #region Public Methods
        /// <summary>
        /// Sets the commonly used tags in the base dicom attribute collection.
        /// </summary>
        public void SetCommonTags()
        {
            SetCommonTags(base.DicomAttributeCollection);
        }
        #endregion

        #region Public Static Methods
        /// <summary>
        /// Sets the commonly used tags in the specified dicom attribute collection.
        /// </summary>
        /// <param name="dicomAttributeCollection">The dicom attribute collection.</param>
        public static void SetCommonTags(DicomAttributeCollection dicomAttributeCollection)
        {
            if (dicomAttributeCollection == null)
                throw new ArgumentNullException("dicomAttributeCollection");

            //dicomAttributeCollection[DicomTags.NumberOfCopies].SetNullValue();
            //dicomAttributeCollection[DicomTags.PrintPriority].SetNullValue();
            //dicomAttributeCollection[DicomTags.MediumType].SetNullValue();
            //dicomAttributeCollection[DicomTags.FilmDestination].SetNullValue();
            //dicomAttributeCollection[DicomTags.FilmSessionLabel].SetNullValue();
            //dicomAttributeCollection[DicomTags.MemoryAllocation].SetNullValue();
            //dicomAttributeCollection[DicomTags.OwnerId].SetNullValue();
        }
        #endregion
    }

    #region ExecutionStatus Enum
    /// <summary>
    /// Execution status of print job.
    /// </summary>
    public enum ExecutionStatus
    {
        /// <summary>
        /// 
        /// </summary>
        None,
        /// <summary>
        /// 
        /// </summary>
        Pending,
        /// <summary>
        /// 
        /// </summary>
        Printing,
        /// <summary>
        /// 
        /// </summary>
        Done,
        /// <summary>
        /// 
        /// </summary>
        Failure
    }
    #endregion
    
}

