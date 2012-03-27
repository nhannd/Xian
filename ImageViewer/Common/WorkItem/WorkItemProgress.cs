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
    [WorkItemProgressDataContract("b2dcf1f6-6e1a-48cd-b807-b720811a6575")]
    public class WorkItemProgress : DataContractBase
    {
        [DataMember(IsRequired = false)]
        public string Status { get; set; }

        [DataMember(IsRequired = false)]
        public string StatusDetails { get; set; }

        [DataMember(IsRequired = true)]
        public Decimal PercentComplete { get; set; }

        [DataMember(IsRequired = true)]
        public bool IsCancelable { get; set; }
    }

    [DataContract(Namespace = ImageViewerNamespace.Value)]
    [WorkItemProgressDataContract("d3b84be6-edc7-40e1-911f-f6ec30c2a128")]
    public class ImportFilesProgress : WorkItemProgress
    {
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

        public void UpdateStatus()
        {
            if (TotalFilesToImport > 0)
                PercentComplete = (Decimal)TotalImportsProcessed / TotalFilesToImport;
            else
                PercentComplete = new decimal(0.0);

            Status = string.Format(SR.ImportFilesProgress_Status, NumberOfFilesImported, TotalFilesToImport,
                                   NumberOfImportFailures);
        }
    }

    [DataContract(Namespace = ImageViewerNamespace.Value)]
    [WorkItemProgressDataContract("8C994CAB-630E-4a92-AA5B-7BF5D6095D6D")]
    public class StudyProcessProgress : WorkItemProgress
    {
        [DataMember(IsRequired = true)]
        public int TotalFilesToProcess { get; set; }

        [DataMember(IsRequired = true)]
        public int NumberOfFilesProcessed { get; set; }
   
        [DataMember(IsRequired = true)]
        public int NumberOfProcessingFailures { get; set; }

        public int TotalFilesProcessed
        {
            get { return NumberOfFilesProcessed + NumberOfProcessingFailures; }
        }

        public bool IsImportComplete()
        {
            return TotalFilesToProcess == TotalFilesProcessed;
        }

        public void UpdateStatus()
        {
            if (TotalFilesToProcess > 0)
                PercentComplete = (Decimal)TotalFilesProcessed/TotalFilesToProcess;
            else            
                PercentComplete = new decimal(0.0);

            Status = string.Format(SR.StudyProcessProgress_Status, NumberOfFilesProcessed, TotalFilesToProcess,
                                   NumberOfProcessingFailures);
        }
    }
}
