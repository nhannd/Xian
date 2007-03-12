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
            ModalitySummary summary = new ModalitySummary();
            summary.ModalityRef = modality.GetRef();
            summary.Active = modality.Active;
            summary.Id = modality.Id;
            summary.Name = modality.Name;
            return summary;
        }

        public ModalityDetail CreateModalityDetail(Modality modality)
        {
            ModalityDetail detail = new ModalityDetail();
            detail.Id = modality.Id;
            detail.Name = modality.Name;
            return detail;
        }

        public void UpdateModality(Modality modality, ModalityDetail detail)
        {
            detail.Id = modality.Id;
            detail.Name = modality.Name;
        }

    }
}
