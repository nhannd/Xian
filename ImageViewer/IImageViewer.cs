using System;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Defines an image viewer.
	/// </summary>
    public interface IImageViewer : IDisposable
    {
		/// <summary>
		/// Gets the image viewer's <see cref="StudyTree"/>.
		/// </summary>
		StudyTree StudyTree { get; }
		
        /// <summary>
        /// Gets the <see cref="PhysicalWorkspace"/>.
        /// </summary>
        IPhysicalWorkspace PhysicalWorkspace { get; }

        /// <summary>
        /// Gets the <see cref="LogicalWorkspace"/>.
        /// </summary>
        ILogicalWorkspace LogicalWorkspace { get; }

        /// <summary>
        /// Gets the <see cref="EventBroker"/>.
        /// </summary>
        EventBroker EventBroker { get; }

        /// <summary>
        /// Gets the currently selected <see cref="IImageBox"/>
        /// </summary>
        /// <value>The currently selected <see cref="IImageBox"/>, or <b>null</b> if
		/// no <see cref="IImageBox"/> is currently selected.</value>
        IImageBox SelectedImageBox { get; }

        /// <summary>
        /// Gets the currently selected <see cref="ITile"/>
        /// </summary>
		/// <value>The currently selected <see cref="ITile"/>, or <b>null</b> if
		/// no <see cref="ITile"/> is currently selected.</value>
		ITile SelectedTile { get; }

        /// <summary>
        /// Gets the currently selected <see cref="IPresentationImage"/>
        /// </summary>
		/// <value>The currently selected <see cref="IPresentationImage"/>, or <b>null</b> if
		/// no <see cref="IPresentationImage"/> is currently selected.</value>
		IPresentationImage SelectedPresentationImage { get; }

		/// <summary>
		/// 
		/// </summary>
		IActionSet ExportedActions { get; }

        /// <summary>
        /// Gets the <see cref="CommandHistory"/> for this image viewer.
        /// </summary>
        CommandHistory CommandHistory { get; }

		/// <summary>
		/// Loads a study with a specific Study Instance UID from a specific source.
		/// </summary>
		/// <param name="studyInstanceUID">The Study Instance UID of the study to be loaded.</param>
		/// <param name="source">The name of the <see cref="IStudyLoader"/> to use, which is specified
		/// by <see cref="IStudyLoader.Name"/>.</param>
		/// <remarks>After this method is executed, the image viewer's <see cref="StudyTree"/>
		/// will be populated with the appropriate <see cref="Study"/>, <see cref="Series"/> 
		/// and <see cref="ImageSop"/> objects.
		/// 
		/// By default, the Framework provides an implementation of 
		/// <see cref="IStudyLoader"/> called <b>LocalDataStoreStudyLoader"</b> which loads
		/// studies from the local database.  If you have implemented your own 
		/// <see cref="IStudyLoader"/> and want to load a study using that implementation,
		/// just pass in the name provided by <see cref="IStudyLoader.Name"/> as the source.
		/// </remarks>
		/// <exception cref="OpenStudyException">The study could not be opened.</exception>
		void LoadStudy(string studyInstanceUID, string source);

		/// <summary>
		/// Loads images with the specified file paths.
		/// </summary>
		/// <param name="path">The file path of the image.</param>
		/// <exception cref="OpenStudyException">One or more images could not be opened.</exception>
		/// <exception cref="ArgumentNullException">A parameter is <b>null</b>.</exception>
		void LoadImages(string[] path);

		/// <summary>
		/// Loads images with the specified file paths and displays a progress bar.
		/// </summary>
		/// <param name="files">A list of file paths.</param>
		/// <param name="desktop">The desktop window.  This is necessary for
		/// a progress bar to be shown.</param>
		/// <param name="cancelled">A value that indicates whether the operation
		/// was cancelled.</param>
		/// <exception cref="OpenStudyException">One or more images could not be opened.</exception>
		/// <exception cref="ArgumentNullException">A parameter is <b>null</b>.</exception>
		void LoadImages(string[] files, IDesktopWindow desktop, out bool cancelled);
    }
}
