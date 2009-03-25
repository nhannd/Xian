using System;

namespace ClearCanvas.ImageServer.Common.Data
{
    /// <summary>
    /// Represents action used for study reconciliation
    /// </summary>
    public enum StudyReconcileAction
    {
        [EnumInfoAttribute(ShortDescription = "Discard", LongDescription = "Discard conflicting images")]
        Discard,

        [EnumInfoAttribute(ShortDescription = "Merge", LongDescription = "Merge conflicting images into study")]
        Merge,

        [EnumInfoAttribute(ShortDescription = "Split Studies", LongDescription = "Create new study from conflicting images")]
        CreateNewStudy,

        [EnumInfoAttribute(ShortDescription = "Process As Is", LongDescription = "Ignore all differences and process the images normally")]
        ProcessAsIs
    }
}