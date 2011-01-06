#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.ServiceModel;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow
{
    /// <summary>
    /// Provides registration workflow related operations, including retrieving registration worklist, worklist preview, 
    /// patient search, cancel orders and check-in patients
    /// </summary>
    [RisApplicationService]
    [ServiceContract]
	[ServiceKnownType(typeof(RegistrationWorklistItemSummary))]
	public interface IRegistrationWorkflowService : IWorklistService<RegistrationWorklistItemSummary>, IWorkflowService
    {

        /// <summary>
        /// Returns a list of patient profiles based on a textual query.
        /// </summary>
        /// <param name="request"><see cref="TextQueryRequest"/></param>
        /// <returns></returns>
        [OperationContract]
        TextQueryResponse<PatientProfileSummary> PatientProfileTextQuery(TextQueryRequest request);

        /// <summary>
        /// Get procedures that can be checked-in for a patient
        /// </summary>
        /// <param name="request"><see cref="ListProceduresForCheckInRequest"/></param>
        /// <returns><see cref="ListProceduresForCheckInResponse"/></returns>
        [OperationContract]
        ListProceduresForCheckInResponse ListProceduresForCheckIn(ListProceduresForCheckInRequest request);

        /// <summary>
        /// Check in procedures for a patient
        /// </summary>
        /// <param name="request"><see cref="CheckInProcedureRequest"/></param>
        /// <returns><see cref="CheckInProcedureResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(ConcurrentModificationException))]
        CheckInProcedureResponse CheckInProcedure(CheckInProcedureRequest request);
    }
}
