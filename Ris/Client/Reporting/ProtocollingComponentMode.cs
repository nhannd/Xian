namespace ClearCanvas.Ris.Client.Reporting
{
    /// <summary>
    /// Dictates the mode the mode of operation <see cref="ProtocollingComponent"/>.  The mode impacts availability of "Protocol Next Order" checkbox
    /// and indicates if the worklist item needs to be claimed
    /// </summary>
    public enum ProtocollingComponentMode
    {
        /// <summary>
        /// "Protocol Next Order" checkbox enabled.  Worklist item attempted to be claimed.
        /// </summary>
        Assign,

        /// <summary>
        /// "Protocol Next Order" checkbox disabled.  Worklist item not claimed.
        /// </summary>
        Edit,

        /// <summary>
        /// Read-only: "Protocol Next Order" checkbox disabled.  Worklist item not claimed.
        /// </summary>
        Review
    }
}