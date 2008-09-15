using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Client.Workflow
{
	public class ProtocollingOrderDetailViewComponent : OrderDetailViewComponent
	{
        // Internal data contract used for jscript deserialization
        [DataContract]
        public class PatientOrderContext : DataContractBase
        {
            public PatientOrderContext(EntityRef patientRef, EntityRef orderRef)
            {
                this.PatientRef = patientRef;
                this.OrderRef = orderRef;
            }

            [DataMember]
            public EntityRef PatientRef;

            [DataMember]
            public EntityRef OrderRef;
        }

        public ProtocollingOrderDetailViewComponent()
		{
		}

        public ProtocollingOrderDetailViewComponent(EntityRef patientRef, EntityRef orderRef)
		{
            _context = new PatientOrderContext(patientRef, orderRef);
        }

        protected override string PageUrl
		{
			get { return WebResourcesSettings.Default.ProtocollingOrderDetailPageUrl; }
		}
	}
}
