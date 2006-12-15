using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom.DataStore
{
    public class DicomUri
    {
        #region NHibernate-specific members
        public DicomUri()
        {
        }

        public virtual string InternalUri
        {
            get 
            {
                if (null != this.InternalUriObject)
                    return this.InternalUriObject.AbsoluteUri;
                else
                    return null;
            }
            set 
            {
                this.InternalUriObject = new Uri(value); 
            }
        }
        #endregion

        public DicomUri(string uri)
        {
            // validate the input
            if (null == uri)
				throw new System.ArgumentNullException("uri", SR.ExceptionUriCannotBeNullOrEmpty);

            if (0 == uri.Length)
				throw new System.ArgumentOutOfRangeException("uri", SR.ExceptionUriCannotBeNullOrEmpty);

            this.InternalUriObject = new Uri(uri);
        }

        public DicomUri(Uri newUri)
        {
            this.InternalUriObject = newUri;
        }

        public bool IsFile
        {
            get 
            {
                if (null != this.InternalUriObject)
                    return this.InternalUriObject.IsFile;
                else
                    return false;
            }
        }

        public string LocalPath
        {
            get 
            {
                if (null != this.InternalUriObject)
                    return this.InternalUriObject.LocalPath;
                else
                    return null;
            }
        }

        public string LocalDiskPath
        {
            get
            {
                if (null != this.InternalUriObject && this.InternalUriObject.IsFile)
                    return this.InternalUriObject.LocalPath.Substring(12);      // remove the "\\localhost\" part
                else
                    return null;
            }
        }

        public override string ToString()
        {
            return this.InternalUriObject.AbsoluteUri;
        }

        /// <summary>
        /// Implicit cast to a String object, for ease of use.
        /// </summary>
        public static implicit operator string(DicomUri uri)
        {
            return uri.ToString();
        }

        private Uri InternalUriObject
        {
            get { return _internalUriObject; }
            set { _internalUriObject = value; }
        }

        private Uri _internalUriObject;
    }
}
