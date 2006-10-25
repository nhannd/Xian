using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    public interface IDicomServer
    {
        string ServerName
        {
            get;
        }

        string ServerPath
        {
            get;
            set;
        }

        bool IsServer
        {
            get;
        }

        string ServerDetails
        {
            get;
        }

    }
}
