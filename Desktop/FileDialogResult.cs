#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Contains the results of a common file dialog operation.
	/// </summary>
	public class FileDialogResult
	{
		private readonly string[] _filenames;
		private readonly DialogBoxAction _action;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="filename"></param>
		public FileDialogResult(DialogBoxAction action, string filename)
		{
			_action = action;
			_filenames = new[]{filename};
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="action"></param>
		/// <param name="filenames"></param>
		public FileDialogResult(DialogBoxAction action, string[] filenames)
		{
			_action = action;
			_filenames = filenames;
		}


		/// <summary>
		/// Gets the filename.
		/// </summary>
		public string FileName
		{
			get { return CollectionUtils.FirstElement(_filenames); }
		}

		/// <summary>
		/// Gets the filenames (if multi-select was enabled).
		/// </summary>
		public string[] FileNames
		{
			get { return _filenames; }
		}

		/// <summary>
		/// Gets the result of the file dialog.
		/// </summary>
		public DialogBoxAction Action
		{
			get { return _action; }
		}
	
	}
}
