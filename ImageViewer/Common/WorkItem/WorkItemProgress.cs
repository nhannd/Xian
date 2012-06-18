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
    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    [WorkItemProgressDataContract("b2dcf1f6-6e1a-48cd-b807-b720811a6575")]
    [WorkItemKnownType]
    public abstract class WorkItemProgress : DataContractBase
    {
        public virtual string Status { get { return string.Empty; } }

        [DataMember(IsRequired = false)]
        public string StatusDetails { get; set; }

        public virtual Decimal PercentComplete {get { return new decimal(0.0); }}

        public virtual Decimal PercentFailed { get { return new decimal(0.0); } }

        [DataMember(IsRequired = true)]
        public bool IsCancelable { get; set; }
    }

    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    [WorkItemProgressDataContract("d3b84be6-edc7-40e1-911f-f6ec30c2a128")]
    [WorkItemKnownType]
    public class ImportFilesProgress : WorkItemProgress
    {
        public ImportFilesProgress()
        {
            IsCancelable = true;
        }

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

        public override Decimal PercentFailed
        {
            get
            {
                if (NumberOfImportFailures > 0)
                    return (Decimal)NumberOfImportFailures / (NumberOfImportFailures+NumberOfFilesImported);

                return new decimal(0.0);
            }
        }
    }

    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    [WorkItemProgressDataContract("8C994CAB-630E-4a92-AA5B-7BF5D6095D6D")]
    [WorkItemKnownType]
    public class ProcessStudyProgress : WorkItemProgress
    {
        public ProcessStudyProgress()
        {
            IsCancelable = true;
        }

        [DataMember(IsRequired = true)]
        public int TotalFilesToProcess { get; set; }

        [DataMember(IsRequired = true)]
        public int NumberOfFilesProcessed { get; set; }
   
        [DataMember(IsRequired = true)]
        public int NumberOfProcessingFailures { get; set; }

        [DataMember(IsRequired = false)]
        public string OtherFatalFailures { get; set; }

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
                var error = string.Format(SR.StudyProcessProgress_Status, NumberOfFilesProcessed, TotalFilesToProcess, NumberOfProcessingFailures);

                if (string.IsNullOrEmpty(OtherFatalFailures)==false)
                    return string.Format("{0}. {1}", error, OtherFatalFailures);
                return error;
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

        public override Decimal PercentFailed
        {
            get
            {
                if (NumberOfProcessingFailures > 0)
                    return (Decimal) NumberOfProcessingFailures/TotalFilesToProcess;

                return new decimal(0.0);
            }
        }
    }

    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    [WorkItemProgressDataContract("A1F64C74-FAE9-4a4c-A120-E82EE45EA21B")]
    [WorkItemKnownType]
    public class ReindexProgress : WorkItemProgress
    {
        public ReindexProgress()
        {
            IsCancelable = true;
        }

        [DataMember(IsRequired = true)]
        public int StudiesToProcess { get; set; }

        [DataMember(IsRequired = true)]
        public int StudiesProcessed { get; set; }

        [DataMember(IsRequired = true)]
        public int StudyFoldersToProcess { get; set; }

        [DataMember(IsRequired = true)]
        public int StudyFoldersProcessed { get; set; }

        [DataMember(IsRequired = true)]
        public int StudiesDeleted { get; set; }

        [DataMember(IsRequired = true)]
        public int StudiesFailed { get; set; }

        [DataMember(IsRequired = true)]
        public bool Complete { get; set; }

        public override string Status
        {
            get
            {
                if (StudiesDeleted == 0 && StudiesToProcess == 0 && StudyFoldersProcessed == 0 && StudyFoldersToProcess == 0 && StudiesProcessed == 0 && StudiesFailed == 0)
                {
                    return Complete ? SR.ReindexProgress_StatusNoStudies : string.Empty;
                }

                return string.Format(SR.ReindexProgress_Status, StudiesProcessed, StudiesToProcess,
                    StudyFoldersProcessed, StudyFoldersToProcess, StudiesDeleted, StudiesFailed);
            }
        }

        public override Decimal PercentComplete
        {
            get
            {
                if (Complete && StudiesToProcess == 0)
                    return new decimal(100.0);

                if (StudiesToProcess > 0 || StudyFoldersToProcess > 0)
                    return (Decimal)(StudiesProcessed + StudyFoldersProcessed) / (StudiesToProcess+StudyFoldersToProcess);

                return new decimal(0.0);
            }
        }

        public override Decimal PercentFailed
        {
            get
            {
                if ((StudiesToProcess > 0 || StudyFoldersToProcess > 0) && StudiesFailed > 0)
                    return (Decimal)StudiesFailed / (StudiesToProcess + StudyFoldersToProcess);

                return new decimal(0.0);
            }
        }
    }

    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    [WorkItemProgressDataContract("D7040F82-8021-420E-B72B-0890053BE8C5")]
    [WorkItemKnownType]
    public class ReapplyRulesProgress : WorkItemProgress
    {
        public ReapplyRulesProgress()
        {
            IsCancelable = false;
        }

        [DataMember(IsRequired = true)]
        public int StudiesToProcess { get; set; }

        [DataMember(IsRequired = true)]
        public int StudiesProcessed { get; set; }

        [DataMember(IsRequired = true)]
        public bool Complete { get; set; }

        public override string Status
        {
            get
            {
                if (StudiesToProcess == 0 && StudiesProcessed == 0)
                {
                    return Complete ? SR.ReapplyRulesProgress_StatusNoStudies : string.Empty;
                }

                return string.Format(SR.ReapplyRulesProgress_Status, StudiesProcessed, StudiesToProcess);
            }
        }

        public override Decimal PercentComplete
        {
            get
            {
                if (Complete && StudiesToProcess == 0)
                    return new decimal(100.0);

                if (StudiesToProcess > 0)
                    return (Decimal)StudiesProcessed / StudiesToProcess ;

                return new decimal(0.0);
            }
        }
    }

    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    [WorkItemProgressDataContract("68BCA074-F1F1-4870-8ABD-281B31E10B6A")]
    [WorkItemKnownType]
    public class DicomSendProgress : WorkItemProgress
    {
        public DicomSendProgress()
        {
            IsCancelable = true;
        }

        [DataMember(IsRequired = true)]
        public int ImagesToSend { get; set; }

        [DataMember(IsRequired = true)]
        public int WarningSubOperations { get; set; }

        [DataMember(IsRequired = true)]
        public int FailureSubOperations { get; set; }

        [DataMember(IsRequired = true)]
        public int SuccessSubOperations { get; set; }

        public int RemainingSubOperations
        {
            get { return ImagesToSend - (WarningSubOperations + FailureSubOperations + SuccessSubOperations); }
        }

        public override string Status
        {
            get
            {
                if (ImagesToSend == 0)
                    return string.Empty;

                return string.Format(SR.DicomSendProgress_Status,
                                     SuccessSubOperations + WarningSubOperations, FailureSubOperations,
                                     RemainingSubOperations);
            }
        }

        public override Decimal PercentComplete
        {
            get
            {
                if (ImagesToSend > 0)
                    return (Decimal) (WarningSubOperations + FailureSubOperations + SuccessSubOperations)/ImagesToSend;

                return new decimal(0.0);
            }
        }

        public override Decimal PercentFailed
        {
            get
            {
                if (ImagesToSend > 0 && FailureSubOperations > 0)
                    return (Decimal)FailureSubOperations / ImagesToSend;

                return new decimal(0.0);
            }
        }
    }

    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    [WorkItemProgressDataContract("24DB4BC0-2759-468E-802B-07C54F91A68D")]
    [WorkItemKnownType]
    public class DicomRetrieveProgress : WorkItemProgress
    {
        public DicomRetrieveProgress()
        {
            IsCancelable = true;
        }

        [DataMember(IsRequired = true)]
        public int ImagesToRetrieve { get; set; }

        [DataMember(IsRequired = true)]
        public int WarningSubOperations { get; set; }

        [DataMember(IsRequired = true)]
        public int FailureSubOperations { get; set; }

        [DataMember(IsRequired = true)]
        public int SuccessSubOperations { get; set; }

        public int RemainingSubOperations
        {
            get { return ImagesToRetrieve - (WarningSubOperations + FailureSubOperations + SuccessSubOperations); }
        }

        public override string Status
        {
            get
            {
                if (ImagesToRetrieve == 0)
                    return string.Empty;

                return string.Format(SR.DicomRetrieveProgress_Status,
                                     SuccessSubOperations + WarningSubOperations, FailureSubOperations,
                                     RemainingSubOperations);
            }
        }

        public override Decimal PercentComplete
        {
            get
            {
                if (ImagesToRetrieve > 0)
                    return (Decimal)(WarningSubOperations + FailureSubOperations + SuccessSubOperations) / ImagesToRetrieve;

                return new decimal(0.0);
            }
        }

        public override Decimal PercentFailed
        {
            get
            {
                if (ImagesToRetrieve > 0 && FailureSubOperations > 0)
                    return (Decimal)FailureSubOperations / ImagesToRetrieve;

                return new decimal(0.0);
            }
        }
    }

    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    [WorkItemProgressDataContract("179B5CD7-8C67-44C1-8211-5B800FE069C4")]
    [WorkItemKnownType]
    public class DeleteProgress : WorkItemProgress
    {
        public DeleteProgress()
        {
            IsCancelable = true;
        }

        [DataMember(IsRequired = true)]
        public int ImagesToDelete { get; set; }

        [DataMember(IsRequired = true)]
        public int ImagesDeleted { get; set; }

        public override string Status
        {
            get
            {
                if (ImagesToDelete == 0)
                    return string.Empty;

                if (ImagesDeleted == 1)
                    return string.Format(SR.DeleteProgress_Status,ImagesDeleted);

                return string.Format(SR.DeleteProgressPlural_Status, ImagesDeleted);
            }
        }

        public override Decimal PercentComplete
        {
            get
            {
                if (ImagesToDelete > 0)
                    return (Decimal)(ImagesDeleted) / ImagesToDelete;

                return new decimal(0.0);
            }
        }
    }
}
