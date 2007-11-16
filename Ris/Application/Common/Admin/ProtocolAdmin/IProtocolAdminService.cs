using System.ServiceModel;

namespace ClearCanvas.Ris.Application.Common.Admin.ProtocolAdmin
{
    /// <summary>
    /// Provides operations to administer protocol codes and protocol groups
    /// </summary>
    [ServiceContract]
    public interface IProtocolAdminService
    {
        /// <summary>
        /// Adds a new protocol code with specified name and description (optional)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        AddProtocolCodeResponse AddProtocolCode(AddProtocolCodeRequest request);

        /// <summary>
        /// Updates name and/or description of specified protocol code
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        UpdateProtocolCodeResponse UpdateProtocolCode(UpdateProtocolCodeRequest request);

        /// <summary>
        /// Marks a protocol code as deleted
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        DeleteProtocolCodeResponse DeleteProtocolCode(DeleteProtocolCodeRequest request);

        /// <summary>
        /// Summary list of all protocol groups
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        ListProtocolGroupsResponse ListProtocolGroups(ListProtocolGroupsRequest request);

        /// <summary>
        /// Loads details for specified protocol group
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        LoadProtocolGroupForEditResponse LoadProtocolGroupForEdit(LoadProtocolGroupForEditRequest request);

        /// <summary>
        /// Provides a list of available protocol codes and reading groups that can be assigned while adding/updating a protocol group
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        GetProtocolGroupEditFormDataResponse GetProtocolGroupEditFormData(GetProtocolGroupEditFormDataRequest request);

        /// <summary>
        /// Adds a new protocol group
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        AddProtocolGroupResponse AddProtocolGroup(AddProtocolGroupRequest request);

        /// <summary>
        /// Updates an existing protocol group
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        UpdateProtocolGroupResponse UpdateProtocolGroup(UpdateProtocolGroupRequest request);

        /// <summary>
        /// Marks a protocol group as deleted
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        DeleteProtocolGroupResonse DeleteProtocolGroup(DeleteProtocolGroupRequest request);
    }
}
