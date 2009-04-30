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
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace ClearCanvas.Utilities.BuildTasks
{
	public class CombineStrings : Task
	{
		private string[] _inputStrings;
		private string _delimiter;
		private string _combinedString;

		[Required]
		public string[] InputStrings
		{
			get { return _inputStrings; }
			set { _inputStrings = value; }
		}

		public string Delimiter
		{
			get { return _delimiter; }
			set { _delimiter = value; }
		}

		[Output]
		public string CombinedString
		{
			get { return _combinedString; }
		}

		public override bool Execute()
		{
			if (_inputStrings == null)
				return false;

			StringBuilder builder = new StringBuilder();
			int i = 0;
			foreach (string str in _inputStrings)
			{
				if (String.IsNullOrEmpty(str))
					continue;

				if (i++ > 0)
					builder.Append(_delimiter ?? "");
				builder.Append(str);
			}

			_combinedString = builder.ToString();

			return true;
		}
	}
}
