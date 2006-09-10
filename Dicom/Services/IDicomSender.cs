using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;

namespace ClearCanvas.Dicom.Services
{
    public interface IDicomSender
    {
        void SetSourceApplicationEntity(ApplicationEntity ae);
        void SetDestinationApplicationEntity(ApplicationEntity ae);
        void Send(IEnumerable<string> fileNames, IEnumerable<string> sopClasses, IEnumerable<string> transferSyntaxes);
    }
}
