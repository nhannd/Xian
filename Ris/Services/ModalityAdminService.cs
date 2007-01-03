using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;
using ClearCanvas.Common;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Ris.Services
{
    [ExtensionOf(typeof(ClearCanvas.Enterprise.ServiceLayerExtensionPoint))]
    public class ModalityAdminService : HealthcareServiceLayer, IModalityAdminService
    {
        [ReadOperation]
        public IList<Modality> GetAllModalities()
        {
            return this.CurrentContext.GetBroker<IModalityBroker>().FindAll();
        }

        [UpdateOperation]
        public void AddModality(Modality modality)
        {
            this.CurrentContext.Lock(modality, DirtyState.New);
        }

        [UpdateOperation]
        public void UpdateModality(Modality modality)
        {
            this.CurrentContext.Lock(modality, DirtyState.Dirty);
        }

        [ReadOperation]
        public Modality LoadModality(EntityRef<Modality> modalityRef)
        {
            IModalityBroker modalityBroker = CurrentContext.GetBroker<IModalityBroker>();
            return modalityBroker.Load(modalityRef);
        }
    }
}
