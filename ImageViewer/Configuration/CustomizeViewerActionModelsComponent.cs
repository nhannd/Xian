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
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Configuration.ActionModel;
using ClearCanvas.Desktop.Trees;
using ClearCanvas.ImageViewer.BaseTools;

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
		private readonly MouseToolSettingsProfile _toolProfile;
		private readonly NodePropertiesValidationPolicy _validationPolicy;
		private readonly Dictionary<XKeys, string> _keyStrokeMap;
		private readonly Dictionary<string, List<AbstractActionModelTreeLeafAction>> _actionMap;
		private bool _updatingKeyStrokes = false;

		public CustomizeViewerActionModelsComponent(IImageViewer imageViewer)
		{
			_imageViewer = imageViewer;
			_toolProfile = MouseToolSettingsProfile.Current.Clone();

			_keyStrokeMap = new Dictionary<XKeys, string>();
			_actionMap = new Dictionary<string, List<AbstractActionModelTreeLeafAction>>();

			NodePropertiesValidationPolicy validationPolicy = new NodePropertiesValidationPolicy();
			validationPolicy.AddRule<AbstractActionModelTreeLeafAction>("CheckState", (n, value) => n.ActionId != typeof (CustomizeViewerActionModelTool).FullName + ":customize" || Equals(value, CheckState.Checked));
			validationPolicy.AddRule<AbstractActionModelTreeLeafAction>("CheckState", (n, value) => n.ActionId != "ClearCanvas.Desktop.Configuration.Tools.OptionsTool:show" || Equals(value, CheckState.Checked));
			validationPolicy.AddRule<AbstractActionModelTreeLeafClickAction>("KeyStroke", this.ValidateClickActionKeyStroke);
			_validationPolicy = validationPolicy;

			_tabComponent = new TabComponentContainer();
			_tabComponent.CurrentPageChanged += new EventHandler(OnTabComponentCurrentPageChanged);

			_tabComponent.Pages.Add(new TabPage(SR.LabelToolbar, new ImageViewerActionModelConfigurationComponent(
			                                                     	_imageViewer.GlobalActionsNamespace,
			                                                     	"global-toolbars",
			                                                     	this)));

			_tabComponent.Pages.Add(new TabPage(SR.LabelContextMenu, new ImageViewerActionModelConfigurationComponent(
			                                                         	_imageViewer.ActionsNamespace,
			                                                         	"imageviewer-contextmenu",
			                                                         	this)));

			_tabComponent.Pages.Add(new TabPage(SR.LabelMainMenu, new ImageViewerActionModelConfigurationComponent(
			                                                      	_imageViewer.GlobalActionsNamespace,
			                                                      	"global-menus",
			                                                      	this)));

			_tabComponentHost = new ContainedComponentHost(this, _tabComponent);
		}

		private bool ValidateClickActionKeyStroke(AbstractActionModelTreeLeafClickAction node, object value)
		{
			// if we're just synchronizing key strokes due to another key stroke set action, short the validation request
			if (_updatingKeyStrokes)
				return true;

			XKeys keys = (XKeys) value;

			// if the key stroke is the empty value, it is always allowed
			if (keys == XKeys.None)
				return true;

			// if the key stroke contains only modifiers and no key, it is never allowed
			if ((keys & XKeys.Modifiers) != 0 && (keys & XKeys.KeyCode) == 0)
				return false;

			// if the action is not part of the viewer component then it is handled by the desktop and must be modified
			if (GetActionsById(_imageViewer.ExportedActions, node.ActionId).Count == 0 && (keys & XKeys.Modifiers) == 0)
				return false;

			// check for other assignments to the same key stroke and confirm the action if there are pre-existing assignments
			if (_keyStrokeMap.ContainsKey(keys) && _keyStrokeMap[keys] != node.ActionId)
			{
				List<AbstractActionModelTreeLeafAction> actions = _actionMap[_keyStrokeMap[keys]];
				if (actions.Count > 0)
				{
					string message = string.Format(SR.MessageKeyStrokeAlreadyAssigned, XKeysConverter.Format(keys), actions[0].Label);
					DialogBoxAction result = base.Host.DesktopWindow.ShowMessageBox(message, MessageBoxActions.YesNo);
					if (result != DialogBoxAction.Yes)
						return false;
				}
			}

			return true;
		}

		private void UpdateClickActionKeyStroke(AbstractActionModelTreeLeafClickAction node, XKeys keys)
		{
			if (_updatingKeyStrokes)
				return;

			_updatingKeyStrokes = true;
			try
			{
				// unassign the key stroke from the old actions if it has previously been assigned to something else
				if (keys != XKeys.None && _keyStrokeMap.ContainsKey(keys) && _keyStrokeMap[keys] != node.ActionId)
				{
					foreach (AbstractActionModelTreeLeafAction action in  _actionMap[_keyStrokeMap[keys]])
					{
						if (action is AbstractActionModelTreeLeafClickAction)
							((AbstractActionModelTreeLeafClickAction) action).KeyStroke = XKeys.None;
					}
				}

				// assign the key stroke to the new actions
				foreach (AbstractActionModelTreeLeafAction action in _actionMap[node.ActionId])
				{
					if (action is AbstractActionModelTreeLeafClickAction)
						((AbstractActionModelTreeLeafClickAction) action).KeyStroke = keys;
				}

				// update the key stroke map
				if (keys != XKeys.None)
					_keyStrokeMap[keys] = node.ActionId;
			}
			finally
			{
				_updatingKeyStrokes = false;
			}
		}

		// reset all node propery bindings because the values may have changed while on another page (due to key/mouse assignment mappings)
		private void OnTabComponentCurrentPageChanged(object sender, EventArgs e)
		{
			ImageViewerActionModelConfigurationComponent component = (ImageViewerActionModelConfigurationComponent) _tabComponent.CurrentPage.Component;
			component.ResetNodeProperties();
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

			MouseToolSettingsProfile.Current = _toolProfile;
			MouseToolSettingsProfile.SaveCurrentAsDefault();

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

		private static IActionSet GetActionsById(IActionSet actionSet, string actionId)
		{
			return actionSet.Select(a => a.ActionID == actionId);
		}

		private class ImageViewerActionModelConfigurationComponent : ActionModelConfigurationComponent
		{
			private readonly CustomizeViewerActionModelsComponent _owner;

			public ImageViewerActionModelConfigurationComponent(string @namespace, string site, CustomizeViewerActionModelsComponent owner)
				: base(@namespace, site, owner._imageViewer.ExportedActions, owner._imageViewer.DesktopWindow, site == "global-toolbars")
			{
				_owner = owner;

				this.ValidationPolicy = _owner._validationPolicy;

				// update the keystroke and action maps
				foreach (AbstractActionModelTreeLeafAction node in base.ActionNodeMap.ActionNodes)
				{
					if (node is AbstractActionModelTreeLeafClickAction)
					{
						AbstractActionModelTreeLeafClickAction clickActionNode = (AbstractActionModelTreeLeafClickAction) node;
						if (clickActionNode.KeyStroke != XKeys.None)
						{
							if (_owner._keyStrokeMap.ContainsKey(clickActionNode.KeyStroke))
							{
								clickActionNode.KeyStroke = XKeys.None;
							}
							else
							{
								_owner._keyStrokeMap.Add(clickActionNode.KeyStroke, clickActionNode.ActionId);
							}
						}
					}
				}

				foreach (string actionId in base.ActionNodeMap.ActionIds)
				{
					if (!_owner._actionMap.ContainsKey(actionId))
						_owner._actionMap.Add(actionId, new List<AbstractActionModelTreeLeafAction>());
					_owner._actionMap[actionId].AddRange(base.ActionNodeMap[actionId]);
				}
			}

			public void ResetNodeProperties()
			{
				this.OnSelectedNodeChanged();
			}

			protected override void OnNodePropertiesValidated(AbstractActionModelTreeNode node, string propertyName, object value)
			{
				base.OnNodePropertiesValidated(node, propertyName, value);

				if (propertyName == "KeyStroke" && node is AbstractActionModelTreeLeafClickAction)
				{
					_owner.UpdateClickActionKeyStroke((AbstractActionModelTreeLeafClickAction) node, (XKeys) value);
				}
			}

			protected override IEnumerable<NodePropertiesComponent> CreateNodePropertiesComponents(AbstractActionModelTreeNode node)
			{
				foreach (NodePropertiesComponent c in base.CreateNodePropertiesComponents(node))
				{
					yield return c;
				}

				if (node is AbstractActionModelTreeLeafClickAction)
				{
					string actionId = ((AbstractActionModelTreeLeafClickAction) node).ActionId;
					if (_owner._toolProfile.IsRegisteredMouseToolActivationAction(actionId))
					{
						yield return new MouseImageViewerToolPropertyComponent((AbstractActionModelTreeLeafClickAction) node, _owner._toolProfile);
					}
				}
			}
		}
	}
}