namespace ClearCanvas.Dicom.Network
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class DicomServer
    {
        public DicomServer(ApplicationEntity ownAEParameters, ListeningPort port)
        {
            SocketManager.InitializeSockets();
            _myOwnAE = ownAEParameters;
        }

        ~DicomServer()
        {
            SocketManager.DeinitializeSockets();
        }

        #region Private members
        private ApplicationEntity _myOwnAE;
        #endregion
    }
}
