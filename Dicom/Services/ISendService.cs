using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom;

namespace ClearCanvas.Dicom.Services
{
    /// <summary>
    /// High-level interface to send an object or set of hiearchical objects 
    /// denoted by referencedUid, to the destination specified by destinationAE
    /// </summary>
    public interface ISendService
    {
        void Send(Uid referencedUid, ApplicationEntity destinationAE);
    }
}
