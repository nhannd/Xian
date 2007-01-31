using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom.Network
{
    public class DicomServerEventArgs : EventArgs
    {
        private IntPtr _interopCallbackInfoPointer;

        public DicomServerEventArgs(IntPtr interopCallbackInfoPointer)
        {
            _interopCallbackInfoPointer = interopCallbackInfoPointer;
        }

        public IntPtr CallbackInfoPointer
        {
            get { return _interopCallbackInfoPointer; }
            set { _interopCallbackInfoPointer = value; }
        }
    }
}
