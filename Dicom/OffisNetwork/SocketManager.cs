
namespace ClearCanvas.Dicom.OffisNetwork
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
		/// <remarks>
		/// Internally, WSAStartup is used, each call to which (beyond the first one, of course)
		/// simply increments a reference count, and therefore must be paired with 
		/// a call to <see cref="DeinitializeSockets"/>.
		/// </remarks>
        public static void InitializeSockets()
        {
            OffisDcm.InitializeSockets();
        }

        /// <summary>
        /// Deinitialize the Winsock library in Windows. In
        /// non-Windows platforms, this function does nothing.
        /// </summary>
		/// <remarks>
		/// Internally, WSACleanup is used, each call to which simply decrements a reference count.
		/// Consequently, <see cref="InitializeSockets"/> must be called first, and <see cref="DeinitializeSockets"/>
		/// must be called for each call to <see cref="InitializeSockets"/>.
		/// </remarks>
		public static void DeinitializeSockets()
        {
            OffisDcm.DeinitializeSockets();
        }       
    }
}
