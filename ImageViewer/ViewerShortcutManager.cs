#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer
{
	internal sealed class ViewerShortcutManager : IViewerShortcutManager
	{
		private readonly Dictionary<ITool, ITool> _setRegisteredTools;

		private readonly ImageViewerComponent _ownerViewer;

		public ViewerShortcutManager(ImageViewerComponent ownerViewer)
		{
			_setRegisteredTools = new Dictionary<ITool, ITool>();
			_ownerViewer = ownerViewer;
		}

		private void RegisterMouseToolButton(MouseImageViewerTool mouseTool)
		{
			if (mouseTool.Active)
				ActivateMouseTool(mouseTool, false);

			mouseTool.MouseButtonChanged += new EventHandler(OnMouseToolMouseButtonChanged);
			mouseTool.ActivationChanged += new EventHandler(OnMouseToolActivationChanged);
		}

		private MouseImageViewerTool GetActiveMouseTool(XMouseButtons button)
		{
			foreach (ITool tool in _setRegisteredTools.Keys)
			{
				MouseImageViewerTool mouseTool = tool as MouseImageViewerTool;
				if (mouseTool != null)
				{
					if (mouseTool.Active && mouseTool.MouseButton == button)
						return mouseTool;
				}
			}

			return null;
		}

		private void DeactivateMouseTools(MouseImageViewerTool activating)
		{
			foreach (ITool tool in _setRegisteredTools.Keys)
			{
				MouseImageViewerTool mouseTool = tool as MouseImageViewerTool;
				if (mouseTool != null)
				{
					if (mouseTool.Active && mouseTool != activating && mouseTool.MouseButton == activating.MouseButton)
						mouseTool.Active = false;
				}
			}
		}

		private void ActivateMouseTool(MouseImageViewerTool activateMouseTool, bool replaceExisting)
		{
			if (activateMouseTool.MouseButton == XMouseButtons.None)
			{
				Platform.Log(LogLevel.Error, String.Format(SR.FormatMouseToolHasNoAssignment, activateMouseTool.GetType().FullName));
				activateMouseTool.Active = false;
				return;
			}

			MouseImageViewerTool current = GetActiveMouseTool(activateMouseTool.MouseButton);
			if (!replaceExisting && current != null && current != activateMouseTool)
			{
				activateMouseTool.Active = false;
				return;
			}

			DeactivateMouseTools(activateMouseTool);
		}

		private void OnMouseToolActivationChanged(object sender, EventArgs e)
		{
			MouseImageViewerTool mouseTool = (MouseImageViewerTool)sender;

			if (mouseTool.Active)
				ActivateMouseTool(mouseTool, true);
		}

		private void OnMouseToolMouseButtonChanged(object sender, EventArgs e)
		{
			MouseImageViewerTool mouseTool = (MouseImageViewerTool)sender;

			if (mouseTool.Active)
				ActivateMouseTool(mouseTool, true);
		}

		/// <summary>
		/// Registers the tool with the viewer shortcut manager.
		/// </summary>
		/// <param name="tool">the tool to register.</param>
		public void RegisterImageViewerTool(ITool tool)
		{
			Platform.CheckForNullReference(tool, "tool");

			if (tool is MouseImageViewerTool)
				RegisterMouseToolButton((MouseImageViewerTool)tool);

			_setRegisteredTools[tool] = tool;
		}

		private IMouseButtonHandler GetRegisteredMouseButtonHandler(MouseButtonShortcut shortcut)
		{
			if (shortcut == null)
				return null;

			foreach (ITool tool in _setRegisteredTools.Keys)
			{
				MouseImageViewerTool mouseButtonHandler = tool as MouseImageViewerTool;
				if (mouseButtonHandler != null)
				{
					//Active mouse button assignments take precedence over inactive ones.
					if (mouseButtonHandler.Active && shortcut.Equals(mouseButtonHandler.MouseButton))
						return mouseButtonHandler;
				}
			}

			return null;
		}

		#region IViewerShortcutManager Members

		public IEnumerable<IMouseButtonHandler> GetMouseButtonHandlers(MouseButtonShortcut shortcut)
		{
			IMouseButtonHandler registeredHandler = GetRegisteredMouseButtonHandler(shortcut);
			if (registeredHandler != null)
				yield return registeredHandler;

			foreach (ITool tool in _setRegisteredTools.Keys)
			{
				MouseImageViewerTool mouseButtonHandler = tool as MouseImageViewerTool;
				if (mouseButtonHandler != null)
				{
					if (shortcut.Equals(mouseButtonHandler.DefaultMouseButtonShortcut))
						yield return mouseButtonHandler;
				}
			}
		}

		/// <summary>
		/// Gets the <see cref="IMouseWheelHandler"/> associated with a shortcut.
		/// </summary>
		/// <param name="shortcut">The shortcut for which an <see cref="IMouseWheelHandler"/> is to be found.</param>
		/// <returns>An <see cref="IMouseWheelHandler"/> or null.</returns>
		public IMouseWheelHandler GetMouseWheelHandler(MouseWheelShortcut shortcut)
		{
			if (shortcut == null)
				return null;

			foreach (ITool tool in _setRegisteredTools.Keys)
			{
				MouseImageViewerTool viewerTool = tool as MouseImageViewerTool;
				if (viewerTool != null)
				{
					if (shortcut.Equals(viewerTool.MouseWheelShortcut))
						return viewerTool;
				}
			}

			return null;
		}

		/// <summary>
		/// Gets the <see cref="IClickAction"/> associated with a shortcut.
		/// </summary>
		/// <param name="shortcut">The shortcut for which an <see cref="IClickAction"/> is to be found.</param>
		/// <returns>An <see cref="IClickAction"/> or null.</returns>
		public IClickAction GetKeyboardAction(KeyboardButtonShortcut shortcut)
		{
			const string globalMenusActionSite = "global-menus";
			const string globalToolbarActionSite = "global-toolbars";

			if (shortcut == null)
				return null;

			var actions = (IActionSet) new ActionSet();
			foreach (var tool in _setRegisteredTools.Keys)
				actions = actions.Union(tool.Actions);

			// precedence is given to actions on the viewer keyboard site
			var action = FindClickAction(shortcut, _ownerViewer.ActionsNamespace, ImageViewerComponent.KeyboardSite, actions);

			// if not defined, give consideration to the viewer context menu
			if (action == null)
				action = FindClickAction(shortcut, _ownerViewer.ActionsNamespace, ImageViewerComponent.ContextMenuSite, actions);

			// if still not found, search the global toolbars
			if (action == null)
				action = FindClickAction(shortcut, _ownerViewer.GlobalActionsNamespace, globalToolbarActionSite, actions);

			// then the global menus
			if (action == null)
				action = FindClickAction(shortcut, _ownerViewer.GlobalActionsNamespace, globalMenusActionSite, actions);

			// and finally any other site in our toolset that hasn't already been covered
			// it's done in this way so that we don't unnecessarily execute the collection mapping and unique finding,
			// as most actions will be on one of the explicitly searched sites
			if (action == null)
			{
				foreach (var site in CollectionUtils.Unique(CollectionUtils.Map<IAction, string>(actions, a => a.Path.Site)))
				{
					if (site != globalMenusActionSite
					    && site != globalToolbarActionSite
					    && site != ImageViewerComponent.KeyboardSite
					    && site != ImageViewerComponent.ContextMenuSite)
					{
						action = FindClickAction(shortcut, _ownerViewer.ActionsNamespace, site, actions);
						if (action != null)
							break;
					}
				}
			}

			return action;
		}

		private static IClickAction FindClickAction(KeyboardButtonShortcut shortcut, string @namespace, string site, IActionSet actionSet)
		{
			var actions = ActionModelRoot.CreateModel(@namespace, site, actionSet).GetActionsInOrder();
			foreach (var action in actions)
			{
				var clickAction = action as IClickAction;
				if (clickAction != null && shortcut.Equals(clickAction.KeyStroke))
						return clickAction;
			}

			return null;
		}

		#endregion
	}
}
