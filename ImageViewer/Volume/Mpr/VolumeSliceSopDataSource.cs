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
		private readonly Volume _volume;
		private readonly Matrix _sliceMatrix;

		internal VolumeSliceSopDataSource(DicomMessageBase sourceMessage, Volume vol, Matrix resliceMatrix)
			: base(sourceMessage)
		{
			_volume = vol;
			_sliceMatrix = resliceMatrix;
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
			
			VolumeSlicer slicer = new VolumeSlicer();
			return slicer.GenerateFrameNormalizedPixelData(_volume, _sliceMatrix);
		}
	}
}
