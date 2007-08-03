using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Ris.Client
{
    [ExtensionPoint()]
    public class ViewerIntegrationExtensionPoint : ExtensionPoint<IViewerIntegration>
    {
    }

    public interface IViewerIntegration
    {
        void OpenStudy(string accessionNumber);
    }
}
