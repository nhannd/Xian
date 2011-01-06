#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;

namespace ClearCanvas.Dicom.DataStore
{
    //TODO: Get rid of this?  It's always just converted to/from a string anyway.
	public class DicomUri : IEquatable<DicomUri>
    {
		private Uri _internalUriObject;

		//for NHibernate.
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
			InternalUriObject = newUri;
		}

		/// <summary>
		/// NHibernate Property.
		/// </summary>
		protected virtual string InternalUri
		{
			get { return InternalUriObject.AbsoluteUri; }
			set { SetInternalUri(value); }
		}

		private void SetInternalUri(string uri)
		{
			if (String.IsNullOrEmpty(uri))
				throw new ArgumentNullException("uri", "Uri cannot be null or empty.");

			InternalUriObject = new Uri(uri);
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
                if (null != InternalUriObject)
                    return InternalUriObject.IsFile;
                else
                    return false;
            }
        }

        public string LocalPath
        {
            get 
            {
                if (null != InternalUriObject)
                    return InternalUriObject.LocalPath;
                else
                    return null;
            }
        }

        public string LocalDiskPath
        {
            get
            {
                if (null != InternalUriObject && InternalUriObject.IsFile)
                    return InternalUriObject.LocalPath.Substring(12);      // remove the "\\localhost\" part
                else
                    return null;
            }
        }

    	#region IEquatable<DicomUri> Members

    	public bool Equals(DicomUri other)
    	{
			if (other == null)
				return false;

			return InternalUriObject.Equals(other.InternalUriObject);
    	}

    	#endregion

    	public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

			if (obj is DicomUri)
				return Equals((DicomUri) obj);

            return false;
        }

		public override int GetHashCode()
		{
			return _internalUriObject.GetHashCode();
		}

        public override string ToString()
        {
            return InternalUriObject.AbsoluteUri;
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
