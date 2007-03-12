using System;
using System.ServiceModel;
using System.Text;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.HL7Admin
{
    /// <summary>
    /// 
    /// </summary>
    [ServiceContract]
    public interface IHL7QueueService
    {
        /// <summary>
        /// Provides all form data for the HL7 queue preview screen
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        GetHL7QueueFormDataResponse GetHL7QueueFormData(GetHL7QueueFormDataRequest request);

        /// <summary>
        /// Provides a list of HL7 queue items matching a specified set of criteria
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        ListHL7QueueItemsResponse ListHL7QueueItems(ListHL7QueueItemsRequest request);

        /// <summary>
        /// Load details for a specified HL7 queue item
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        LoadHL7QueueItemResponse LoadHL7QueueItem(LoadHL7QueueItemRequest request);

        /// <summary>
        /// Manually [re]process a specified HL7 queue item
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(HL7ProcessingException))]
        ProcessHL7QueueItemResponse ProcessHL7QueueItem(ProcessHL7QueueItemRequest request);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        SetHL7QueueItemCompleteResponse SetHL7QueueItemComplete(SetHL7QueueItemCompleteRequest request);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        SetHL7QueueItemErrorResponse SetHL7QueueItemError(SetHL7QueueItemErrorRequest request);

        /// <summary>
        /// Determines the patient referenced by a particular HL7 message
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        GetReferencedPatientResponse GetReferencedPatient(GetReferencedPatient request);
    }
}
