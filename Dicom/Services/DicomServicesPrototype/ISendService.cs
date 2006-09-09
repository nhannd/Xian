using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom;

namespace ClearCanvas.Dicom.Services
{
    public interface ISendService
    {
        void Send(Uid referencedUid, ApplicationEntity destinationAE);
    }
}
