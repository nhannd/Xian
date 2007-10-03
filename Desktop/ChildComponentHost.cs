using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    public class ChildComponentHost : ApplicationComponentHost
    {
        private IApplicationComponentHost _parentHost;

        public ChildComponentHost(IApplicationComponentHost parentHost, IApplicationComponent childComponent)
            : base(childComponent)
        {
            Platform.CheckForNullReference(parentHost, "parentHost");

            _parentHost = parentHost;
        }

        public override DesktopWindow DesktopWindow
        {
            get { return _parentHost.DesktopWindow; }
        }

        public override string Title
        {
            get { return _parentHost.Title; }
        }

    }
}
