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

using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Adt
{
	[ExtensionPoint]
	public class TechnologistMainWorkflowFolderExtensionPoint : ExtensionPoint<IFolder>
	{
	}

	[ExtensionPoint]
	public class TechnologistMainWorkflowItemToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[ExtensionPoint]
	public class TechnologistMainWorkflowFolderToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	public class TechnologistMainWorkflowFolderSystem
		: TechnologistWorkflowFolderSystemBase<TechnologistMainWorkflowFolderExtensionPoint, TechnologistMainWorkflowFolderToolExtensionPoint,
			TechnologistMainWorkflowItemToolExtensionPoint>, ISearchDataHandler
	{
		private readonly Folders.TechnologistSearchFolder _searchFolder;

		public TechnologistMainWorkflowFolderSystem(IFolderExplorerToolContext folderExplorer)
			: base(SR.TitlePerformingFolderSystem, folderExplorer)
		{
			this.ResourceResolver = new ResourceResolver(this.GetType().Assembly, this.ResourceResolver);

			if (Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Development.ViewUnfilteredWorkflowFolders))
			{
				this.AddFolder(new Folders.ScheduledTechnologistWorkflowFolder(this));
				this.AddFolder(new Folders.CheckedInTechnologistWorkflowFolder(this));
				this.AddFolder(new Folders.InProgressTechnologistWorkflowFolder(this));
				this.AddFolder(new Folders.UndocumentedTechnologistWorkflowFolder(this));
				this.AddFolder(new Folders.CancelledTechnologistWorkflowFolder(this));
				this.AddFolder(new Folders.CompletedTechnologistWorkflowFolder(this));
			}
			this.AddFolder(_searchFolder = new Folders.TechnologistSearchFolder(this));
		}

		public override string PreviewUrl
		{
			get { return WebResourcesSettings.Default.TechnologistFolderSystemUrl; }
		}

		public override void SelectedItemDoubleClickedEventHandler(object sender, System.EventArgs e)
		{
			base.SelectedItemDoubleClickedEventHandler(sender, e);

			TechnologistDocumentationTool documentationTool = (TechnologistDocumentationTool)CollectionUtils.SelectFirst(this.ItemTools.Tools,
				delegate(ITool tool) { return tool is TechnologistDocumentationTool; });

			if (documentationTool != null && documentationTool.Enabled)
				documentationTool.Apply();
		}

		#region ISearchDataHandler Members

		public SearchData SearchData
		{
			set
			{
				_searchFolder.SearchData = value;
				SelectedFolder = _searchFolder;
			}
		}

		#endregion
	}
}
