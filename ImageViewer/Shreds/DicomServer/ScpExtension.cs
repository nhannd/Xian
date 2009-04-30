#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

using System.Collections.Generic;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Network.Scp;

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
{
	public interface IDicomServerContext
	{
		string AETitle { get; }
		string Host { get; }
		int Port { get; }
		string InterimStorageDirectory { get; }
	}

	public abstract class ScpExtension : IDicomScp<IDicomServerContext>
	{
		private IDicomServerContext _context;
		private readonly List<SupportedSop> _supportedSops;

		protected ScpExtension(IEnumerable<SupportedSop> supportedSops)
		{
			_supportedSops = new List<SupportedSop>(supportedSops);
		}

		protected IDicomServerContext Context
		{
			get { return _context; }
		}

		#region IDicomScp<ServerContext> Members

		public DicomPresContextResult VerifyAssociation(AssociationParameters association, byte pcid)
		{
			DicomRejectResult result;
			DicomRejectReason reason;
			if (!AssociationVerifier.VerifyAssociation(Context, association, out result, out reason))
				return DicomPresContextResult.RejectUser;

			return OnVerifyAssociation(association, pcid);
		}

		public virtual DicomPresContextResult OnVerifyAssociation(AssociationParameters association, byte pcid)
		{
			return DicomPresContextResult.Accept;
		}

		public abstract bool OnReceiveRequest(Dicom.Network.DicomServer server, ServerAssociationParameters association, byte presentationID, DicomMessage message);

		public IList<SupportedSop> GetSupportedSopClasses()
		{
			return _supportedSops;
		}

		public void SetContext(IDicomServerContext context)
		{
			_context = context;	
		}

		#endregion
	}
}
