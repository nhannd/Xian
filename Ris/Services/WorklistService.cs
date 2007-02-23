using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Workflow;

namespace ClearCanvas.Ris.Services
{
    [ExtensionOf(typeof(ClearCanvas.Enterprise.ServiceLayerExtensionPoint))]
    public class WorklistService : WorkflowServiceBase, IWorklistService
    {
        [ReadOperation]
        public IList GetWorklist(string worklistClassName)
        {
            return GetWorklist(worklistClassName, null);
        }

        [ReadOperation]
        public IList GetWorklist(string worklistClassName, SearchCriteria additionalCriteria)
        {
            IExtensionPoint worklistExtPoint = new ClearCanvas.Healthcare.Workflow.Registration.WorklistExtensionPoint();
            IWorklist worklist = (IWorklist)worklistExtPoint.CreateExtension(new ClassNameExtensionFilter(worklistClassName));
            return worklist.GetWorklist(this.CurrentContext, additionalCriteria);
        }

        [ReadOperation]
        public IList GetQueryResultForWorklistItem(string worklistClassName, IWorklistItem item)
        {
            IExtensionPoint worklistExtPoint = new ClearCanvas.Healthcare.Workflow.Registration.WorklistExtensionPoint();
            IWorklist worklist = (IWorklist)worklistExtPoint.CreateExtension(new ClassNameExtensionFilter(worklistClassName));
            return worklist.GetQueryResultForWorklistItem(this.CurrentContext, item);
        }

        [ReadOperation]
        public RequestedProcedure LoadRequestedProcedure(EntityRef<RequestedProcedure> rpRef, bool loadDetail)
        {
            IRequestedProcedureBroker rpBroker = CurrentContext.GetBroker<IRequestedProcedureBroker>();
            IOrderBroker orderBroker = CurrentContext.GetBroker<IOrderBroker>();

            RequestedProcedure rp = rpBroker.Load(rpRef);

            if (loadDetail)
            {
                rpBroker.LoadCheckInProcedureStepsForRequestedProcedure(rp);
                rpBroker.LoadModalityProcedureStepsForRequestedProcedure(rp);
                rpBroker.LoadOrderForRequestedProcedure(rp);
                orderBroker.LoadOrderingFacilityForOrder(rp.Order);
                orderBroker.LoadOrderingPractitionerForOrder(rp.Order);
                rpBroker.LoadTypeForRequestedProcedure(rp);
            }

            return rp;
        }

        [UpdateOperation]
        public void UpdateRequestedProcedure(RequestedProcedure rp)
        {
            this.CurrentContext.Lock(rp, DirtyState.Dirty);
        }

        [UpdateOperation]
        public void AddCheckInProcedureStep(CheckInProcedureStep cps)
        {
            this.CurrentContext.Lock(cps, DirtyState.New);
        }

        [ReadOperation]
        public IWorklistItem LoadWorklistItemPreview(IWorklistQueryResult result)
        {
            //IModalityProcedureStepBroker spsBroker = this.CurrentContext.GetBroker<IModalityProcedureStepBroker>();
            //IRequestedProcedureBroker rpBroker = this.CurrentContext.GetBroker<IRequestedProcedureBroker>();
            //IOrderBroker orderBroker = this.CurrentContext.GetBroker<IOrderBroker>();
            //IPatientBroker patientBroker = this.CurrentContext.GetBroker<IPatientBroker>();

            //ModalityProcedureStep sps = spsBroker.Load(item.ProcedureStep);

            //// force a whole bunch of relationships to load... this could be optimized by using fetch joins
            ////spsBroker.LoadRequestedProcedureForModalityProcedureStep(sps);
            ////rpBroker.LoadOrderForRequestedProcedure(sps.RequestedProcedure);
            //orderBroker.LoadOrderingFacilityForOrder(sps.RequestedProcedure.Order);

            //// ensure that these associations are loaded
            //orderBroker.LoadDiagnosticServiceForOrder(sps.RequestedProcedure.Order);
            //spsBroker.LoadTypeForModalityProcedureStep(sps);
            //rpBroker.LoadTypeForRequestedProcedure(sps.RequestedProcedure);


            //patientBroker.LoadProfilesForPatient(sps.RequestedProcedure.Order.Patient);
            //return sps;
            return null;
        }

        [UpdateOperation]
        public void ExecuteOperation(EntityRef<ModalityProcedureStep> stepRef, string operationClassName)
        {
            ExecuteOperation(LoadStep(stepRef),
                new ClearCanvas.Healthcare.Workflow.Modality.WorkflowOperationExtensionPoint(), operationClassName);
        }

        [ReadOperation]
        public IDictionary<string, bool> GetOperationEnablement(EntityRef<ModalityProcedureStep> stepRef)
        {
            return GetOperationEnablement(LoadStep(stepRef),
                new ClearCanvas.Healthcare.Workflow.Modality.WorkflowOperationExtensionPoint());
        }

        private ModalityProcedureStep LoadStep(EntityRef<ModalityProcedureStep> stepRef)
        {
            IModalityProcedureStepBroker broker = this.CurrentContext.GetBroker<IModalityProcedureStepBroker>();
            return broker.Load(stepRef, EntityLoadFlags.CheckVersion);
        }
    
    }
}
