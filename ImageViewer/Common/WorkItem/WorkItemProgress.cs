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
using System.Xml.Serialization;
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.ImageViewer.Common.WorkItem
{
    [XmlInclude(typeof(ImportFilesProgress))]
    [XmlInclude(typeof(StudyProcessProgress))]
    [DataContract(Namespace = ImageViewerNamespace.Value)]
    [WorkItemProgressDataContract("b2dcf1f6-6e1a-48cd-b807-b720811a6575")]
    public abstract class WorkItemProgress : DataContractBase
    {
        public virtual string Status { get { return string.Empty; } }

        [DataMember(IsRequired = false)]
        public string StatusDetails { get; set; }

        public virtual Decimal PercentComplete {get { return new decimal(0.0); }}

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

        [DataMember(IsRequired = true)]
        public int PathsToImport { get; set; }

        [DataMember(IsRequired = true)]
        public int PathsImported { get; set; }


        public int TotalImportsProcessed
        {
            get { return NumberOfFilesImported + NumberOfImportFailures; }
        }

        public bool IsImportComplete()
        {
            return TotalFilesToImport == TotalImportsProcessed;
        }

        public override string Status
        {
            get
            {
                return string.Format(SR.ImportFilesProgress_Status, NumberOfFilesImported, TotalFilesToImport,
                                   NumberOfImportFailures);
            }
        }

        public override Decimal PercentComplete
        {
            get
            {
                if (PathsToImport > 0)
                    return (Decimal)PathsImported / PathsToImport;

                return new decimal(0.0);
            }
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

        public override string Status
        {
            get
            {
                return string.Format(SR.StudyProcessProgress_Status, NumberOfFilesProcessed, TotalFilesToProcess,
                                   NumberOfProcessingFailures);                
            }
        }
        public override Decimal PercentComplete
        {
            get
            {
                if (TotalFilesToProcess > 0)
                    return (Decimal)TotalFilesProcessed / TotalFilesToProcess;

                return new decimal(0.0);
            }
        }
    }

    [DataContract(Namespace = ImageViewerNamespace.Value)]
    [WorkItemProgressDataContract("A1F64C74-FAE9-4a4c-A120-E82EE45EA21B")]
    public class ReindexProgress : WorkItemProgress
    {
        [DataMember(IsRequired = true)]
        public int NumberOfStudiesToProcess { get; set; }

        [DataMember(IsRequired = true)]
        public int StudiesProcessed { get; set; }

        [DataMember(IsRequired = true)]
        public int StudyFoldersProcessed { get; set; }

        [DataMember(IsRequired = true)]
        public int NumberOfStudiesDeleted { get; set; }

        public override string Status
        {
            get
            {
                if (NumberOfStudiesDeleted == 0 && NumberOfStudiesToProcess == 0 && StudyFoldersProcessed == 0 && StudiesProcessed == 0)
                    return SR.Progress_Pending;

                return string.Format(SR.ReindexProgress_Status, StudiesProcessed, NumberOfStudiesToProcess,
                    NumberOfStudiesDeleted, StudyFoldersProcessed);
            }
        }

        public override Decimal PercentComplete
        {
            get
            {
                if (NumberOfStudiesToProcess > 0)
                    return (Decimal)StudiesProcessed / NumberOfStudiesToProcess;

                return new decimal(0.0);
            }
        }
    }
}
