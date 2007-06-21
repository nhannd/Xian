using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageServer.Dicom.Offis
{
    internal class OffisDcmItem : IDisposable
    {
        private DcmItem _item;
        private DcmFileFormat _fileFormat;

        internal OffisDcmItem(DcmItem item, DcmFileFormat format)
        {
            _item = item;
            _fileFormat = format;
        }

        internal DcmItem Item
        {
            get { return _item; }
        }

        internal DcmFileFormat FileFormat
        {
            get { return _fileFormat; }
        }


        #region IDisposable Members

        public void Dispose()
        {
            try
            {
                //Dispose(true);
                GC.SuppressFinalize(this);
            }
            catch (Exception)
            {
                // shouldn't throw anything from inside Dispose()
                // TODO: Platform.Log(e);
            }
        }

        #endregion

    }
}
