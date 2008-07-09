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

using ClearCanvas.Common;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.Archiving.Hsm
{
	/// <summary>
	/// HSM Based archive plugin.
	/// </summary>
	[ExtensionOf(typeof(ImageServerArchiveExtensionPoint))]
	public class HsmArchive : ImageServerArchiveBase
	{
		private HsmArchiveService _archiveService;
		private HsmRestoreService _restoreService;
	
		public override ArchiveTypeEnum ArchiveType
		{
			get { return ArchiveTypeEnum.HsmArchive; }
		}

		public override void Start(PartitionArchive archive)
		{
			_partitionArchive = archive;

			_restoreService = new HsmRestoreService("HSM Restore", archive, this);
			_restoreService.StartService();

			if (!_partitionArchive.ReadOnly)
			{
				_archiveService = new HsmArchiveService("HSM Archive", archive, this);	
				_archiveService.StartService();
			}			
		}

		public override void Stop()
		{
			if (_restoreService != null)
			{
				_restoreService.StopService();
				_restoreService = null;
			}

			if (_archiveService != null)
			{
				_archiveService.StopService();
				_archiveService = null;
			}
		}
	}
}
