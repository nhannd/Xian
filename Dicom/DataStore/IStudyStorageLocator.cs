namespace ClearCanvas.Dicom.DataStore
{
	public interface IStudyStorageLocator
	{
		string GetStudyStoragePath(string studyInstanceUid);
	}
}