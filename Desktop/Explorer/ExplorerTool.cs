#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using System.Collections.Generic;

namespace ClearCanvas.Desktop.Explorer
{
	[ExtensionPoint()]
	public class HealthcareArtifactExplorerExtensionPoint : ExtensionPoint<IHealthcareArtifactExplorer>
	{
	}

	//[MenuAction("show", "global-menus/MenuFile/MenuExplorer", "Show", KeyStroke = XKeys.Control | XKeys.S)]
	//[ButtonAction("show", "global-toolbars/ToolbarStandard/ToolbarExplorer", "Show")]
	//[Tooltip("show", "TooltipExplorer")]
	//[IconSet("show", IconScheme.Colour, "Icons.ExplorerToolSmall.png", "Icons.ExplorerToolMedium.png", "Icons.ExplorerToolLarge.png")]
	//[GroupHint("show", "Application.Browsing.Explorer")]

    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class ExplorerTool : Tool<IDesktopToolContext>
	{
		TabComponentContainer _tabComponentContainer;
		List<IHealthcareArtifactExplorer> _healthcareArtifactExplorers;
		static IWorkspace _workspace;

		public ExplorerTool()
		{
		}

		public override void Initialize()
		{
			Show();
			base.Initialize();
		}

		public void Show()
		{
			// We only ever want one explorer
			if (_workspace != null)
				return;

			_tabComponentContainer = new TabComponentContainer();

			HealthcareArtifactExplorerExtensionPoint xp = new HealthcareArtifactExplorerExtensionPoint();
			object[] extensions = xp.CreateExtensions();

			if (_healthcareArtifactExplorers == null)
			{
				_healthcareArtifactExplorers = new List<IHealthcareArtifactExplorer>();

				foreach (IHealthcareArtifactExplorer explorer in extensions)
				{
					_healthcareArtifactExplorers.Add(explorer);
				}
			}

			foreach (IHealthcareArtifactExplorer explorer in _healthcareArtifactExplorers)
			{
				TabPage tabPage = new TabPage(explorer.Name, explorer.Component);
				_tabComponentContainer.Pages.Add(tabPage);
			}

			_workspace = ApplicationComponent.LaunchAsWorkspace(
				this.Context.DesktopWindow,
				_tabComponentContainer,
				SR.TitleExplorer,
				delegate
				{
					_workspace = null;
					_tabComponentContainer = null;
					_healthcareArtifactExplorers.Clear();
					_healthcareArtifactExplorers = null;
					Application.Quit();
				});

			//_workspace.NeverClose = true;
		}
	}
}
