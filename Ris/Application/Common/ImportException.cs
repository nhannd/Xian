using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common
{
    [Serializable]
    public class ImportException : Exception
    {
        private int _dataRow;

        public ImportException(string message)
            :base(message)
        {
            _dataRow = -1;
        }

        public ImportException(string message, int dataRow)
            :base(message)
        {
            _dataRow = dataRow;
        }

        public ImportException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Gets the row at which the exception occured, or -1 if not applicable.
        /// </summary>
        public int DataRow
        {
            get { return _dataRow; }
        }
    }
}
