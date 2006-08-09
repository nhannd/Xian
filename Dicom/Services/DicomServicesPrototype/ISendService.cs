namespace ClearCanvas.Dicom.Services
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using ClearCanvas.Dicom.Network;
    using ClearCanvas.Dicom;

    interface ISendService
    {
        void Send(Uid referencedUid, ApplicationEntity destinationAE);
    }
}
