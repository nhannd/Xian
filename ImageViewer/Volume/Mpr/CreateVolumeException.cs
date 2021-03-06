#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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

	public class UnsupportedSourceImagesException : CreateVolumeException
	{
		public UnsupportedSourceImagesException() : base("Source images are of an unsupported type.") {}
	}

	public class UnsupportedPixelFormatSourceImagesException : CreateVolumeException
	{
		public UnsupportedPixelFormatSourceImagesException() : base("Source images must be 16-bit monochrome images.") {}
	}

	public class UnsupportedMultiFrameSourceImagesException : CreateVolumeException
	{
		public UnsupportedMultiFrameSourceImagesException() : this(null) {}

		public UnsupportedMultiFrameSourceImagesException(Exception innerException) : base("Multiframe source images are currently not supported.", innerException) {}
	}

	public class InsufficientFramesException : CreateVolumeException
	{
		public InsufficientFramesException() : base("Insufficient frames from which to create a volume. At least three are required.") {}
	}

	public class NullSourceSeriesException : CreateVolumeException
	{
		public NullSourceSeriesException() : base("One or more source frames are missing study and/or series information.") {}
	}

	public class MultipleSourceSeriesException : CreateVolumeException
	{
		public MultipleSourceSeriesException() : base("Multiple studies/series were found in the source frames. All source frames must be from the same study and series.") {}
	}

	public class NullFrameOfReferenceException : CreateVolumeException
	{
		public NullFrameOfReferenceException() : base("One or more source frames do not specify the frame of reference.") {}
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

	public class UncalibratedFramesException : CreateVolumeException
	{
		public UncalibratedFramesException() : base("Source frames must be calibrated.") {}
	}

	public class AnisotropicPixelAspectRatioException : CreateVolumeException
	{
		public AnisotropicPixelAspectRatioException() : base("Source frames must have isotropic pixel aspect ratio.") {}
	}

	public class UnsupportedGantryTiltAxisException : CreateVolumeException
	{
		public UnsupportedGantryTiltAxisException() : base("Source frames have a gantry tilt about an unsupported axis.") {}
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
			else if (ex is UnsupportedSourceImagesException)
				message = SR.MessageSourceDataSetImagesAreNotSupported;
			else if (ex is UnsupportedPixelFormatSourceImagesException)
				message = SR.MessageSourceDataSetImagesMustBe16BitGreyscale;
			else if (ex is UnsupportedMultiFrameSourceImagesException)
				message = SR.MessageSourceDataSetMultiFrameImagesAreNotSupported;
			else if (ex is MultipleFramesOfReferenceException)
				message = SR.MessageSourceDataSetMustBeSingleFrameOfReference;
			else if (ex is MultipleImageOrientationsException)
				message = SR.MessageSourceDataSetMustBeSameImageOrientationPatient;
			else if (ex is MultipleSourceSeriesException)
				message = SR.MessageSourceDataSetMustBeSingleSeries;
			else if (ex is NullFrameOfReferenceException)
				message = SR.MessageSourceDataSetMustSpecifyFrameOfReference;
			else if (ex is NullImageOrientationException)
				message = SR.MessageSourceDataSetMustDefineImageOrientationPatient;
			else if (ex is NullSourceSeriesException)
				message = SR.MessageSourceDataSetMustBeSingleSeries;
			else if (ex is UnevenlySpacedFramesException)
				message = SR.MessageSourceDataSetImagesMustBeEvenlySpacedForMpr;
			else if (ex is UncalibratedFramesException)
				message = SR.MessageSourceDataSetImagesMustBeCalibrated;
			else if (ex is AnisotropicPixelAspectRatioException)
				message = SR.MessageSourceDataSetImagesMayNotHaveAnisotropicPixels;
			else if (ex is UnsupportedGantryTiltAxisException)
				message = SR.MessageSourceDataSetImagesMayBotBeGantrySlewed;
			exceptionHandlingContext.ShowMessageBox(message);
		}
	}
}