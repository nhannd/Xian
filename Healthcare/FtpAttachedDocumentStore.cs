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
using System.IO;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare
{
	/// <summary>
	/// Implementation of <see cref="IAttachedDocumentStore"/> that stores
	/// documents on an FTP site.
	/// </summary>
	public class FtpAttachedDocumentStore : IAttachedDocumentStore
	{
		private readonly FtpFileTransfer _ftpFileTransfer;
		private readonly string _tempPath;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="ftpFileTransfer">FTP file transfer object.</param>
		/// <param name="tempPath">A path that can be used to store downloaded document files.</param>
		public FtpAttachedDocumentStore(FtpFileTransfer ftpFileTransfer, string tempPath)
		{
			_ftpFileTransfer = ftpFileTransfer;
			_tempPath = tempPath;
		}

		#region Implementation of IAttachedDocumentStore

		/// <summary>
		/// Gets the document at the specified URI, returning a local path to a copy of the document.
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public string GetDocument(string url)
		{
			var fullUrl = new Uri(_ftpFileTransfer.BaseUri, url);
			var localFilePath = Path.Combine(_tempPath, Path.GetFileName(fullUrl.LocalPath));
			_ftpFileTransfer.Download(new FileTransferRequest(fullUrl, localFilePath));

			return localFilePath;
		}

		/// <summary>
		/// Puts the document at the specified local path to the specified URI.
		/// </summary>
		/// <param name="url"></param>
		/// <param name="localFilePath"></param>
		public void PutDocument(string url, string localFilePath)
		{
			var fullUrl = new Uri(_ftpFileTransfer.BaseUri, url);
			_ftpFileTransfer.Upload(new FileTransferRequest(fullUrl, localFilePath));
		}

		#endregion
	}
}
