
namespace ClearCanvas.Dicom.DataStore
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public struct ConnectionString
    {
        public ConnectionString(String connectionString)
        {
            // validate the input
            if (null == connectionString)
                throw new System.ArgumentNullException("connectionString", "---");

            if (0 == connectionString.Length)
                throw new System.ArgumentOutOfRangeException("connectionString", "---");

            _connectionString = connectionString;
        }

        public override string ToString()
        {
            return _connectionString;
        }

        private String _connectionString;
    }
}
