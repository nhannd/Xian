using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;
using ClearCanvas.Common;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Services.Admin.ModalityAdmin
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    public class ModalityAdminService : ApplicationServiceBase, IModalityAdminService
    {
        [ReadOperation]
        public IList<ModalityAdmin> GetAllModalities()
        {
            return this.CurrentContext.GetBroker<IModalityBroker>().FindAll();
        }

        [UpdateOperation]
        public void AddModality(ModalityAdmin modality)
        {
            this.CurrentContext.Lock(modality, DirtyState.New);
        }

        [UpdateOperation]
        public void UpdateModality(ModalityAdmin modality)
        {
            this.CurrentContext.Lock(modality, DirtyState.Dirty);
        }

        [ReadOperation]
        public ModalityAdmin LoadModality(EntityRef modalityRef)
        {
            IModalityBroker modalityBroker = CurrentContext.GetBroker<IModalityBroker>();
            return modalityBroker.Load(modalityRef);
        }
    }
}
