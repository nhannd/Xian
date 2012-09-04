#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    /// <summary>
    /// Request object for <see cref="IReportingWorkflowService.GetPriors"/>.
    /// </summary>
    /// <remarks>
    /// </remarks>
    [DataContract]
    public class GetPriorsRequest : DataContractBase
    {
        /// <summary>
        /// A report for which relevant priors are obtained.  Only one of ReportRef and OrderRef should be set.
        /// </summary>
        [DataMember]
        public EntityRef ReportRef;

        /// <summary>
		/// An order for which relevant priors are obtained.  Only one of ReportRef and OrderRef should be set.
        /// </summary>
        [DataMember]
        public EntityRef OrderRef;

		/// <summary>
		/// Specifies whether only relevant priors should be returned, as opposed to all priors for the patient.
		/// </summary>
		[DataMember]
    	public bool RelevantOnly;

    }
}
