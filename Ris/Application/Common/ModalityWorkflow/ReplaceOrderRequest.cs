using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;
using PlaceOrderRequest = ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry.PlaceOrderRequest;
using CancelOrderRequest = ClearCanvas.Ris.Application.Common.RegistrationWorkflow.CancelOrderRequest;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow
{
    [DataContract]
    public class ReplaceOrderRequest : DataContractBase
    {
        public ReplaceOrderRequest(
            PlaceOrderRequest placeOrderRequest,
            CancelOrderRequest cancelOrderRequest)
        {
            this.PlaceOrderRequest = placeOrderRequest;
            this.CancelOrderRequest = cancelOrderRequest;
        }

        [DataMember]
        public PlaceOrderRequest PlaceOrderRequest;

        [DataMember]
        public CancelOrderRequest CancelOrderRequest;
    }
}
