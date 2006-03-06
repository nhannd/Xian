
namespace ClearCanvas.Dicom.DataStore
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public struct Url
    {
        public Url(String url)
        {
            // validate the input
            if (null == url)
                throw new System.ArgumentNullException("url", "---");

            if (0 == url.Length)
                throw new System.ArgumentOutOfRangeException("url", "---");

            _url = url;
        }

        public override string ToString()
        {
            return _url;
        }

        private String _url;
    }
}
