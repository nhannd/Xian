#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

using System.IO;
using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Common.CommandProcessor
{
	public class CopyFileCommand : ServerCommand
	{
		#region Private Members
		private readonly string _sourceFile;
		private readonly string _destinationFile;
		#endregion

		public CopyFileCommand(string sourceFile, string destinationFile)
			: base("Copy File", true)
		{
			Platform.CheckForNullReference(sourceFile, "Source filename");
			Platform.CheckForNullReference(destinationFile, "Destination filename");

			_sourceFile = sourceFile;
			_destinationFile = destinationFile;
		}

		protected override void OnExecute()
		{
			if (File.Exists(_destinationFile))
			{
				File.Delete(_destinationFile);
			}

			File.Copy(_sourceFile, _destinationFile);
		}

		protected override void OnUndo()
		{
			if (File.Exists(_destinationFile))
				File.Delete(_destinationFile);
		}
	}
}
