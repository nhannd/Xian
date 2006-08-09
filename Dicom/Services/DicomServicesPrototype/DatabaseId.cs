using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom.Services
{
    public struct DatabaseId
    {
        public DatabaseId(Int64 newId)
        {
            _internalId = newId;
        }

        Int64 _internalId;
    }
}
