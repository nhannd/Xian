using System.ServiceModel;

namespace ClearCanvas.Ris.Application.Common.Admin.RequestedProcedureTypeGroupAdmin
{
    /// <summary>
    /// 
    /// </summary>
    [ServiceContract]
    public interface IRequestedProcedureTypeGroupAdminService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        GetRequestedProcedureTypeGroupEditFormDataResponse GetRequestedProcedureTypeGroupEditFormData(
            GetRequestedProcedureTypeGroupEditFormDataRequest request);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        ListRequestedProcedureTypeGroupsResponse ListRequestedProcedureTypeGroups(
            ListRequestedProcedureTypeGroupsRequest request);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        LoadRequestedProcedureTypeGroupForEditResponse LoadRequestedProcedureTypeGroupForEdit(
            LoadRequestedProcedureTypeGroupForEditRequest request);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        AddRequestedProcedureTypeGroupResponse AddRequestedProcedureTypeGroup(
            AddRequestedProcedureTypeGroupRequest request);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof (RequestValidationException))]
        [FaultContract(typeof (ConcurrentModificationException))]
        UpdateRequestedProcedureTypeGroupResponse UpdateRequestedProcedureTypeGroup(
            UpdateRequestedProcedureTypeGroupRequest request);
    }
}