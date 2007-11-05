using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services.ProtocollingWorkflow
{
    internal class ProtocollingWorkflowAssembler
    {
        public ProtocolCodeDetail CreateProtocolCodeDetail(ProtocolCode pc)
        {
            ProtocolCodeDetail detail = new ProtocolCodeDetail(pc.Name, pc.Description);
            return detail;
        }

        public ProtocolDetail CreateProtocolDetail(Protocol protocol)
        {
            return new ProtocolDetail();
        }
    }
}