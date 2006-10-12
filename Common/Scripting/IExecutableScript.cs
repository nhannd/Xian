using System;
using System.Collections;
using System.Text;

namespace ClearCanvas.Common.Scripting
{
    public interface IExecutableScript
    {
        object Run(IDictionary context);
    }
}
