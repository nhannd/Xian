
namespace ClearCanvas.Dicom.DataStore
{
	/// <summary>
	/// For use by the Local Data Store service (or equivalent service) only.
	/// </summary>
	public interface IStudyStorageLocator
	{
		string GetStudyStorageDirectory(string studyInstanceUid);
	}
}
