using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Dicom.DataStore
{
    public class DicomUri : IEquatable<DicomUri>
    {
		private Uri _internalUriObject;

		/// <summary>
		/// Constructor for NHibernate.
		/// </summary>
        private DicomUri()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
		public DicomUri(string uri)
        {
        	SetInternalUri(uri);
        }

		/// <summary>
		/// Constructor.
		/// </summary>
		public DicomUri(Uri newUri)
		{
			Platform.CheckForNullReference(newUri, "newUri");
			this.InternalUriObject = newUri;
		}

		/// <summary>
		/// NHibernate Property.
		/// </summary>
		protected virtual string InternalUri
		{
			get { return this.InternalUriObject.AbsoluteUri; }
			set { SetInternalUri(value); }
		}

		private void SetInternalUri(string uri)
		{
			if (String.IsNullOrEmpty(uri))
				throw new System.ArgumentNullException("uri", SR.ExceptionUriCannotBeNullOrEmpty);

			this.InternalUriObject = new Uri(uri);
		}

		private Uri InternalUriObject
		{
			get { return _internalUriObject; }
			set { _internalUriObject = value; }
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

    	#region IEquatable<DicomUri> Members

    	public bool Equals(DicomUri other)
    	{
			return InternalUriObject.Equals(other.InternalUriObject);
    	}

    	#endregion

    	public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

			if (obj is DicomUri)
				return this.Equals((DicomUri) obj);

            return false;
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
    }
}
