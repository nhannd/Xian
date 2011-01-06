#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common.AttachedDocumentService;

namespace ClearCanvas.Ris.Application.Services.AttachedDocumentService
{
    [ServiceImplementsContract(typeof(IAttachedDocumentService))]
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    public class AttachedDocumentService : ApplicationServiceBase, IAttachedDocumentService
    {
        [ReadOperation]
        public GetAttachedDocumentFormDataResponse GetAttachedDocumentFormData(GetAttachedDocumentFormDataRequest request)
        {
            return new GetAttachedDocumentFormDataResponse(
                EnumUtils.GetEnumValueList<PatientAttachmentCategoryEnum>(PersistenceContext),
                EnumUtils.GetEnumValueList<OrderAttachmentCategoryEnum>(PersistenceContext));
        }
    }
}
