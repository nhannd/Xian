using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare {

    /// <summary>
    /// RequestedProcedureTypeGroupCategory enumeration
    /// </summary>
    [EnumValueClass(typeof(RequestedProcedureTypeGroupCategoryEnum))]
    public enum RequestedProcedureTypeGroupCategory
	{
        /// <summary> 
        /// Reading
        /// </summary>
        [EnumValue("Reading")]
        READING,

        /// <summary> 
        /// Relevence
        /// </summary>
        [EnumValue("Relevance")]
        RELEVANCE,

        /// <summary> 
        /// Scheduled
        /// </summary>
        [EnumValue("Modality")]
        MODALITY
    }
}