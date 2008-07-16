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

using System.Xml;
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
		private string _hsmPath;

		public string HsmPath
		{
			get { return _hsmPath; }
		}

		/// <summary>
		/// The <see cref="PartitionArchive"/> associated with the HsmArchive.
		/// </summary>
		public PartitionArchive PartitionArchive
		{
			get { return _partitionArchive; }
		}

		/// <summary>
		/// The Archive Type.
		/// </summary>
		public override ArchiveTypeEnum ArchiveType
		{
			get { return ArchiveTypeEnum.HsmArchive; }
		}

		/// <summary>
		/// Start the archive.
		/// </summary>
		/// <param name="archive">The <see cref="PartitionArchive"/> to start.</param>
		public override void Start(PartitionArchive archive)
		{
			_partitionArchive = archive;

			LoadServerPartition();

			_hsmPath = string.Empty;

			//Hsm Archive specific Xml data.
			XmlElement element = archive.ConfigurationXml.DocumentElement;
			foreach (XmlElement node in element.ChildNodes)
				if (node.Name.Equals("RootDir"))
					_hsmPath = node.InnerText;

			
			// Start the restore service
			_restoreService = new HsmRestoreService("HSM Restore", this);
			_restoreService.StartService();

			// If not "readonly", start the archive service.
			if (!_partitionArchive.ReadOnly)
			{
				_archiveService = new HsmArchiveService("HSM Archive", this);	
				_archiveService.StartService();
			}			
		}

		/// <summary>
		/// Stop the archive.
		/// </summary>
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
