using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom.Network
{
    public struct HostName
    {
        private string _hostName;

        public HostName(string hostName)
        {
            // validate the input
            if (null == hostName)
                throw new System.ArgumentNullException("hostName", "The hostname cannot be set to null");

            if (0 == hostName.Length)
                throw new System.ArgumentOutOfRangeException("hostName", "The hostname cannot be zero-length");

            // todo: if input is an ip address, ensure that the format is correct

            // todo: if input is an alphanumeric name, ensure that the format is correct

            // validation complete
            _hostName = hostName;
        }

        public override string ToString()
        {
            return _hostName;
        }
    }
}
