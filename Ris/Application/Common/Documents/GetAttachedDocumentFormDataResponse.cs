#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Common.Documents
{
    [DataContract]
    public class GetAttachedDocumentFormDataResponse : DataContractBase
    {
        public GetAttachedDocumentFormDataResponse(
            List<EnumValueInfo> patientAttachmentCategoryChoices,
            List<EnumValueInfo> orderAttachmentCategoryChoices)
        {
            this.PatientAttachmentCategoryChoices = patientAttachmentCategoryChoices;
            this.OrderAttachmentCategoryChoices = orderAttachmentCategoryChoices;
        }

        [DataMember]
        public List<EnumValueInfo> PatientAttachmentCategoryChoices;

        [DataMember]
        public List<EnumValueInfo> OrderAttachmentCategoryChoices;
    }
}
