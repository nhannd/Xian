
namespace ClearCanvas.Dicom.DataStore
{
	/// <summary>
	/// Provides a way to determine if a dicom dataset is valid for insertion into the 
	/// data store before attempting to insert it.
	/// </summary>
	/// <remarks>
	/// The implementation of this interface is not guaranteed to be thread-safe.
	/// </remarks>
	/// <seealso cref="IDicomPersistentStore"/>
	public interface IDicomPersistentStoreValidator
	{
		/// <summary>
		/// Validates the input data and insures it is appropriate for insertion into the datastore.
		/// </summary>
		/// <remarks>An exception is thrown when the data is invalid.</remarks>
		void Validate(DicomAttributeCollection metaInfo, DicomAttributeCollection sopInstanceDataset);
	}
}
