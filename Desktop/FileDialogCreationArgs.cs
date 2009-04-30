#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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
