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
		: base(name, uid, description)
		{
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
				//ggerade ToRes: Ideally we wouldn't have to set this initially right? The imagebox
				//	didn't want an empty display set so choose some reasonable default oblique
				slicer.SetSlicePlaneOblique(90, 0, 45);
				name = "MPR (Oblique)";
			}

			MprDisplaySet displaySet = new MprDisplaySet(name, DicomUid.GenerateUid().UID, name, identifier, slicer);
			slicer.PopulateDisplaySet(displaySet, false);
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
			_slicer.PopulateDisplaySet(this, true);

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

			_slicer.PopulateDisplaySet(this, false);

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

			//Vector3D sliceThroughPatient = _volume.CenterPointPatient;
			//sliceThroughPatient.X = 0;
			//sliceThroughPatient.Y = 0;
			//sliceThroughPatient.Z = 0;
			//_obliqueSlicer.SliceThroughPointPatient = sliceThroughPatient;
			//_obliqueSlicer.SliceExtentMillimeters = 150;

			try
			{
				_slicer.SetSlicePlaneOblique(rotateX, rotateY, rotateZ);
				_slicer.PopulateDisplaySet(this, false);
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
