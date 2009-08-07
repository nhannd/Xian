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
using System.Drawing;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.Volume.Mpr.Utilities
{
	public abstract class MouseImageViewerToolMaster<T> : MouseImageViewerTool where T : MouseImageViewerTool
	{
		private IToolSet _toolSet;
		private IActionSet _actionSet;
		private T _selectedTool;

		protected MouseImageViewerToolMaster() {}

		protected T SelectedTool
		{
			get { return _selectedTool; }
			private set
			{
				if (_selectedTool != value)
				{
					if (_selectedTool != null)
						this.OnToolUnselected(_selectedTool);

					_selectedTool = value;

					if (_selectedTool != null)
						this.OnToolSelected(_selectedTool);

					this.Active = (_selectedTool != null);
				}
			}
		}

		protected virtual void OnToolSelected(T tool)
		{
			this.Behaviour = tool.Behaviour;
			this.DefaultMouseButtonShortcut = tool.DefaultMouseButtonShortcut;
			this.MouseButton = tool.MouseButton;
			this.MouseWheelShortcut = tool.MouseWheelShortcut;
			this.Select();
		}

		protected virtual void OnToolUnselected(T tool)
		{
			this.Behaviour = MouseButtonHandlerBehaviour.Default;
			this.DefaultMouseButtonShortcut = new MouseButtonShortcut(XMouseButtons.None);
			this.MouseButton = XMouseButtons.None;
			this.MouseWheelShortcut = new MouseWheelShortcut();
		}

		public override IActionSet Actions
		{
			get
			{
				if (_actionSet == null)
				{
					IActionSet actionSet = new ActionSet(base.Actions);
					foreach (T tool in _toolSet.Tools)
						actionSet = actionSet.Union(new ActionSet(CustomActionAttributeProcessor.Process(tool)));
					_actionSet = actionSet;
				}
				return _actionSet;
			}
		}

		protected abstract IEnumerable<T> CreateTools();

		protected virtual void OnToolInitialized(T tool)
		{
			tool.ActivationChanged += OnToolActivationChanged;
			tool.DefaultMouseButtonShortcutChanged += tool_DefaultMouseButtonShortcutChanged;
			tool.MouseButtonChanged += tool_MouseButtonChanged;
			tool.MouseWheelShortcutChanged += tool_MouseWheelShortcutChanged;
			tool.TooltipChanged += tool_TooltipChanged;
		}

		protected virtual void OnToolDisposing(T tool)
		{
			tool.TooltipChanged -= tool_TooltipChanged;
			tool.MouseWheelShortcutChanged -= tool_MouseWheelShortcutChanged;
			tool.MouseButtonChanged -= tool_MouseButtonChanged;
			tool.DefaultMouseButtonShortcutChanged -= tool_DefaultMouseButtonShortcutChanged;
			tool.ActivationChanged -= OnToolActivationChanged;
		}

		private void OnToolActivationChanged(object sender, EventArgs e)
		{
			this.SelectedTool = (T) sender;
			foreach (T tool in _toolSet.Tools)
			{
				if (tool != sender)
					tool.Active = false;
			}
		}

		private void tool_TooltipChanged(object sender, EventArgs e) {}

		private void tool_MouseWheelShortcutChanged(object sender, EventArgs e)
		{
			if (this.SelectedTool == sender)
				this.MouseWheelShortcut = this.SelectedTool.MouseWheelShortcut;
		}

		private void tool_MouseButtonChanged(object sender, EventArgs e)
		{
			if (this.SelectedTool == sender)
				this.MouseButton = this.SelectedTool.MouseButton;
		}

		private void tool_DefaultMouseButtonShortcutChanged(object sender, EventArgs e)
		{
			if (this.SelectedTool == sender)
				this.DefaultMouseButtonShortcut = this.SelectedTool.DefaultMouseButtonShortcut;
		}

		public override void Initialize()
		{
			base.Initialize();

			_toolSet = new ToolSet(this.CreateTools(), new ToolContext(this.ImageViewer));
			foreach (T tool in _toolSet.Tools)
				this.OnToolInitialized(tool);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_actionSet = null;

				if (_toolSet != null)
				{
					foreach (T tool in _toolSet.Tools)
						this.OnToolDisposing(tool);
					_toolSet.Dispose();
					_toolSet = null;
				}
			}

			base.Dispose(disposing);
		}

		protected override void OnActivationChanged()
		{
			base.OnActivationChanged();
			if (!this.Active)
				this.SelectedTool = null;
		}

		#region Mouse Input Forwarding

		public override void Cancel()
		{
			if (this.SelectedTool != null)
				this.SelectedTool.Cancel();
		}

		public override CursorToken GetCursorToken(Point point)
		{
			if (this.SelectedTool != null)
				return this.SelectedTool.GetCursorToken(point);
			return base.GetCursorToken(point);
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			if (this.SelectedTool != null)
				return this.SelectedTool.Start(mouseInformation);
			return base.Start(mouseInformation);
		}

		public override void StartWheel()
		{
			if (this.SelectedTool != null)
				this.SelectedTool.StartWheel();
		}

		public override bool Stop(IMouseInformation mouseInformation)
		{
			if (this.SelectedTool != null)
				return this.SelectedTool.Stop(mouseInformation);
			return base.Stop(mouseInformation);
		}

		public override void StopWheel()
		{
			if (this.SelectedTool != null)
				this.SelectedTool.StopWheel();
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			if (this.SelectedTool != null)
				return this.SelectedTool.Track(mouseInformation);
			return base.Track(mouseInformation);
		}

		public override void Wheel(int wheelDelta)
		{
			if (this.SelectedTool != null)
				this.SelectedTool.Wheel(wheelDelta);
		}

		#endregion

		#region ToolContext

		private class ToolContext : IImageViewerToolContext
		{
			private readonly IImageViewer _viewer;

			public ToolContext(IImageViewer viewer)
			{
				_viewer = viewer;
			}

			public IImageViewer Viewer
			{
				get { return _viewer; }
			}

			public IDesktopWindow DesktopWindow
			{
				get { return _viewer.DesktopWindow; }
			}
		}

		#endregion

		#region CustomActionAttributeProcessor

		private static class CustomActionAttributeProcessor
		{
			/// <summary>
			/// Processes the set of action attributes declared on a given target object to generate the
			/// corresponding set of <see cref="IAction"/> objects.
			/// </summary>
			/// <param name="actionTarget">The target object on which the attributes are declared, typically a tool.</param>
			/// <returns>The resulting set of actions, where each action is bound to the target object.</returns>
			public static IAction[] Process(object actionTarget)
			{
				object[] attributes = actionTarget.GetType().GetCustomAttributes(typeof (ActionAttribute), true);

				// first pass - create an ActionBuilder for each initiator of the specified type
				List<CustomActionBuildingContext> actionBuilders = new List<CustomActionBuildingContext>();
				foreach (ActionAttribute a in attributes)
				{
					if (a is ActionInitiatorAttribute)
					{
						CustomActionBuildingContext actionBuilder = new CustomActionBuildingContext(GetQualifiedActionID(a, actionTarget), actionTarget);
						a.Apply(actionBuilder);
						actionBuilders.Add(actionBuilder);
					}
				}

				// second pass - apply decorators to all ActionBuilders with same actionID
				foreach (ActionAttribute a in attributes)
				{
					if (a is ActionDecoratorAttribute)
					{
						foreach (CustomActionBuildingContext actionBuilder in actionBuilders)
						{
							if (GetQualifiedActionID(a, actionTarget) == actionBuilder.ActionID)
							{
								a.Apply(actionBuilder);
							}
						}
					}
				}

				List<IAction> actions = new List<IAction>();
				foreach (CustomActionBuildingContext actionBuilder in actionBuilders)
				{
					actions.Add(actionBuilder.Action);
				}

				return actions.ToArray();
			}

			private static string GetQualifiedActionID(ActionAttribute a, object actionTarget)
			{
				return string.Format("{0}_{1:x8}", a.QualifiedActionID(actionTarget), actionTarget.GetHashCode());
			}

			/// <summary>
			/// Default implementation of <see cref="IActionBuildingContext"/>.
			/// </summary>
			private class CustomActionBuildingContext : IActionBuildingContext
			{
				private readonly string _actionID;
				private readonly object _actionTarget;
				private readonly ResourceResolver _resolver;
				private Action _action;

				public CustomActionBuildingContext(string actionID, object actionTarget)
				{
					_actionID = actionID;
					_actionTarget = actionTarget;

					_resolver = new ActionResourceResolver(_actionTarget);
				}

				public string ActionID
				{
					get { return _actionID; }
				}

				public Action Action
				{
					get { return _action; }
					set { _action = value; }
				}

				public IResourceResolver ResourceResolver
				{
					get { return _resolver; }
				}

				public object ActionTarget
				{
					get { return _actionTarget; }
				}
			}
		}

		#endregion
	}
}