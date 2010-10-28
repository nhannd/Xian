#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.Import
{
    [RisApplicationService]
    [ServiceContract]
    public interface IImportService
    {
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        ListImportersResponse ListImporters(ListImportersRequest request);

        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        [FaultContract(typeof(ImportException))]
        ImportCsvResponse ImportCsv(ImportCsvRequest request);
    }
}
