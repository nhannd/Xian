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
using System.Drawing;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	internal enum MprDisplaySetIdentifier
	{
		Identity,
		OrthoX,
		OrthoY,
		Oblique
	}

	internal class MprDisplaySet : DisplaySet
	{
		private readonly MprDisplaySetIdentifier _identifier;
		private VolumeSlicer _slicer;

		private MprDisplaySet(string name, string uid, string description, MprDisplaySetIdentifier identifier, VolumeSlicer slicer)
		: base(name, uid)
		{
			base.Description = description;
			_identifier = identifier;
			_slicer = slicer;
		}

		public MprDisplaySetIdentifier Identifier
		{
			get { return _identifier; }	
		}

		public int RotateAboutX
		{
			get { return _slicer.RotateAboutX; }	
		}

		public int RotateAboutY
		{
			get { return _slicer.RotateAboutY; }
		}
		
		public int RotateAboutZ
		{
			get { return _slicer.RotateAboutZ; }
		}

		public static MprDisplaySet Create(MprDisplaySetIdentifier identifier, Volume volume)
		{
			VolumeSlicer slicer = new VolumeSlicer(volume);

			string name;
			if (identifier == MprDisplaySetIdentifier.Identity)
			{
				slicer.SetSlicePlaneIdentity();
				name = "MPR (Identity)";
			}
			else if (identifier == MprDisplaySetIdentifier.OrthoX)
			{
				slicer.SetSlicePlaneOrthoX();
				name = "MPR (OrthoX)";
			}
			else if (identifier == MprDisplaySetIdentifier.OrthoY)
			{
				slicer.SetSlicePlaneOrthoY();
				name = "MPR (OrthoY)";
			}
			else
			{
				//TODO: Ideally we wouldn't have to set this initially right? The imagebox
				//	didn't want an empty display set so choose some reasonable default oblique
				slicer.SetSlicePlaneRotateDegrees(90, 0, 45);
				name = "MPR (Oblique)";
			}

			MprDisplaySet displaySet = new MprDisplaySet(name, DicomUid.GenerateUid().UID, name, identifier, slicer);
			slicer.PopulateDisplaySetFull(displaySet);
			return displaySet;
		}

		private Vector3D _startPoint, _endPoint;
		public void SetCutLine(Vector3D sourceOrientationColumn, Vector3D sourceOrientationRow, Vector3D startPoint, Vector3D endPoint)
		{
			if (_identifier != MprDisplaySetIdentifier.Oblique)
				throw new InvalidOperationException("Display set must be oblique.");

			_startPoint = startPoint;
			_endPoint = endPoint;

			int currentIndex = ImageBox.TopLeftPresentationImageIndex;

			object transformMemento = null;
			IPresentationImage oldImage = this.ImageBox.TopLeftPresentationImage;
			if (oldImage is ISpatialTransformProvider)
				transformMemento = (oldImage as ISpatialTransformProvider).SpatialTransform.CreateMemento();

			IComposableLut lut = null;
			if (oldImage is IVoiLutProvider)
				lut = (oldImage as IVoiLutProvider).VoiLutManager.GetLut();

			_slicer.SetSlicePlanePatient(sourceOrientationColumn, sourceOrientationRow, startPoint, endPoint);
			_slicer.PopulateDisplaySetOneImage(this);

			if (lut != null || transformMemento != null)
			{
				// Hacked this in so that the Imagebox wouldn't jump to first image all the time
				foreach (IPresentationImage image in PresentationImages)
				{
					if (lut != null && image is IVoiLutProvider)
						(image as IVoiLutProvider).VoiLutManager.InstallLut(lut.Clone());
					if (transformMemento != null && image is ISpatialTransformProvider)
						(image as ISpatialTransformProvider).SpatialTransform.SetMemento(transformMemento);
				}
			}

			//TODO: this seems a bit slow
			IPresentationImage closestImage = GetClosestSlice(startPoint + (endPoint - startPoint) * 2);
			if (closestImage == null)
				ImageBox.TopLeftPresentationImageIndex = currentIndex;
			else
				ImageBox.TopLeftPresentationImage = closestImage;

			Draw();
		}

		public void CreateFullObliqueDisplaySet()
		{
			if (_startPoint == null || _endPoint == null)
				return;

			object transformMemento = null;
			IPresentationImage oldImage = this.ImageBox.TopLeftPresentationImage;
			if (oldImage is ISpatialTransformProvider)
				transformMemento = (oldImage as ISpatialTransformProvider).SpatialTransform.CreateMemento();

			IComposableLut lut = null;
			if (oldImage is IVoiLutProvider)
				lut = (oldImage as IVoiLutProvider).VoiLutManager.GetLut();

			_slicer.PopulateDisplaySetFull(this);

			if (lut != null || transformMemento != null)
			{
				// Hacked this in so that the Imagebox wouldn't jump to first image all the time
				foreach (IPresentationImage image in PresentationImages)
				{
					if (lut != null && image is IVoiLutProvider)
						(image as IVoiLutProvider).VoiLutManager.InstallLut(lut.Clone());
					if (transformMemento != null && image is ISpatialTransformProvider)
						(image as ISpatialTransformProvider).SpatialTransform.SetMemento(transformMemento);
				}
			}

			//TODO: this seems a bit slow
			IPresentationImage closestImage = GetClosestSlice(_startPoint + (_endPoint - _startPoint) * 2);
			if (closestImage == null)
				ImageBox.TopLeftPresentationImageIndex = 0;
			else
				ImageBox.TopLeftPresentationImage = closestImage;

			Draw();
		}

		public void Rotate(int rotateX, int rotateY, int rotateZ)
		{
			if (_identifier != MprDisplaySetIdentifier.Oblique)
				throw new InvalidOperationException("Display set must be oblique.");

			// Hang on to the current index, we'll keep it the same with the new DisplaySet
			int currentIndex = ImageBox.TopLeftPresentationImageIndex;

			object transformMemento = null;
			IPresentationImage oldImage = this.ImageBox.TopLeftPresentationImage;
			if (oldImage is ISpatialTransformProvider)
				transformMemento = (oldImage as ISpatialTransformProvider).SpatialTransform.CreateMemento();

			IComposableLut lut = null;
			if (oldImage is IVoiLutProvider)
				lut = (oldImage as IVoiLutProvider).VoiLutManager.GetLut();

			try
			{
				_slicer.SetSlicePlaneRotateDegrees(rotateX, rotateY, rotateZ);
				_slicer.PopulateDisplaySetFull(this);
			}
			catch
			{
				//SB: do this for now until we can handle things properly
				return;
			}

			if (lut != null || transformMemento != null)
			{
				// Hacked this in so that the Imagebox wouldn't jump to first image all the time
				foreach (IPresentationImage image in PresentationImages)
				{
					if (lut != null && image is IVoiLutProvider)
						(image as IVoiLutProvider).VoiLutManager.InstallLut(lut.Clone());
					if (transformMemento != null && image is ISpatialTransformProvider)
						(image as ISpatialTransformProvider).SpatialTransform.SetMemento(transformMemento);
				}
			}

			ImageBox.TopLeftPresentationImageIndex = currentIndex;

			Draw();
		}

		private IPresentationImage GetClosestSlice(Vector3D positionPatient)
		{
			float closestDistance = float.MaxValue;
			IPresentationImage closestImage = null;

			foreach (IPresentationImage image in PresentationImages)
			{
				if (image is IImageSopProvider)
				{
					Frame frame = (image as IImageSopProvider).Frame;
					Vector3D positionCenterOfImage = frame.ImagePlaneHelper.ConvertToPatient(new PointF((frame.Columns - 1) / 2F, (frame.Rows - 1) / 2F));
					Vector3D distanceVector = positionCenterOfImage - positionPatient;
					float distance = distanceVector.Magnitude;

					if (distance <= closestDistance)
					{
						closestDistance = distance;
						closestImage = image;
					}
				}
			}

			return closestImage;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
				_slicer = null;

			base.Dispose(disposing);
		}
	}
}
