using System;

namespace ClearCanvas.ImageServer.Common.Data
{
    /// <summary>
    /// Represents action used for study reconciliation
    /// </summary>
    public enum StudyReconcileAction
    {
        [EnumInfoAttribute(ShortDescription = "Discard", LongDescription = "Discarded conflicting images")]
        Discard,

        [EnumInfoAttribute(ShortDescription = "Merge", LongDescription = "Merged study and conflicting images")]
        Merge,

        [EnumInfoAttribute(ShortDescription = "Split Studies", LongDescription = "Created new study from conflicting images")]
        CreateNewStudy,

        [EnumInfoAttribute(ShortDescription = "Process As Is", LongDescription = "Processed the images normally")]
        ProcessAsIs
    }
}