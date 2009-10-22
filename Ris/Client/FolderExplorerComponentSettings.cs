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
using System.Configuration;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Provides services for storing the folder and folder system structure 
	/// to an XML document, and rebuilding that folder structure from the document.
	/// </summary>
	[SettingsGroupDescription("Configures folder systems.")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	internal sealed partial class FolderExplorerComponentSettings
	{
		private readonly IFolderExplorerConfiguration _defaultConfig;
		private readonly IFolderExplorerUserConfiguration _userConfig;

		private FolderExplorerComponentSettings()
		{
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);

			_defaultConfig = new FolderExplorerConfiguration(() => this.DefaultConfigurationXml);
			_userConfig = new FolderExplorerUserConfiguration(() => this.UserConfigurationXml, updatedUserConfigurationXml =>
				{
					this.UserConfigurationXml = updatedUserConfigurationXml;
					Save();
				});
		}

		#region Public API

		/// <summary>
		/// Orders the folder systems according to the default and user specific settings
		/// </summary>
		/// <param name="folderSystems">Input list of folder systems</param>
		public IEnumerable<IFolderSystem> ApplyUserFolderSystemsOrder(IEnumerable<IFolderSystem> folderSystems)
		{
			folderSystems = _defaultConfig.ApplyUserFolderSystemsOrder(folderSystems);
			return _userConfig.ApplyUserFolderSystemsOrder(folderSystems);
		}

		/// <summary>
		/// Customizes the folders in the specified folder system according to the default and user specific settings
		/// </summary>
		/// <param name="folderSystem"></param>
		public IEnumerable<IFolder> ApplyUserFoldersCustomizations(IFolderSystem folderSystem)
		{
			var folders = _defaultConfig.ApplyUserFoldersCustomizations(folderSystem.Id, folderSystem.Folders);
			return _userConfig.ApplyUserFoldersCustomizations(folderSystem.Id, folders);
		}

		public delegate void UpdateFolderExplorerUserConfigurationAction(IFolderExplorerUserConfigurationUpdater userConfiguration);

		/// <summary>
		/// Allows user configuration to be stored.  The update action should be used to invoke methods on the 
		/// <see cref="IFolderExplorerUserConfigurationUpdater"/> to set the required user configuration.
		/// </summary>
		/// <param name="updateAction"></param>
		public void UpdateUserConfiguration(UpdateFolderExplorerUserConfigurationAction updateAction)
		{
			_userConfig.BeginTransaction();

			try
			{
				updateAction(_userConfig);
				_userConfig.CommitTransaction();
			}
			catch
			{
				_userConfig.RollbackTransaction();
				throw;
			}
		}

		/// <summary>
		/// Indicates that the current user's folder/folder system customizations have been committed.
		/// </summary>
		public event EventHandler UserConfigurationSaved
		{
			add { _userConfig.ChangesCommitted += value; }
			remove { _userConfig.ChangesCommitted -= value; }
		}

		public bool IsFolderSystemReadOnly(IFolderSystem folderSystem)
		{
			return _defaultConfig.IsFolderSystemReadOnly(folderSystem);
		}

		#endregion
	}
}
