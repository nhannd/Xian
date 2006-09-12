using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;

namespace ClearCanvas.Dicom.Services
{
    /// <summary>
    /// Access the underlying DICOM facility to perform sending (C-STORE SCU)
    /// </summary>
    public interface IDicomSender
    {
        void SetSourceApplicationEntity(ApplicationEntity ae);
        void SetDestinationApplicationEntity(ApplicationEntity ae);
        void Send(IEnumerable<string> fileNames, IEnumerable<string> sopClasses, IEnumerable<string> transferSyntaxes);
    }
}
