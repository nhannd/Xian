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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	//TODO: would like to just add IsImageStorage property to SopClass, but this'll do for now.
	internal static class SopDataHelper
	{
		private static readonly ReadOnlyCollection<SopClass> _imageSopClasses = new List<SopClass>(GetImageSopClasses()).AsReadOnly();

		private static IEnumerable<SopClass> GetImageSopClasses()
		{
			yield return SopClass.ComputedRadiographyImageStorage;
			yield return SopClass.CtImageStorage;

			yield return SopClass.DigitalIntraOralXRayImageStorageForPresentation;
			yield return SopClass.DigitalIntraOralXRayImageStorageForProcessing;

			yield return SopClass.DigitalMammographyXRayImageStorageForPresentation;
			yield return SopClass.DigitalMammographyXRayImageStorageForProcessing;

			yield return SopClass.DigitalXRayImageStorageForPresentation;
			yield return SopClass.DigitalXRayImageStorageForProcessing;

			yield return SopClass.EnhancedCtImageStorage;
			yield return SopClass.EnhancedMrImageStorage;

			yield return SopClass.EnhancedXaImageStorage;

			yield return SopClass.EnhancedXrfImageStorage;

			yield return SopClass.MrImageStorage;

			yield return SopClass.MultiFrameGrayscaleByteSecondaryCaptureImageStorage;
			yield return SopClass.MultiFrameGrayscaleWordSecondaryCaptureImageStorage;
			yield return SopClass.MultiFrameSingleBitSecondaryCaptureImageStorage;
			yield return SopClass.MultiFrameTrueColorSecondaryCaptureImageStorage;

			yield return SopClass.NuclearMedicineImageStorageRetired;
			yield return SopClass.NuclearMedicineImageStorage;

			yield return SopClass.OphthalmicPhotography16BitImageStorage;
			yield return SopClass.OphthalmicPhotography8BitImageStorage;
			yield return SopClass.OphthalmicTomographyImageStorage;

			yield return SopClass.PositronEmissionTomographyImageStorage;

			yield return SopClass.RtImageStorage;

			yield return SopClass.SecondaryCaptureImageStorage;

			yield return SopClass.UltrasoundImageStorage;
			yield return SopClass.UltrasoundImageStorageRetired;
			yield return SopClass.UltrasoundMultiFrameImageStorage;
			yield return SopClass.UltrasoundMultiFrameImageStorageRetired;

			yield return SopClass.VideoEndoscopicImageStorage;
			yield return SopClass.VideoMicroscopicImageStorage;
			yield return SopClass.VideoPhotographicImageStorage;

			yield return SopClass.VlEndoscopicImageStorage;
			yield return SopClass.VlMicroscopicImageStorage;
			yield return SopClass.VlPhotographicImageStorage;
			yield return SopClass.VlSlideCoordinatesMicroscopicImageStorage;

			yield return SopClass.XRay3dAngiographicImageStorage;
			yield return SopClass.XRay3dCraniofacialImageStorage;

			yield return SopClass.XRayAngiographicBiPlaneImageStorageRetired;
			yield return SopClass.XRayAngiographicImageStorage;

			yield return SopClass.XRayRadiofluoroscopicImageStorage;
		}

		public static bool IsImageSop(string sopClassUid)
		{
			return IsImageSop(SopClass.GetSopClass(sopClassUid));
		}

		public static bool IsImageSop(SopClass sopClass)
		{
			return _imageSopClasses.Contains(sopClass);
		}
	}
}
