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
			_resliceMatrix = new Matrix(resliceMatrix);
		}

		protected override byte[] CreateFrameNormalizedPixelData(int frameNumber)
		{
			return _volumeSlicer.GenerateFrameNormalizedPixelData(_resliceMatrix);
		}
	}
}
