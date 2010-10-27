#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
