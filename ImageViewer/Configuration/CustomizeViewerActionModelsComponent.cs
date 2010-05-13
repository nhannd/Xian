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
		private readonly NodePropertiesValidationPolicy _validationPolicy;
		private readonly Dictionary<XKeys, string> _keyStrokeMap;
		private readonly Dictionary<XMouseButtons, string> _initialMouseToolsMap;
		private readonly MultiValuedDictionary<XMouseButtons, string> _mouseButtonMap;
		private readonly MultiValuedDictionary<string, AbstractActionModelTreeLeafAction> _actionMap;
		private bool _updatingKeyStrokes = false;

		public CustomizeViewerActionModelsComponent(IImageViewer imageViewer)
		{
			_imageViewer = imageViewer;

			_keyStrokeMap = new Dictionary<XKeys, string>();
			_initialMouseToolsMap = new Dictionary<XMouseButtons, string>(5);
			_mouseButtonMap = new MultiValuedDictionary<XMouseButtons, string>(5);
			_actionMap = new MultiValuedDictionary<string, AbstractActionModelTreeLeafAction>();

			NodePropertiesValidationPolicy validationPolicy = new NodePropertiesValidationPolicy();
			validationPolicy.AddRule<AbstractActionModelTreeLeafAction>("CheckState", (n, value) => n.ActionId != typeof (CustomizeViewerActionModelTool).FullName + ":customize" || Equals(value, CheckState.Checked));
			validationPolicy.AddRule<AbstractActionModelTreeLeafAction>("CheckState", (n, value) => n.ActionId != "ClearCanvas.Desktop.Configuration.Tools.OptionsTool:show" || Equals(value, CheckState.Checked));
			validationPolicy.AddRule<AbstractActionModelTreeLeafAction>("ActiveMouseButtons", (n, value) => !Equals(value, XMouseButtons.None));
			validationPolicy.AddRule<AbstractActionModelTreeLeafClickAction, XKeys>("KeyStroke", this.ValidateClickActionKeyStroke);
			validationPolicy.AddRule<AbstractActionModelTreeLeafAction, bool>("InitiallyActive", this.ValidateMouseToolInitiallyActive);
			_validationPolicy = validationPolicy;

			_tabComponent = new TabComponentContainer();
			_tabComponent.CurrentPageChanged += OnTabComponentCurrentPageChanged;

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

		private static TValue GetValue<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
		{
			TValue value;
			if (dictionary.TryGetValue(key, out value))
				return value;
			return defaultValue;
		}

		private bool ValidateClickActionKeyStroke(AbstractActionModelTreeLeafClickAction node, XKeys keys)
		{
			// if we're just synchronizing key strokes due to another key stroke set action, short the validation request
			if (_updatingKeyStrokes)
				return true;

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
				IList<AbstractActionModelTreeLeafAction> actions = _actionMap[_keyStrokeMap[keys]];
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

		private void UpdateMouseToolMouseButton(AbstractActionModelTreeLeafAction node, XMouseButtons mouseButton)
		{
			if (_updatingKeyStrokes)
				return;

			_updatingKeyStrokes = true;
			try
			{
				// find the original mouse button to which the tool was assigned
				var oldMouseButton = _mouseButtonMap.FindKey(node.ActionId, XMouseButtons.Left);
				if (oldMouseButton == mouseButton)
					return;

				// if the tool was originally the initial tool for this button, remove it
				if (GetValue(_initialMouseToolsMap, oldMouseButton, string.Empty) == node.ActionId)
					_initialMouseToolsMap[oldMouseButton] = string.Empty;

				// unassign the tool from the mouse button
				_mouseButtonMap.Remove(oldMouseButton, node.ActionId);

				// update the mouse button map
				if (mouseButton != XMouseButtons.None && !_mouseButtonMap[mouseButton].Contains(node.ActionId))
					_mouseButtonMap.Add(mouseButton, node.ActionId);
			}
			finally
			{
				_updatingKeyStrokes = false;
			}
		}

		private bool ValidateMouseToolInitiallyActive(AbstractActionModelTreeLeafAction node, bool initiallyActive)
		{
			// if we're just synchronizing the value due to another update action, short the validation request
			if (_updatingKeyStrokes)
				return true;

			// find the mouse button to which this tool is assigned
			var mouseButton = _mouseButtonMap.FindKey(node.ActionId, XMouseButtons.Left);

			// check for presence of another initial tool for this button
			if (initiallyActive && GetValue(_initialMouseToolsMap, mouseButton, node.ActionId) != node.ActionId)
			{
				IList<AbstractActionModelTreeLeafAction> actions = _actionMap[_initialMouseToolsMap[mouseButton]];
				if (actions.Count > 0)
				{
					string message = string.Format(SR.MessageMouseButtonInitialToolAlreadyAssigned, XMouseButtonsConverter.Format(mouseButton), actions[0].Label);
					DialogBoxAction result = base.Host.DesktopWindow.ShowMessageBox(message, MessageBoxActions.YesNo);
					if (result != DialogBoxAction.Yes)
						return false;
				}
			}

			return true;
		}

		private void UpdateMouseToolInitiallyActive(AbstractActionModelTreeLeafAction node, bool initiallyActive)
		{
			if (_updatingKeyStrokes)
				return;

			_updatingKeyStrokes = true;
			try
			{
				// find the mouse button to which this tool is assigned
				var mouseButton = _mouseButtonMap.FindKey(node.ActionId, XMouseButtons.Left);

				// check if updating this value actually causes a change
				var oldInitiallyActive = GetValue(_initialMouseToolsMap, mouseButton, string.Empty) == node.ActionId;
				if (oldInitiallyActive == initiallyActive)
					return;

				// update the initial tool map
				_initialMouseToolsMap[mouseButton] = initiallyActive ? node.ActionId : string.Empty;
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

			MouseToolSettingsProfile toolProfile = MouseToolSettingsProfile.Current.Clone();
			foreach (KeyValuePair<XMouseButtons, IEnumerable<string>> pair in _mouseButtonMap)
			{
				foreach (string actionId in pair.Value)
				{
					if (toolProfile.HasEntryByActivationActionId(actionId))
					{
						var setting = toolProfile.GetEntryByActivationActionId(actionId);
						setting.MouseButton = pair.Key;
						setting.InitiallyActive = GetValue(_initialMouseToolsMap, pair.Key, string.Empty) == actionId;
					}
				}
			}
			MouseToolSettingsProfile.Current = toolProfile;
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
			private readonly MouseToolSettingsProfile _toolProfile;

			public ImageViewerActionModelConfigurationComponent(string @namespace, string site, CustomizeViewerActionModelsComponent owner)
				: base(@namespace, site, owner._imageViewer.ExportedActions, owner._imageViewer.DesktopWindow, site == "global-toolbars")
			{
				_owner = owner;

				// just keep a single copy of it for a consistent startup state - we don't store the unsaved changes in here
				_toolProfile = MouseToolSettingsProfile.Current.Clone();

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
								clickActionNode.KeyStroke = XKeys.None;
							else
								_owner._keyStrokeMap.Add(clickActionNode.KeyStroke, clickActionNode.ActionId);
						}
					}

					if (_toolProfile.HasEntryByActivationActionId(node.ActionId))
					{
						var mouseToolSetting = _toolProfile.GetEntryByActivationActionId(node.ActionId);
						var mouseButtonValue = mouseToolSetting.MouseButton.GetValueOrDefault(XMouseButtons.Left);
						if (mouseButtonValue != XMouseButtons.None)
						{
							if (mouseToolSetting.InitiallyActive.GetValueOrDefault(false))
							{
								if (GetValue(_owner._initialMouseToolsMap, mouseButtonValue, node.ActionId) != node.ActionId)
									mouseToolSetting.InitiallyActive = false;
								else
									_owner._initialMouseToolsMap[mouseButtonValue] = node.ActionId;
							}
							if (!_owner._mouseButtonMap[mouseButtonValue].Contains(node.ActionId))
								_owner._mouseButtonMap.Add(mouseButtonValue, node.ActionId);
						}
					}
				}

				foreach (string actionId in base.ActionNodeMap.ActionIds)
					_owner._actionMap.AddRange(actionId, base.ActionNodeMap[actionId]);
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

				if (node is AbstractActionModelTreeLeafAction)
				{
					AbstractActionModelTreeLeafAction actionNode = (AbstractActionModelTreeLeafAction) node;
					if (propertyName == "InitiallyActive")
					{
						_owner.UpdateMouseToolInitiallyActive(actionNode, (bool) value);
					}
					else if (propertyName == "ActiveMouseButtons")
					{
						_owner.UpdateMouseToolMouseButton(actionNode, (XMouseButtons) value);
					}
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
					if (_toolProfile.IsRegisteredMouseToolActivationAction(actionId))
					{
						var activeMouseButtons = _owner._mouseButtonMap.FindKey(actionId, XMouseButtons.Left);
						var globalMouseButtons = XMouseButtons.None;
						var globalModifiers = ModifierFlags.None;
						var initiallyActive = GetValue(_owner._initialMouseToolsMap, activeMouseButtons, string.Empty) == actionId;
						yield return new MouseImageViewerToolPropertyComponent(node,
						                                                       activeMouseButtons,
						                                                       globalMouseButtons,
						                                                       globalModifiers,
						                                                       initiallyActive);
					}
				}
			}
		}
	}
}