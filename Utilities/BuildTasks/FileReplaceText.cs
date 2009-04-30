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
using System.IO;
using System.Text;
using Microsoft.Build.Utilities;

namespace ClearCanvas.Utilities.BuildTasks
{
	public class FileReplaceText : Task
	{
		private string _filePath;
		private string _textToReplace;
		private string _replacementText;

		public FileReplaceText()
		{
		}

		public string FilePath
		{
			get { return _filePath; }
			set { _filePath = value; }
		}

		public string TextToReplace
		{
			get { return _textToReplace; }
			set { _textToReplace = value; }
		}

		public string ReplacementText
		{
			get { return _replacementText; }
			set { _replacementText = value; }
		}

		public override bool Execute()
		{
			try
			{
				if (!File.Exists(_filePath))
				{
					base.Log.LogMessage(String.Format("File does not exist: {0}", _filePath));
					return false;
				}

				string oldText = "";
				Encoding encoding;
				using (StreamReader reader = new StreamReader(_filePath, true))
				{
					encoding = reader.CurrentEncoding;
					base.Log.LogMessage(String.Format("Detected encoding: {0}", encoding));
					oldText = reader.ReadToEnd();
				}

				string newText = oldText.Replace(_textToReplace, _replacementText);
				if (oldText != newText)
				{
					base.Log.LogMessage(String.Format("Replacing '{0}' with '{1}' in file '{2}'", _textToReplace, _replacementText, _filePath));

					using (StreamWriter writer = new StreamWriter(_filePath, false, encoding))
					{
						writer.Write(newText);
					}
				}
				else
				{
					base.Log.LogMessage(String.Format("The text to replace ('{0}') could not be found in file '{1}'", _textToReplace, _filePath));
				}
			}
			catch(Exception e)
			{
				base.Log.LogErrorFromException(e, true);
				return false;
			}

			return true;
		}
	}
}