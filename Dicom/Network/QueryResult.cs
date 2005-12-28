using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom.Network
{
    public class QueryResult
    {
        public Uid StudyInstanceUid
        {
            get
            {
                return new Uid("1.2.840.1.2.311432.43242.266");
            }
            private set
            {
            }
        }
    }
}
