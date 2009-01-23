using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	public interface ILocalSopDataSource : IDicomMessageSopDataSource
	{
		DicomFile File { get; }

		string Filename { get; }
	}
}
