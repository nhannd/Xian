using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare {

    /// <summary>
    /// RequestedProcedureTypeGroupCategory enumeration
    /// </summary>
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
        [EnumValue("Relevence")]
        RELEVENCE,

        /// <summary> 
        /// Scheduled
        /// </summary>
        [EnumValue("Scheduled")]
        SCHEDULED
    }
}