using ClearCanvas.Dicom;
using System;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	public interface ISopDataSource : IDicomAttributeProvider, IDisposable
	{
		string StudyInstanceUid { get; }
		string SeriesInstanceUid { get; }
		string SopInstanceUid { get; }
		int InstanceNumber { get; }

		string SopClassUid { get; }
		string TransferSyntaxUid { get; }

		byte[] GetFrameNormalizedPixelData(int frameNumber);
		void UnloadFrameData(int frameNumber);

		bool IsStored { get; }

		string StudyLoaderName { get; }
		object Server { get; }
	}
}
