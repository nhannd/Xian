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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Common.Auditing
{
	/// <summary>
	/// The Auditor extension point (extensions implement <see cref="IAuditor"/>).
	/// </summary>
	/// <remarks>
	/// Although there would normally only be a single auditor present in a running application,
	/// it is possible for there to be more than one.  For example, you might have a local auditor
	/// that logs to a text file, and a remote auditor that logs to a .NET remoting service.
	/// </remarks>
	[ExtensionPoint()]
    public class AuditorExtensionPoint : ExtensionPoint<IAuditor>
    {
    }

	/// <summary>
	/// The AuditManager is responsible for loading all Auditors (<see cref="AuditorExtensionPoint"/>) 
	/// as well as passing each <see cref="IAuditMessage"/> that has been generated to the Auditors.
	/// </summary>
	public class AuditManager : BasicExtensionPointManager<IAuditor>
	{
		public AuditManager()
		{
		}

		/// <summary>
		/// Audits an <see cref="IAuditMessage"/> to all existing auditors (<see cref="AuditorExtensionPoint"/>).
		/// </summary>
		/// <param name="auditMessage">Interface to an <see cref="IAuditMessage"/>.</param>
		/// <seealso cref="BasicExtensionPointManager.LoadExtensions"/>
		public void Audit(IAuditMessage auditMessage)
		{
			LoadExtensions();

			foreach (IAuditor auditor in this.Extensions)
			{
				try
				{
					auditor.Audit(auditMessage);
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e);
				}
			}
		}

		protected override IExtensionPoint GetExtensionPoint()
		{
			return new AuditorExtensionPoint();
		}
	}
}
