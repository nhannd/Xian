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
using ClearCanvas.Desktop;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Thrown when an error occurs while cloning an <see cref="IDisplaySet"/>.
	/// </summary>
	public class DisplaySetCloningException : Exception
	{
		internal DisplaySetCloningException(IDisplaySet sourceDisplaySet, Exception innerException)
			: base(BuildMessage(sourceDisplaySet), innerException)
		{

		}

		internal static string BuildMessage(IDisplaySet sourceDisplaySet)
		{
			string message = "An error has occurred while attempting to clone a display set";
			if (!String.IsNullOrEmpty(sourceDisplaySet.Name))
				message += string.Format(" (name = {0})", sourceDisplaySet.Name);
			
			message += ".";
			return message;
		}
	}

	/// <summary>
	/// Thrown when an error occurs while cloning an <see cref="IPresentationImage"/>.
	/// </summary>
	public class PresentationImageCloningException : Exception
	{
		internal PresentationImageCloningException(IPresentationImage sourceImage, Exception innerException)
			: base(BuildMessage(sourceImage), innerException)
		{
		}

		internal static string BuildMessage(IPresentationImage sourceImage)
		{
			string message = "An error has occurred while attempting to clone an image";
			if (!String.IsNullOrEmpty(sourceImage.Uid))
				message += string.Format(" (uid = {0})", sourceImage.Uid);

			message += ".";
			return message;
		}
	}

	/// <summary>
	/// Exception policy for cloning of <see cref="IPresentationImage"/>s and <see cref="IDisplaySet"/>s.
	/// </summary>
	[ExceptionPolicyFor(typeof(DisplaySetCloningException))]
	[ExceptionPolicyFor(typeof(PresentationImageCloningException))]
	[ExtensionOf(typeof(ExceptionPolicyExtensionPoint))]
	public class CloningExceptionPolicy : IExceptionPolicy
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public CloningExceptionPolicy()
		{
		}

		#region IExceptionPolicy Members

		///<summary>
		/// Handles the specified exception.
		///</summary>
		public void Handle(Exception e, IExceptionHandlingContext exceptionHandlingContext)
		{
			if (e is DisplaySetCloningException)
				exceptionHandlingContext.ShowMessageBox(SR.MessageErrorCloningDisplaySet);
			else
				exceptionHandlingContext.ShowMessageBox(SR.MessageErrorCloningPresentationImage);
		}

		#endregion
	}
}
