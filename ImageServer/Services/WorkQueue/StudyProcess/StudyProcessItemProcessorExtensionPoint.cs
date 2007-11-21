using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Services.WorkQueue.StudyProcess
{
    [ExtensionPoint()]
    public class StudyProcessItemProcessorExtensionPoint:ExtensionPoint<IStudyProcessItemProcessorListener>
    {
    }
}
