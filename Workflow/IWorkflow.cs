using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Workflow
{
    public interface IWorkflow
    {
        void AddActivity(Activity activity);
    }
}
