using System;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Holds parameters that initialize the display of a folder browser dialog.
	/// </summary>
	public class SelectFolderDialogCreationArgs
	{
		private string _path;
		private string _prompt;
		private bool _allowCreateNewFolder;

		/// <summary>
		/// Constructs an object holding parameters for the display of a folder browser dialog.
		/// </summary>
		public SelectFolderDialogCreationArgs()
		{
			_path = Environment.CurrentDirectory;
			_allowCreateNewFolder = true;
		}

		/// <summary>
		/// Constructs an object holding parameters for the display of a folder browser dialog.
		/// </summary>
		/// <param name="path"></param>
		public SelectFolderDialogCreationArgs(string path)
		{
			_path = path;
			_allowCreateNewFolder = true;
		}

		/// <summary>
		/// Gets or sets the path that the folder browser dialog will show initially.
		/// </summary>
		public string Path
		{
			get { return _path; }
			set { _path = value; }
		}

		/// <summary>
		/// Gets or sets the prompt to the user shown on the dialog.
		/// </summary>
		public string Prompt
		{
			get { return _prompt; }
			set { _prompt = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating if the creation of a new folder should be allowed by the dialog.
		/// </summary>
		public bool AllowCreateNewFolder
		{
			get { return _allowCreateNewFolder; }
			set { _allowCreateNewFolder = value; }
		}
	}
}