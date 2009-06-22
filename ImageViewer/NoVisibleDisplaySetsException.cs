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

using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Exception handling policy for <see cref="NoVisibleDisplaySetsException"/>s.
	/// </summary>
	[ExceptionPolicyFor(typeof(NoVisibleDisplaySetsException))]

	[ExtensionOf(typeof(ExceptionPolicyExtensionPoint))]
	public sealed class NoVisibleDisplaySetsExceptionHandlingPolicy : IExceptionPolicy
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public NoVisibleDisplaySetsExceptionHandlingPolicy()
		{
		}

		#region IExceptionPolicy Members

		///<summary>
		/// Handles the specified exception.
		///</summary>
		public void Handle(Exception e, IExceptionHandlingContext exceptionHandlingContext)
		{
			exceptionHandlingContext.Log(LogLevel.Error, e);
			exceptionHandlingContext.ShowMessageBox(SR.MessageNoVisibleDisplaySets);
		}

		#endregion
	}

	/// <summary>
	/// Exception thrown by the <see cref="LayoutManager"/> when no display sets
	/// were added to the <see cref="ILogicalWorkspace"/>.
	/// </summary>
	public class NoVisibleDisplaySetsException : Exception
	{
		internal NoVisibleDisplaySetsException(string message)
			: base(message)
		{
		}
	}
}
