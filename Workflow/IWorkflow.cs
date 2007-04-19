using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Workflow
{
    public interface IWorkflow
    {
        void AddActivity(Activity activity);
        IPersistenceContext CurrentContext { get; }
    }
}
