#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

namespace ClearCanvas.Common.Auditing
{
	/// <summary>
	/// Extension point for <see cref="IAuditor"/>s.
	/// </summary>
	/// <remarks>
	/// Although there would normally only be a single auditor present in a running application,
	/// it is possible for there to be more than one.  For example, you might have a local auditor
	/// that logs to a text file, and a remote auditor that logs to a WCF service.
	/// </remarks>
	[ExtensionPoint()]
	public sealed class AuditorExtensionPoint : ExtensionPoint<IAuditor>
	{
	}

	/// <summary>
	/// An auditor.
	/// </summary>
	/// <remarks>
	/// <see cref="IAuditor"/>s are internally created an managed via the <see cref="AuditorExtensionPoint"/>.
	/// The <see cref="Platform.AuditManager"/> is itself an <see cref="IAuditor"/>, but internally it 
	/// creates/manages a list of <see cref="IAuditor"/>s.
	/// </remarks>
	public interface IAuditor
	{
		/// <summary>
		/// Audits an <see cref="IAuditMessage"/>.
		/// </summary>
		void Audit(IAuditMessage auditMessage);
	}
}
