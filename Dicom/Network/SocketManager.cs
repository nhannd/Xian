
namespace ClearCanvas.Dicom.Network
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using ClearCanvas.Dicom.OffisWrapper;

    internal class SocketManager
    {
        /// <summary>
        /// Initialize the Winsock library in Windows. In 
        /// non-Windows platforms, this function does nothing via a compiler define.
        /// </summary>
        public static void InitializeSockets()
        {
            OffisDcm.InitializeSockets();
        }

        /// <summary>
        /// Deinitialize the Winsock library in Windows. In
        /// non-Windows platforms, this function does nothing.
        /// </summary>
        public static void DeinitializeSockets()
        {
            OffisDcm.DeinitializeSockets();
        }       
    }
}
