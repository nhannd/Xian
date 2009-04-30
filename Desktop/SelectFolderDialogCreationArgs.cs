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