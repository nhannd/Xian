using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Alert;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.PatientReconciliation;
using ClearCanvas.Healthcare.Workflow.Registration;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Application.Services.RegistrationWorkflow
{
    [ServiceImplementsContract(typeof(IRegistrationWorkflowService))]
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    public class RegistrationWorkflowService : WorklistService, IRegistrationWorkflowService
    {
        IList<IPatientAlert> _patientAlertTests;

        public RegistrationWorkflowService()
        {
            PatientAlertExtensionPoint xp = new PatientAlertExtensionPoint();
            object[] tests = xp.CreateExtensions();

            _patientAlertTests = new List<IPatientAlert>();
            foreach (object o in tests)
            {
                _patientAlertTests.Add((IPatientAlert)o);
            }
        }

        [ReadOperation]
        public GetWorklistResponse GetWorklist(GetWorklistRequest request)
        {
            RegistrationWorkflowAssembler assembler = new RegistrationWorkflowAssembler();
            return new GetWorklistResponse(
                CollectionUtils.Map<WorklistItem, RegistrationWorklistItem, List<RegistrationWorklistItem>>(
                    GetWorklist(request.WorklistClassName, assembler.CreatePatientProfileSearchCriteria(request.SearchCriteria)),
                    delegate(WorklistItem item)
                    {
                        return assembler.CreateRegistrationWorklistItem(item, this.PersistenceContext);
                    }));
        }

        [ReadOperation]
        public LoadWorklistPreviewResponse LoadWorklistPreview(LoadWorklistPreviewRequest request)
        {
            PatientProfile profile = (PatientProfile)PersistenceContext.Load(request.WorklistItem.PatientProfileRef);
            PersistenceContext.GetBroker<IPatientProfileBroker>().LoadPatientForPatientProfile(profile);
            PersistenceContext.GetBroker<IPatientBroker>().LoadNotesForPatient(profile.Patient);
            List<AlertNotificationDetail> alertNotifications = GetAlertsHelper(profile.Patient, this.PersistenceContext);

            IPatientReconciliationStrategy strategy = (IPatientReconciliationStrategy)(new PatientReconciliationStrategyExtensionPoint()).CreateExtension();
            IList<PatientProfileMatch> matches = strategy.FindReconciliationMatches(profile, PersistenceContext);

            RegistrationWorkflowAssembler assembler = new RegistrationWorkflowAssembler();
            return new LoadWorklistPreviewResponse(assembler.CreateRegistrationWorklistPreview(
                request.WorklistItem.PatientProfileRef, 
                GetQueryResultForWorklistItem(request.WorklistItem.WorklistClassName, 
                    new WorklistItem(request.WorklistItem.WorklistClassName, request.WorklistItem.PatientProfileRef)),
                matches.Count > 0,
                alertNotifications,
                this.PersistenceContext));
        }

        [ReadOperation]
        public GetOperationEnablementResponse GetOperationEnablement(GetOperationEnablementRequest request)
        {
            return new GetOperationEnablementResponse(GetOperationEnablement(request.ProcedureStepRef));
        }

        [UpdateOperation]
        public void ExecuteOperation(ExecuteOperationRequest request)
        {
            ExecuteOperation(request.ProcedureStepRef, request.OperationClassName);
        }

        [ReadOperation]
        public GetDataForCheckInTableResponse GetDataForCheckInTable(GetDataForCheckInTableRequest request)
        {
            IRequestedProcedureBroker rpBroker = PersistenceContext.GetBroker<IRequestedProcedureBroker>();
            IOrderBroker orderBroker = PersistenceContext.GetBroker<IOrderBroker>();

            List<WorklistQueryResult> listQueryResult = (List<WorklistQueryResult>) GetQueryResultForWorklistItem(request.WorklistClassName, new WorklistItem(request.WorklistClassName, request.PatientProfileRef));
            List<EntityRef> rpRefList = new List<EntityRef>();
            List<CheckInTableItem> checkInItemList = new List<CheckInTableItem>();
            foreach (WorklistQueryResult queryResult in listQueryResult)
            {
                if (rpRefList.Contains(queryResult.RequestedProcedure) == false)
                {
                    rpRefList.Add(queryResult.RequestedProcedure);

                    RequestedProcedure rp = rpBroker.Load(queryResult.RequestedProcedure);
                    rpBroker.LoadOrderForRequestedProcedure(rp);
                    rpBroker.LoadTypeForRequestedProcedure(rp);
                    orderBroker.LoadOrderingFacilityForOrder(rp.Order);
                    orderBroker.LoadOrderingPractitionerForOrder(rp.Order);

                    checkInItemList.Add(new CheckInTableItem(
                            rp.GetRef(), 
                            rp.Type.Name, 
                            rp.Order.SchedulingRequestDateTime, 
                            rp.Order.OrderingFacility.Name));
                }
            }

            return new GetDataForCheckInTableResponse(checkInItemList);
        }

        [UpdateOperation]
        public CheckInProcedureResponse CheckInProcedure(CheckInProcedureRequest request)
        {
            foreach (EntityRef rpRef in request.RequestedProcedures)
            {
                RequestedProcedure rp = PersistenceContext.GetBroker<IRequestedProcedureBroker>().Load(rpRef);
                Staff staff = PersistenceContext.GetBroker<IStaffBroker>().FindOne(new StaffSearchCriteria(request.Staff));

                CheckInProcedureStep cps = new CheckInProcedureStep(rp);
                cps.Start(staff);
                cps.Complete(staff);

                rp.CheckInProcedureSteps.Add(cps);
                PersistenceContext.Lock(rp, DirtyState.Dirty);
            }

            return new CheckInProcedureResponse();
        }

        [ReadOperation]
        public LoadPatientSearchComponentFormDataResponse LoadPatientSearchComponentFormData(LoadPatientSearchComponentFormDataRequest request)
        {
            return new LoadPatientSearchComponentFormDataResponse(
                CollectionUtils.Map<SexEnum, EnumValueInfo, List<EnumValueInfo>>(
                    PersistenceContext.GetBroker<ISexEnumBroker>().Load().Items,
                    delegate(SexEnum e)
                    {
                        return new EnumValueInfo(e.Code.ToString(), e.Value);
                    }));           
        }

        private List<AlertNotificationDetail> GetAlertsHelper(Patient patient, IPersistenceContext context)
        {
            AlertAssembler assembler = new AlertAssembler();
            List<AlertNotificationDetail> results = new List<AlertNotificationDetail>();

            foreach (IPatientAlert patientAlertTests in _patientAlertTests)
            {
                IAlertNotification testResult = patientAlertTests.Test(patient, context);
                if (testResult != null)
                {
                    results.Add(assembler.CreateAlertNotification(testResult));
                }
            }

            return results;
        }
    }
}
