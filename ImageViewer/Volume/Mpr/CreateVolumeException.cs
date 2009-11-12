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

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	public class CreateVolumeException : Exception
	{
		public CreateVolumeException() : base("An unexpected exception was encountered while creating the volume.") {}
		public CreateVolumeException(string message) : base(message) {}
		public CreateVolumeException(string message, Exception innerException) : base(message, innerException) {}
	}

	public class InsufficientFramesException : CreateVolumeException
	{
		public InsufficientFramesException() : base("Insufficient frames from which to create a volume. At least three are required.") {}
	}

	public class MultipleSourceSeriesException : CreateVolumeException
	{
		public MultipleSourceSeriesException() : base("Multiple studies/series were found in the source frames. All source frames must be from the same study/series.") {}
	}

	public class MultipleFramesOfReferenceException : CreateVolumeException
	{
		public MultipleFramesOfReferenceException() : base("Multiple frames of reference were found in the source frames. All source frames must have the same frame of reference.") {}
	}

	public class NullImageOrientationException : CreateVolumeException
	{
		public NullImageOrientationException() : base("One or more source frames do not have the image orientation defined.") {}
	}

	public class MultipleImageOrientationsException : CreateVolumeException
	{
		public MultipleImageOrientationsException() : base("Mulitple image orientations were found in the source frames. All source frames must have the same image orientation.") {}
	}

	public class UnevenlySpacedFramesException : CreateVolumeException
	{
		public UnevenlySpacedFramesException() : base("Source frames must be evenly spaced.") {}
	}

	public class UnsupportedGantryTiltAxisException : CreateVolumeException
	{
		public UnsupportedGantryTiltAxisException() : base("Multi-axial gantry tilted source frames are currently not supported.") {}
	}

	[ExceptionPolicyFor(typeof (CreateVolumeException))]
	[ExtensionOf(typeof (ExceptionPolicyExtensionPoint))]
	public class CreateVolumeExceptionPolicy : IExceptionPolicy
	{
		public void Handle(Exception ex, IExceptionHandlingContext exceptionHandlingContext)
		{
			string message = SR.MessageUnexpectedCreateVolumeException;
			if (ex is InsufficientFramesException)
				message = SR.MessageSourceDataSetNeedsThreeImagesForMpr;
			else if (ex is MultipleFramesOfReferenceException)
				message = SR.MessageSourceDataSetMustBeSingleFrameOfReference;
			else if (ex is MultipleImageOrientationsException)
				message = SR.MessageSourceDataSetMustBeSameImageOrientationPatient;
			else if (ex is MultipleSourceSeriesException)
				message = SR.MessageSourceDataSetMustBeSingleSeries;
			else if (ex is NullImageOrientationException)
				message = SR.MessageSourceDataSetMustDefineImageOrientationPatient;
			else if (ex is UnevenlySpacedFramesException)
				message = SR.MessageSourceDataSetImagesMustBeEvenlySpacedForMpr;
			else if (ex is UnsupportedGantryTiltAxisException)
				message = SR.MessageSourceDataSetCanOnlyGantryTiltedInOneAxis;
			exceptionHandlingContext.ShowMessageBox(message);
		}
	}
}