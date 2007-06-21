using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageServer.Dicom
{
    public class ApplicationEntity
    {
        #region Member Variables
        
        private uint _maxPduLength = (1500 - 6) * 64;

        private String _name = "AE_TITLE";

        private int _sendBufferSize = 128 * 1024;
        private int _receiveBufferSize = 128 * 1024;
        private int _readTimeout = 30;
        private int _writeTimeout = 30;

        #endregion

        #region Properties
        /// <summary>
        /// The Maximum PDU length accepted by this Application.
        /// </summary>
        public uint MaximumPduLength
        {
            get
            {
                return _maxPduLength;
            }
            set
            {
                _maxPduLength = value;
            }
        }

        /// <summary>
        /// The Application Entity Title.
        /// </summary>
        public String Name
        {
            get
            {
                return _name;
            }
        }

        /// <summary>
        /// The network Send Buffer size utilized by this application.
        /// </summary>
        public int SendBufferSize
        {
            get
            {
                return _sendBufferSize;
            }
            set
            {
                _sendBufferSize = value;
            }
        }

        /// <summary>
        /// The network Receive Buffer size utilized by this application.
        /// </summary>
        public int ReceiveBufferSize
        {
            get
            {
                return _receiveBufferSize;
            }
            set
            {
                _receiveBufferSize = value;
            }
        }

        /// <summary>
        /// The timeout for any network Read operations.
        /// </summary>
        public int ReadTimeout
        {
            get
            {
                return _readTimeout;
            }
            set
            {
                _readTimeout = value;
            }
        }

        /// <summary>
        /// The timeout for any network write operations.
        /// </summary>
        public int WriteTimeout
        {
            get
            {
                return _writeTimeout;
            }
            set
            {
                _writeTimeout = value;
            }
        }

#endregion

        #region Constructors
        public ApplicationEntity(String ae)
        {
            _name = ae;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Retrieves the AE Title.
        /// </summary>
        /// <returns>The AE Title as a string.</returns>
        public override string ToString()
        {
            return _name;
        }

        /// <summary>
        /// Implicit cast to a String object, for ease of use.
        /// </summary>
        /// <param name="aet">The AETitle object to be casted.</param>
        /// <returns>A String representation of the AE Title object.</returns>
        public static implicit operator String(ApplicationEntity aet)
        {
            return aet.ToString();
        }

        #endregion
    }
}
