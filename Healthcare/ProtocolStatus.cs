using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare {

    /// <summary>
    /// ProtocolStatus enumeration
    /// </summary>
    [EnumValueClass(typeof(ProtocolStatusEnum))]
	public enum ProtocolStatus
	{
        /// <summary>
        /// Pending
        /// </summary>
        [EnumValue("Pending", Description = "Protocol is pending")]
        PN,

        /// <summary>
        /// Protocolled
        /// </summary>
        [EnumValue("Protocolled", Description = "Protocol assigned and order accepted")]
        PR,

        /// <summary>
        /// Protocolled
        /// </summary>
        [EnumValue("Rejected", Description = "Protocol assigned and order rejected")]
        RJ,

        /// <summary>
        /// Protocolled
        /// </summary>
        [EnumValue("Suspended", Description = "Protocol suspended pending further order information")]
        SU,

        /// <summary>
        /// Awaiting Approval
        /// </summary>
        [EnumValue("Awaiting Approval", Description = "Protocol submitted for approval by resident")]
        AA
    }
}