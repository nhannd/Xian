using System;
using System.Collections.Generic;
using ClearCanvas.Desktop;
using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client
{
    [ExtensionPoint]
    public class ReportEditorExtensionPoint : ExtensionPoint<IReportEditor>
    {
    }

    /// <summary>
    /// Defines the interface of a report editor.
    /// </summary>
    public interface IReportEditor : IApplicationComponent
    {
        /// <summary>
        /// Sets the reporting worklist item
        /// </summary>
        ReportingWorklistItem WorklistItem { set; }

        /// <summary>
        /// Set the report associated with the worklist item
        /// </summary>
        ReportDetail Report { set; }

        /// <summary>
        /// Gets and sets the report part being edited
        /// </summary>
        ReportPartDetail ReportPart { get; set; }

		/// <summary>
		/// Gets and sets the extended properties associated to the worklist item being edited
		/// </summary>
		Dictionary<string, string> ExtendedProperties { get; set; }

        /// <summary>
        /// Sets the editor mode
        /// </summary>
        bool IsEditingAddendum { set; }

        /// <summary>
        /// Sets the enablement of the Verify operation
        /// </summary>
        bool VerifyEnabled { set; }

        /// <summary>
        /// Sets the enablement of the Send To Verify operation
        /// </summary>
        bool SendToVerifyEnabled { set; }

        /// <summary>
        /// Sets the enablement of the Send To Transcription operation
        /// </summary>
        bool SendToTranscriptionEnabled { set; }

        /// <summary>
        /// Notifies that the editor requests to verify the report
        /// </summary>
        event EventHandler VerifyRequested;

        /// <summary>
        /// Notifies that the editor requests to send the report to be verified
        /// </summary>
        event EventHandler SendToVerifyRequested;

        /// <summary>
        /// Notifies that the editor requests to send the report to transcription
        /// </summary>
        event EventHandler SendToTranscriptionRequested;

        /// <summary>
        /// Notifies that the editor requests to save the report
        /// </summary>
        event EventHandler SaveRequested;

        /// <summary>
        /// Notifies that the editor requests to cancel
        /// </summary>
        event EventHandler CancelRequested;
    }
}
