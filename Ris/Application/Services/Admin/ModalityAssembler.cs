using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Ris.Application.Common.Admin;

namespace ClearCanvas.Ris.Application.Services.Admin
{
    public class ModalityAssembler
    {
        public ModalitySummary CreateModalitySummary(Modality modality)
        {
            return new ModalitySummary(
                modality.GetRef(),
                modality.Id,
                modality.Name);
        }

        public ModalityDetail CreateModalityDetail(Modality modality)
        {
            return new ModalityDetail(
                modality.Id,
                modality.Name);
        }

        public void UpdateModality(ModalityDetail detail, Modality modality)
        {
            modality.Id = detail.Id;
            modality.Name = detail.Name;
        }

    }
}
