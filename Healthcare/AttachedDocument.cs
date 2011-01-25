#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using System.Collections.Generic;
using System.IO;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Healthcare {


	/// <summary>
	/// AttachedDocument entity
	/// </summary>
	public partial class AttachedDocument
	{
		/// <summary>
		/// Copy constructor that creates either an exact copy, or an "unprocessed" copy.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="unprocessed"></param>
		protected AttachedDocument(AttachedDocument source, bool unprocessed)
		{
			_creationTime = unprocessed ? Platform.Time : source.CreationTime;
			_mimeType = source.MimeType;
	  		_fileExtension = source.FileExtension;

			_contentUrl = source.ContentUrl;
		}
	
		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}

		/// <summary>
		/// Duplicates this document.
		/// </summary>
		/// <remarks>
		/// Does not duplicate the remote resource. However, the remote resource is treated as immutable,
		/// so having multiple instances referring to the same remote resource poses no problems.
		/// </remarks>
		/// <returns></returns>
		public virtual AttachedDocument Duplicate(bool unprocessed)
		{
			// derived classes may implement support
			throw new NotSupportedException();
		}

		/// <summary>
		/// Creates a ghost copy of this document.
		/// </summary>
		/// <returns></returns>
		public virtual AttachedDocument CreateGhostCopy()
		{
			// derived classes may implement support
			throw new NotSupportedException();
		}

		public virtual DateTime? DocumentReceivedTime
		{
			get { return null; }
		}

		/// <summary>
		/// Marks this document as having been attached.
		/// </summary>
		public virtual void Attach()
		{
		}

		/// <summary>
		/// Marks this document as being detached.
		/// </summary>
		public virtual void Detach()
		{
		}

		/// <summary>
		/// Summary of derived-class specific details of the attached document
		/// </summary>
		public virtual IDictionary<string, string> DocumentHeaders
		{
			get { return null; }
		}

		public virtual string DocumentTypeName
		{
			get { return "Attached Document"; }
		}

		/// <summary>
		/// Gets the file associated with this attached document, from the document store.
		/// </summary>
		/// <returns></returns>
		public virtual string GetFile(IAttachedDocumentStore documentStore)
		{
			return documentStore.GetDocument(_contentUrl);
		}

		/// <summary>
		/// Sets the file associated with this attached document, and stores a copy to the document store.
		/// </summary>
		/// <returns></returns> 
		public virtual void PutFile(IAttachedDocumentStore documentStore, string localFilePath)
		{
			const string pathDelimiter = "/";

			var builder = new StringBuilder();
			builder.Append(_creationTime.Year.ToString());
			builder.Append(pathDelimiter);
			builder.Append(_creationTime.Month.ToString());
			builder.Append(pathDelimiter);
			builder.Append(_creationTime.Day.ToString());
			builder.Append(pathDelimiter);

			// important that we always generate a new GUID here, because multiple AttachedDocument objects
			// are allowed to refer to the same remote resource - therefore we must treat the remote resource
			// as immutable
			builder.AppendFormat("{0}.{1}", Guid.NewGuid().ToString("D"), _fileExtension);

			_contentUrl = builder.ToString();
			documentStore.PutDocument(_contentUrl, localFilePath);
		}



		/// <summary>
		/// Shifts the object in time by the specified number of minutes, which may be negative or positive.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The method is not intended for production use, but is provided for the purpose
		/// of generating back-dated data for demos and load-testing.
		/// </para>
		/// </remarks>
		/// <param name="minutes"></param>
		public virtual void TimeShift(int minutes)
		{
			_creationTime = _creationTime.AddMinutes(minutes);
		}
	}
}