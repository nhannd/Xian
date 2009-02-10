using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	//TODO: share a 'model' data source between all the slices and remove it from Volume class.
	//class ModelVolumeSliceSopDataSource : DicomMessageSopDataSource
	//{
	//    private readonly Volume _volume;
	//    private readonly DicomMessageBase _modelDicom;
	//}

	class VolumeSliceSopDataSource : DicomMessageSopDataSource 
	{
		private readonly VolumeSlicer _volumeSlicer;
		private readonly Matrix _resliceMatrix;

		internal VolumeSliceSopDataSource(DicomMessageBase sourceMessage, VolumeSlicer slicer, Matrix resliceMatrix)
			: base(sourceMessage)
		{
			_volumeSlicer = slicer;
			_resliceMatrix = resliceMatrix;
		}

		//ggerade ToRes: I think I'm going to do away with the whole up front creation of the slices, but
		//	I figured I'd try to get this working and review with Stewart.
		private byte[] _pixelData;
		internal VolumeSliceSopDataSource(DicomMessageBase sourceMessage, byte[] pixelData)
			: base(sourceMessage)
		{
			_pixelData = pixelData;
		}

		protected override byte[] CreateFrameNormalizedPixelData(int frameNumber)
		{
			if (_pixelData != null)
			{
				byte[] temp = _pixelData;
				_pixelData = null;
				return temp;
			}
			
			return _volumeSlicer.GenerateFrameNormalizedPixelData(_resliceMatrix);
		}
	}
}
