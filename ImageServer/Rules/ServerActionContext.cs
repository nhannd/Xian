using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageServer.Rules
{
    public class ServerActionContext
    {
        #region Private Members

        private readonly DicomMessageBase _msg;
        #endregion

        #region Constructors
        public ServerActionContext(DicomMessageBase msg)
        {
            _msg = msg;
        }
        #endregion

        #region Public Properties
        public DicomMessageBase Message
        {
            get { return _msg; }
        }
        #endregion
    }
}
