using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Desktop
{
    public enum ApplicationComponentExitCode
    {
        Normal,
        Cancelled,
    }

    public interface IApplicationComponent : INotifyPropertyChanged
    {
        void SetHost(IApplicationComponentHost host);
        IToolSet ToolSet { get; }

        void Start();
        void Stop();

        bool CanExit();
        ApplicationComponentExitCode ExitCode { get; }
    }
}
