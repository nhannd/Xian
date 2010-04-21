#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Configuration.ActionModel;
using ClearCanvas.Desktop.Trees;

namespace ClearCanvas.ImageViewer.Configuration
{
	[ExtensionPoint]
	public sealed class CustomizeViewerActionModelsComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	[AssociateView(typeof (CustomizeViewerActionModelsComponentViewExtensionPoint))]
	public class CustomizeViewerActionModelsComponent : ApplicationComponentContainer
	{
		private readonly ContainedComponentHost _tabComponentHost;
		private readonly TabComponentContainer _tabComponent;
		private readonly IImageViewer _imageViewer;

		public CustomizeViewerActionModelsComponent(IImageViewer imageViewer)
		{
			_imageViewer = imageViewer;

			NodePropertiesValidationPolicy validationPolicy = new NodePropertiesValidationPolicy();
			validationPolicy.AddRule<AbstractActionModelTreeLeafAction>("CheckState", (n, value) => n.ActionId != typeof (CustomizeViewerActionModelTool).FullName + ":customize" || Equals(value, CheckState.Checked));
			validationPolicy.AddRule<AbstractActionModelTreeLeafAction>("CheckState", (n, value) => n.ActionId != "ClearCanvas.Desktop.Configuration.Tools.OptionsTool:show" || Equals(value, CheckState.Checked));
			validationPolicy.AddRule<AbstractActionModelTreeLeafClickAction>("KeyStroke", this.ValidateClickActionKeyStroke);

			_tabComponent = new TabComponentContainer();

			_tabComponent.Pages.Add(new TabPage(SR.LabelToolbar, new ActionModelConfigurationComponent(
			                                                     	_imageViewer.GlobalActionsNamespace,
			                                                     	"global-toolbars",
			                                                     	_imageViewer.ExportedActions,
			                                                     	_imageViewer.DesktopWindow,
			                                                     	true) {ValidationPolicy = validationPolicy}
			                        	));

			_tabComponent.Pages.Add(new TabPage(SR.LabelContextMenu, new ActionModelConfigurationComponent(
			                                                         	_imageViewer.ActionsNamespace,
			                                                         	"imageviewer-contextmenu",
			                                                         	_imageViewer.ExportedActions,
			                                                         	_imageViewer.DesktopWindow) {ValidationPolicy = validationPolicy}
			                        	));

			_tabComponent.Pages.Add(new TabPage(SR.LabelMainMenu, new ActionModelConfigurationComponent(
			                                                      	_imageViewer.GlobalActionsNamespace,
			                                                      	"global-menus",
			                                                      	_imageViewer.ExportedActions,
			                                                      	_imageViewer.DesktopWindow) {ValidationPolicy = validationPolicy}
			                        	));

			_tabComponentHost = new ContainedComponentHost(this, _tabComponent);
		}

		private bool ValidateClickActionKeyStroke(AbstractActionModelTreeLeafClickAction node, object value)
		{
			if (_imageViewer.ExportedActions.Select(a => a.ActionID == node.ActionId).Count > 0)
				return true;
			XKeys keys = (XKeys) value;
			return ((keys & XKeys.Modifiers) != 0);
		}

		/// <summary>
		/// The host object for the contained <see cref="IApplicationComponent"/>.
		/// </summary>
		public ApplicationComponentHost TabComponentHost
		{
			get { return _tabComponentHost; }
		}

		public override void Start()
		{
			base.Start();
			_tabComponentHost.StartComponent();
		}

		public override void Stop()
		{
			_tabComponentHost.StopComponent();
			base.Stop();
		}

		public void Accept()
		{
			if (this.HasValidationErrors)
			{
				this.ShowValidation(true);
				return;
			}

			try
			{
				foreach (ActionModelConfigurationComponent component in _tabComponent.ContainedComponents)
				{
					component.Save();
				}
			}
			catch (Exception ex)
			{
				ExceptionHandler.Report(ex, this.Host.DesktopWindow);
			}

			base.Exit(ApplicationComponentExitCode.Accepted);
		}

		public void Cancel()
		{
			base.Exit(ApplicationComponentExitCode.None);
		}

		/// <summary>
		/// Gets a value indicating whether there are any data validation errors.
		/// </summary>
		public override bool HasValidationErrors
		{
			get { return _tabComponent.HasValidationErrors || base.HasValidationErrors; }
		}

		/// <summary>
		/// Sets the <see cref="ApplicationComponent.ValidationVisible"/> property and raises the 
		/// <see cref="ApplicationComponent.ValidationVisibleChanged"/> event.
		/// </summary>
		public override void ShowValidation(bool show)
		{
			base.ShowValidation(show);
			_tabComponent.ShowValidation(show);
		}

		public override IEnumerable<IApplicationComponent> ContainedComponents
		{
			get { yield return _tabComponent; }
		}

		public override IEnumerable<IApplicationComponent> VisibleComponents
		{
			get { yield return _tabComponent; }
		}

		public override void EnsureVisible(IApplicationComponent component)
		{
			// contained component cannot be made invisible
		}

		public override void EnsureStarted(IApplicationComponent component)
		{
			// contained component is always started as long as container is started
		}
	}
}