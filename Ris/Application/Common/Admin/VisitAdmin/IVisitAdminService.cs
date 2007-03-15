using System;
using System.ServiceModel;

namespace ClearCanvas.Ris.Application.Common.Admin.VisitAdmin
{
    /// <summary>
    /// Provides data loading/saving for the <see cref="VisitEditorComponent"/>
    /// </summary>
    [ServiceContract]
    public interface IVisitAdminService
    {
        /// <summary>
        /// Loads all form data for the <see cref="VisitEditorComponent"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        LoadVisitEditorFormDataResponse LoadVisitEditorFormData(LoadVisitEditorFormDataRequest request);

        /// <summary>
        /// Loads a list of visit summaries for a specified patient
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        ListVisitsForPatientResponse ListVisitsForPatient(ListVisitsForPatientRequest request);

        /// <summary>
        /// Loads all visit data for the <see cref="VisitEditorComponent"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        LoadVisitForAdminEditResponse LoadVisitForAdminEdit(LoadVisitForAdminEditRequest request);

        /// <summary>
        /// Saves changes to a visit made via the <see cref="VisitEditorComponent"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        SaveAdminEditsForVisitResponse SaveAdminEditsForVisit(SaveAdminEditsForVisitRequest request);
    }
}
