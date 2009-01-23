using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	public interface IDicomMessageSopDataSource : ISopDataSource
	{
		DicomMessageBase SourceMessage { get; }
	}
}