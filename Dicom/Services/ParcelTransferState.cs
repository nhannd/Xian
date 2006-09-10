using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom.Services
{
    /// <summary>
    /// State table:
    /// <table>
    /// <tr><td>State i</td><td>Event</td><td>State i+1</td></tr>
    /// <tr><td>Pending</td><td>Start</td><td>InProgress</td></tr>
    /// <tr><td>Pending</td><td>Cancel</td><td>CancelRequested</td></tr>
    /// <tr><td>Pending</td><td>External Error</td><td>Error</td></tr>
    /// <tr><td>InProgress</td><td>Cancel</td><td>CancelRequested</td></tr>
    /// <tr><td>InProgress</td><td>Pause</td><td>PauseRequested</td></tr>
    /// <tr><td>InProgress</td><td>External Error</td><td>Error</td></tr>
    /// <tr><td>InProgress</td><td>Completion</td><td>Completed</td></tr>
    /// <tr><td>CancelRequesed</td><td>Iteration</td><td>Cancelled</td></tr>
    /// <tr><td>Error</td><td>Restart</td><td>Pending</td></tr>
    /// <tr><td>PauseRequested</td><td>Iteration</td><td>Paused</td></tr>
    /// </table>
    /// </summary>
    public enum ParcelTransferState
    {
        Unknown = 0,
        Pending,
        InProgress,
        CancelRequested,
        Cancelled,
        PauseRequested,
        Paused,
        Completed,
        Error
    }
}
