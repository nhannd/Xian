using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Workflow;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
    public abstract class WorklistService : WorkflowServiceBase//, IWorklistService
    {
        protected IList GetWorklist(string worklistClassName)
        {
            return GetWorklist(worklistClassName, null);
        }

        protected IList GetWorklist(string worklistClassName, SearchCriteria additionalCriteria)
        {
            IExtensionPoint worklistExtPoint = new ClearCanvas.Healthcare.Workflow.Registration.WorklistExtensionPoint();
            IWorklist worklist = (IWorklist)worklistExtPoint.CreateExtension(new ClassNameExtensionFilter(worklistClassName));
            return worklist.GetWorklist(this.PersistenceContext, additionalCriteria);
        }

        protected IList GetQueryResultForWorklistItem(string worklistClassName, IWorklistItem item)
        {
            IExtensionPoint worklistExtPoint = new ClearCanvas.Healthcare.Workflow.Registration.WorklistExtensionPoint();
            IWorklist worklist = (IWorklist)worklistExtPoint.CreateExtension(new ClassNameExtensionFilter(worklistClassName));
            return worklist.GetQueryResultForWorklistItem(this.PersistenceContext, item);
        }

        protected RequestedProcedure LoadRequestedProcedure(EntityRef rpRef, bool loadDetail)
        {
            IRequestedProcedureBroker rpBroker = PersistenceContext.GetBroker<IRequestedProcedureBroker>();
            IOrderBroker orderBroker = PersistenceContext.GetBroker<IOrderBroker>();

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

        protected void UpdateRequestedProcedure(RequestedProcedure rp)
        {
            this.PersistenceContext.Lock(rp, DirtyState.Dirty);
        }

        protected void AddCheckInProcedureStep(CheckInProcedureStep cps)
        {
            this.PersistenceContext.Lock(cps, DirtyState.New);
        }

        protected IWorklistItem LoadWorklistItemPreview(IWorklistQueryResult result)
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

        protected void ExecuteOperation(EntityRef stepRef, string operationClassName)
        {
            ExecuteOperation(LoadStep(stepRef),
                new ClearCanvas.Healthcare.Workflow.Modality.WorkflowOperationExtensionPoint(), operationClassName);
        }

        protected IDictionary<string, bool> GetOperationEnablement(EntityRef stepRef)
        {
            return GetOperationEnablement(LoadStep(stepRef),
                new ClearCanvas.Healthcare.Workflow.Modality.WorkflowOperationExtensionPoint());
        }

        protected ModalityProcedureStep LoadStep(EntityRef stepRef)
        {
            return PersistenceContext.GetBroker<IModalityProcedureStepBroker>().Load(stepRef, EntityLoadFlags.CheckVersion);
        }

    }
}
