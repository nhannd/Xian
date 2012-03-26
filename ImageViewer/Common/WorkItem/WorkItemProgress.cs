#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.ImageViewer.Common.WorkItem
{
    [DataContract(Namespace = ImageViewerNamespace.Value)]
    [WorkItemProgressDataContract("{b2dcf1f6-6e1a-48cd-b807-b720811a6575}")]
    public class WorkItemProgress : DataContractBase
    {
        [DataMember(IsRequired = false)]
        public string StatusDescription { get; set; }

        [DataMember(IsRequired = true)]
        public Decimal PercentComplete { get; set; }

        [DataMember(IsRequired = true)]
        public bool IsCancelable { get; set; }
    }

    [DataContract(Namespace = ImageViewerNamespace.Value)]
    [WorkItemProgressDataContract("{9745ef5c-766e-4e99-8229-e332acbed2ec}")]
    public class ImportWorkItemProgress : WorkItemProgress
    {
        [DataMember(IsRequired = true)]
        public string Description { get; set; }

        [DataMember(IsRequired = true)]
        public int TotalFilesToImport { get; set; }

        [DataMember(IsRequired = true)]
        public int NumberOfFilesImported { get; set; }
   
        [DataMember(IsRequired = true)]
        public int NumberOfImportFailures { get; set; }

        public int TotalImportsProcessed
        {
            get { return NumberOfFilesImported + NumberOfImportFailures; }
        }

        public bool IsImportComplete()
        {
            return TotalFilesToImport == TotalImportsProcessed;
        }
    }
}
