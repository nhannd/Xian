using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Rules;

namespace ClearCanvas.ImageServer.Services.WorkQueue.StudyProcess
{
    /// <summary>
    /// Defines the interface of the component that can be attached to a <see cref="StudyProcessItemProcessor"/>.
    /// </summary>
    /// <remarks>
    /// </remarks>
    public interface IStudyProcessItemProcessorListener:IWorkQueueProcessorListener<StudyProcessItemProcessor>
    {
       
    }
}
