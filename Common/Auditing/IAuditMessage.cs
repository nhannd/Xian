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


namespace ClearCanvas.Common.Auditing
{
	/// <summary>
	/// IAuditMessage is a base interface for any message that is sent to an auditor.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The following assumptions are made about audit messages:
	/// </para>
	/// <para>
	/// <list type="bullet">
	///	<item>
	/// Audit messages are text-based.
	/// </item>
	/// <item>
	/// The <see cref="GetMessage"/> method is used to retrieve the message text.  The message text
	/// is formatted/created by the implementation of <see cref="IAuditMessage"/>.
	/// </item>
	/// <item>
	/// It is up to the implementor, but it is recommended that the implementation of
	/// <see cref="IAuditMessage"/> be responsible (and therefore have access to all necessary information)
	/// for constructing the message text.  The Auditor can then simply use the <see cref="GetMessage"/> method
	/// to log the message to the appropriate Audit Repository.  In this scenario, the
	/// auditor is only responsible for ensuring that the audit messages are reliably stored
	/// to the Audit Repository (whatever that may be: text file, logging service, etc).  The implementation
	/// of <see cref="IAuditMessage"/> is responsible for gathering the required information to format the
	/// message appropriately.
	/// </item>
	/// </list>
	/// </para>
	/// </remarks>

	public interface IAuditMessage
	{
		/// <summary>
		/// Get the text of the Audit Message.
		/// </summary>
		/// <returns>The message text.</returns>
		string GetMessage();
	}
}
