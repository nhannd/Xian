#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Contains the results of a common file dialog operation.
	/// </summary>
	public class FileDialogResult
	{
		private readonly string _filename;
		private readonly DialogBoxAction _action;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="filename"></param>
		public FileDialogResult(DialogBoxAction action, string filename)
		{
			_action = action;
			_filename = filename;
		}

		/// <summary>
		/// Gets the filename.
		/// </summary>
		public string FileName
		{
			get { return _filename; }
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
