using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom.Network
{
    /// <summary>
    /// Encapsulation of a listening port number.
    /// </summary>
    [SerializableAttribute]
    public struct ListeningPort
    {
        /// <summary>
        /// Mandatory constructor.
        /// </summary>
        /// <param name="listeningPort">The listening port as a 32-bit integer.</param>
        public ListeningPort(Int32 listeningPort)
        {
            // validate the input
            if (listeningPort < 1 || listeningPort > System.Int32.MaxValue)
                throw new System.ArgumentOutOfRangeException("listeningPort", SR.ExceptionDicomListeningPortOutOfRange);

            _listeningPort = listeningPort;
        }

        /// <summary>
        /// Gets the listening port as a 32-bit integer.
        /// </summary>
        /// <returns>The listening port as a 32-bit integer.</returns>
        public Int32 ToInt32()
        {
            return _listeningPort;
        }

        /// <summary>
        /// Implicit cast to return the listen port as an integer for ease of use.
        /// </summary>
        /// <param name="lp">The ListeningPort object to be casted.</param>
        /// <returns>An integer representing the listening port.</returns>
        public static implicit operator Int32(ListeningPort lp)
        {
            return lp._listeningPort;
        }

        private System.Int32 _listeningPort;
    }
}
