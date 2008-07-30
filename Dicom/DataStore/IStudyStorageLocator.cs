namespace ClearCanvas.Dicom.DataStore
{
	/// <summary>
	/// For use by the Local Data Store service only.
	/// </summary>
	public interface IStudyStorageLocator
	{
		string GetStudyStoragePath(string studyInstanceUid);
	}
}