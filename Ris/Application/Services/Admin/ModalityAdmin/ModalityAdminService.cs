using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.ModalityAdmin;

namespace ClearCanvas.Ris.Application.Services.Admin.ModalityAdmin
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    public class ModalityAdminService : ApplicationServiceBase, IModalityAdminService
    {
        [ReadOperation]
        public ListAllModalitiesResponse ListAllModalities(ListAllModalitiesRequest request)
        {
            ModalityAssembler assembler = new ModalityAssembler();
            return new ListAllModalitiesResponse(
                CollectionUtils.Map<Modality, ModalitySummary, List<ModalitySummary>>(
                    PersistenceContext.GetBroker<IModalityBroker>().FindAll(),
                    delegate(Modality m)
                    {
                        return assembler.CreateModalitySummary(m);
                    }));
        }

        [ReadOperation]
        public LoadModalityForEditResponse LoadModalityForEdit(LoadModalityForEditRequest request)
        {
            // note that the version of the ModalityRef is intentionally ignored here (default behaviour of ReadOperation)
            Modality m = (Modality)PersistenceContext.Load(request.ModalityRef);
            ModalityAssembler assembler = new ModalityAssembler();

            return new LoadModalityForEditResponse(m.GetRef(), assembler.CreateModalityDetail(m));
        }

        [UpdateOperation]
        public AddModalityResponse AddModality(AddModalityRequest request)
        {
            Modality modality = new Modality();
            ModalityAssembler assembler = new ModalityAssembler();
            assembler.UpdateModality(request.ModalityDetail, modality);

            // TODO prior to accepting this add request, we should check that the same modality does not already exist

            PersistenceContext.Lock(modality, DirtyState.New);

            // ensure the new modality is assigned an OID before using it in the return value
            PersistenceContext.SynchState();

            return new AddModalityResponse(assembler.CreateModalitySummary(modality));
        }

        [UpdateOperation]
        public UpdateModalityResponse UpdateModality(UpdateModalityRequest request)
        {
            Modality modality = (Modality)PersistenceContext.Load(request.ModalityRef, EntityLoadFlags.CheckVersion);

            ModalityAssembler assembler = new ModalityAssembler();
            assembler.UpdateModality(request.ModalityDetail, modality);

            // TODO prior to accepting this update request, we should check that the same modality does not already exist

            return new UpdateModalityResponse(assembler.CreateModalitySummary(modality));
        }
    }
}
