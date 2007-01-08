using System;
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;
using ClearCanvas.Workflow;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// Order entity
    /// </summary>
	public partial class Order : Entity
	{
        public static Order NewOrder(
            string accessionNumber,
            Patient patient,
            Visit visit,
            DiagnosticService diagnosticService,
            DateTime schedulingRequestDateTime,
            Practitioner orderingPhysician,
            Facility orderingFacility,
            OrderPriority priority)
        {
            // create the basic order
            Order order = new Order(
                                patient,
                                visit,
                                accessionNumber,
                                diagnosticService,
                                Platform.Time,
                                schedulingRequestDateTime,
                                orderingPhysician,
                                orderingFacility,
                                priority);

            // add requested procedures according to the diagnostic service breakdown
            int rpIndex = 0;
            foreach (RequestedProcedureType rpt in diagnosticService.RequestedProcedureTypes)
            {
                RequestedProcedure rp = new RequestedProcedure(order, rpt, string.Format("{0}", ++rpIndex));
                order.RequestedProcedures.Add(rp);

                // add scheduled procedure steps according to the diagnostic service breakdown
                foreach (ModalityProcedureStepType spt in rpt.ModalityProcedureStepTypes)
                {
                    ModalityProcedureStep sps = new ModalityProcedureStep(rp, spt, spt.DefaultModality);
                    rp.ModalityProcedureSteps.Add(sps);
                }
            }

            return order;
        }


	
		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}
		
		#region Object overrides
		
		public override bool Equals(object that)
		{
            Order other = that as Order;
			return other != null && other.AccessionNumber == this.AccessionNumber;
		}
		
		public override int GetHashCode()
		{
            return this.AccessionNumber.GetHashCode();
		}
		
		#endregion

	}
}