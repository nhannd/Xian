using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation;

namespace ClearCanvas.Ris.Application.Services
{
    public class ModalityPerformedProcedureStepAssembler
    {
        public  ModalityPerformedProcedureStepSummary CreateModalityPerformedProcedureStepSummary(ModalityPerformedProcedureStep mpps, IPersistenceContext context)
        {
            StringBuilder nameBuilder = new StringBuilder();
            int mpsCount = mpps.Activities.Count;
            foreach (ModalityProcedureStep mps in mpps.Activities)
            {
                nameBuilder.Append(mps.Name);
                if (1 < mpsCount--) nameBuilder.Append(" / ");
            }

            ModalityProcedureStepAssembler assembler = new ModalityProcedureStepAssembler();

            // include the details of each MPS in the mpps summary
            List<ModalityProcedureStepDetail> mpsDetails = CollectionUtils.Map<ModalityProcedureStep, ModalityProcedureStepDetail>(
                mpps.Activities,
                delegate(ModalityProcedureStep mps) { return assembler.CreateModalityProcedureStepDetail(mps, context); });

            Dictionary<string, string> extendedProperties = new Dictionary<string, string>();
            foreach (string key in mpps.ExtendedProperties.Keys)
            {
                extendedProperties[key] = mpps.ExtendedProperties[key];
            }


            return new ModalityPerformedProcedureStepSummary(
                mpps.GetRef(),
                nameBuilder.ToString(),
                EnumUtils.GetEnumValueInfo(mpps.State, context),
                mpps.StartTime,
                mpps.EndTime,
                "Dummy Performer",
                mpsDetails,
                extendedProperties);
        }
    }
}
