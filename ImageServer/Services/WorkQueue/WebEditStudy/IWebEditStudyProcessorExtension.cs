using System;
using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Services.WorkQueue.WebEditStudy
{
    /// <summary>
    /// Defines the interface of a extension to <see cref="WebEditStudyItemProcessor"/>
    /// </summary>
    public interface IWebEditStudyProcessorExtension : IDisposable
    {
        /// <summary>
        /// Gets a value indicating whether the extension is enabled.
        /// </summary>
        bool Enabled { get; }

        /// <summary>
        /// Initializes the extension.
        /// </summary>
        /// <param name="workQueueProcessor"></param>
        void Initialize(WebEditStudyItemProcessor workQueueProcessor);

        /// <summary>
        /// Called when study is about to be updated.
        /// </summary>
        /// <param name="context"></param>
        void OnStudyEditing(WebEditStudyContext context);

        /// <summary>
        /// Called after the study has been updated.
        /// </summary>
        /// <param name="context"></param>
        void OnStudyEdited(WebEditStudyContext context);
    }

    class WebEditStudyProcessorExtensionPoint:ExtensionPoint<IWebEditStudyProcessorExtension>
    {}
}
