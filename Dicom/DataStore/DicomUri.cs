#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
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
