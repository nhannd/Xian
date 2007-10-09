namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// Defines a study loader.
	/// </summary>
	/// <remarks>
	/// <see cref="IStudyLoader"/> abstracts the loading of studies,
	/// allowing different many means of loading studies (e.g., local file system,
	/// DICOM WADO, DICOMDIR CD, streaming, etc.) to be treated in the same way.
	/// </remarks>
    public interface IStudyLoader
    {
		/// <summary>
		/// Gets the name of the study loader.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Starts the enumeration of images that match the specified
		/// Study Instance UID.
		/// </summary>
		/// <param name="studyInstanceUID"></param>
		/// <returns>Number of images in study.</returns>
		int Start(string studyInstanceUID);

		/// <summary>
		/// Loads the next image.
		/// </summary>
		/// <returns>The next <see cref="ImageSop"/> or <b>null</b> if there are
		/// no more images remaining.</returns>
		/// <remarks>
		/// Implementers of <see cref="IStudyLoader"/> should avoid loading pixel data
		/// in this method for performance reasons.
		/// </remarks>
		ImageSop LoadNextImage();
    }
}
