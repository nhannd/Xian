using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom.Network
{
    /// <summary>
    /// Encapsulation of a hostname.
    /// </summary>
    [SerializableAttribute]
    public struct HostName
    {
        /// <summary>
        /// Mandatory constructor.
        /// </summary>
        /// <param name="hostName">The Hostname as a string.</param>
        public HostName(string hostName)
        {
            // validate the input
            if (null == hostName)
                throw new System.ArgumentNullException("hostName", SR.ExceptionDicomHostnameNull);

            if (0 == hostName.Length)
                throw new System.ArgumentOutOfRangeException("hostName", SR.ExceptionDicomHostnameZeroLength);

            // todo: if input is an ip address, ensure that the format is correct

            // todo: if input is an alphanumeric name, ensure that the format is correct

            // validation complete
            _hostName = hostName;
        }

        /// <summary>
        /// Gets the Hostname as a string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _hostName;
        }

        public String StringForm
        {
            get { return this.ToString(); }
        }

        private string _hostName;
    }
}
