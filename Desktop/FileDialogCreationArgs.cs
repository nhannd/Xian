using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Describes a file dialog extension filter.
	/// </summary>
	public class FileExtensionFilter
	{
		private string _filter;
		private string _description;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="description"></param>
		public FileExtensionFilter(string filter, string description)
		{
			_filter = filter;
			_description = description;
		}

		/// <summary>
		/// Gets or sets the value of the filter, which must be a wildcard expression (e.g. *.txt).
		/// </summary>
		public string Filter
		{
			get { return _filter; }
			set { _filter = value; }
		}

		/// <summary>
		/// Gets or sets the value displayed for the filter, e.g. Text files (*.txt).
		/// </summary>
		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}
	}


	/// <summary>
	/// Holds parameters that initialize the display of a common file dialog.
	/// </summary>
	public class FileDialogCreationArgs
	{
		private string _fileExtension;
		private string _filename;
		private string _directory;
		private string _title;
		private readonly List<FileExtensionFilter> _filters;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="directory"></param>
		/// <param name="filename"></param>
		/// <param name="fileExtension"></param>
		/// <param name="filters"></param>
		public FileDialogCreationArgs(string filename, string directory, string fileExtension, IEnumerable<FileExtensionFilter> filters)
		{
			_directory = directory;
			_filename = filename;
			_fileExtension = fileExtension;
			_filters = new List<FileExtensionFilter>(filters);
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="filename"></param>
		public FileDialogCreationArgs(string filename)
			: this(filename, null, null, new FileExtensionFilter[] { })
		{

		}

		/// <summary>
		/// Gets or sets the default extension to append to the filename, if not specified by user.
		/// </summary>
		public string FileExtension
		{
			get { return _fileExtension; }
			set { _fileExtension = value; }
		}

		/// <summary>
		/// Gets or sets the initial value of the file name.
		/// </summary>
		public string FileName
		{
			get { return _filename; }
			set { _filename = value; }
		}

		/// <summary>
		/// Gets or sets the initial directory.
		/// </summary>
		public string Directory
		{
			get { return _directory; }
			set { _directory = value; }
		}

		/// <summary>
		/// Gets or sets the title of the file dialog.
		/// </summary>
		public string Title
		{
			get { return _title; }
			set { _title = value; }
		}

		/// <summary>
		/// Gets the list of file extension filters.
		/// </summary>
		public List<FileExtensionFilter> Filters
		{
			get { return _filters; }
		}
	}
}
