using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom.Network
{
    public struct ListeningPort
    {
        private System.Int32 _listeningPort;

        public ListeningPort(int listeningPort)
        {
            // validate the input
            if (listeningPort < 1 || listeningPort > System.Int32.MaxValue)
                throw new System.ArgumentOutOfRangeException("listeningPort", SR.ExceptionDicomListeningPortOutOfRange);

            _listeningPort = listeningPort;
        }

        public System.Int32 ToInt32()
        {
            return _listeningPort;
        }
    }
}
