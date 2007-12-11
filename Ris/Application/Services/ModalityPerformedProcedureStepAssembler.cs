using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Workflow;

namespace ClearCanvas.Ris.Application.Services
{
    public class ModalityPerformedProcedureStepAssembler
    {
        public  ModalityPerformedProcedureStepSummary CreateModalityPerformedProcedureStepSummary(ModalityPerformedProcedureStep mpps, IPersistenceContext context)
        {
            string name = StringUtilities.Combine(mpps.Activities, " / ", delegate(Activity a) { return a.As<ModalityProcedureStep>().Name; });

            ModalityProcedureStepAssembler assembler = new ModalityProcedureStepAssembler();

            // include the details of each MPS in the mpps summary
            List<ModalityProcedureStepSummary> mpsDetails = CollectionUtils.Map<ModalityProcedureStep, ModalityProcedureStepSummary>(
                mpps.Activities,
                delegate(ModalityProcedureStep mps) { return assembler.CreateModalityProcedureStepSummary(mps, context); });

            Dictionary<string, string> extendedProperties = new Dictionary<string, string>();
            foreach (string key in mpps.ExtendedProperties.Keys)
            {
                extendedProperties[key] = mpps.ExtendedProperties[key];
            }


            return new ModalityPerformedProcedureStepSummary(
                mpps.GetRef(),
                name,
                EnumUtils.GetEnumValueInfo(mpps.State, context),
                mpps.StartTime,
                mpps.EndTime,
                "Dummy Performer",
                mpsDetails,
                extendedProperties);
        }
    }
}
