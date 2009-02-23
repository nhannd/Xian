using System;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	internal enum DisplaySetIdentifier
	{
		Identity,
		OrthoX,
		OrthoY,
		Oblique
	}

	internal class MprDisplaySet : DisplaySet
	{
		private readonly DisplaySetIdentifier _identifier;
		private readonly VolumeSlicer _slicer;

		private MprDisplaySet(string name, string uid, string description, DisplaySetIdentifier identifier, VolumeSlicer slicer)
		: base(name, uid, description)
		{
			_identifier = identifier;
			_slicer = slicer;
		}

		public DisplaySetIdentifier Identifier
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

		public static MprDisplaySet Create(DisplaySetIdentifier identifier, Volume volume)
		{
			VolumeSlicer slicer = new VolumeSlicer(volume);

			string name;
			if (identifier == DisplaySetIdentifier.Identity)
			{
				slicer.SetSlicePlaneIdentity();
				name = "MPR (Identity)";
			}
			else if (identifier == DisplaySetIdentifier.OrthoX)
			{
				slicer.SetSlicePlaneOrthoX();
				name = "MPR (OrthoX)";
			}
			else if (identifier == DisplaySetIdentifier.OrthoY)
			{
				slicer.SetSlicePlaneOrthoY();
				name = "MPR (OrthoY)";
			}
			else
			{
				slicer.SetSlicePlaneOblique(45, 0, 0); //just choose a default.
				name = "MPR (Oblique)";
			}
			
			MprDisplaySet displaySet = new MprDisplaySet(name, DicomUid.GenerateUid().UID, name, identifier, slicer);
			slicer.PopulateDisplaySet(displaySet);
			return displaySet;
		}

		public void SetCutLine(Vector3D sourceOrientationColumn, Vector3D sourceOrientationRow, Vector3D startPoint, Vector3D endPoint)
		{
			if (_identifier != DisplaySetIdentifier.Oblique)
				throw new InvalidOperationException("Display set must be oblique.");

			int currentIndex = this.ImageBox.TopLeftPresentationImageIndex;

			//ggerade ToRes: What about other settings like Window/level? It is lost when the DisplaySets
			// are swapped out.

			_slicer.SetSlicePlanePatient(sourceOrientationColumn, sourceOrientationRow, startPoint, endPoint);
			_slicer.PopulateDisplaySet(this);

			// Hacked this in so that the Imagebox wouldn't jump to first image all the time
			this.ImageBox.TopLeftPresentationImageIndex = currentIndex;
			Draw();
		}

		public void Rotate(int rotateX, int rotateY, int rotateZ)
		{
			if (_identifier != DisplaySetIdentifier.Oblique)
				throw new InvalidOperationException("Display set must be oblique.");

			// Hang on to the current index, we'll keep it the same with the new DisplaySet
			int currentIndex = ImageBox.TopLeftPresentationImageIndex;

			//ggerade ToRes: What about other settings like Window/level? It is lost when the DisplaySets
			// are swapped out.

			_slicer.SetSlicePlaneOblique(rotateX, rotateY, rotateZ);

			//Vector3D sliceThroughPatient = _volume.CenterPointPatient;
			//sliceThroughPatient.X = 0;
			//sliceThroughPatient.Y = 0;
			//sliceThroughPatient.Z = 0;
			//_obliqueSlicer.SliceThroughPointPatient = sliceThroughPatient;
			//_obliqueSlicer.SliceExtentMillimeters = 150;

			// Hacked this in so that the Imagebox wouldn't jump to first image all the time
			ImageBox.TopLeftPresentationImageIndex = currentIndex;

			// Taken care of by hack above
			//obliqueImageBox.TopLeftPresentationImageIndex = currentIndex;

			Draw();
		}
	}
}
