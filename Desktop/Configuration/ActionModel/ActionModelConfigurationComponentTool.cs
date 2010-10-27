#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Desktop.Configuration.ActionModel
{
	[ExtensionPoint]
	public sealed class ActionModelConfigurationComponentToolExtensionPoint : ExtensionPoint<ITool> {}

	public interface IActionModelConfigurationComponentToolContext : IToolContext
	{
		ActionModelConfigurationComponent Component { get; }
		IDesktopWindow DesktopWindow { get; }
	}

	public abstract class ActionModelConfigurationComponentTool : Tool<IActionModelConfigurationComponentToolContext>
	{
		public override void Initialize()
		{
			base.Initialize();

			this.Component.SelectedNodeChanged += OnComponentSelectedNodeChanged;
		}

		private void OnComponentSelectedNodeChanged(object sender, EventArgs e)
		{
			this.OnSelectedNodeChanged();
		}

		protected override void Dispose(bool disposing)
		{
			this.Component.SelectedNodeChanged -= OnComponentSelectedNodeChanged;

			base.Dispose(disposing);
		}

		protected AbstractActionModelTreeNode SelectedNode
		{
			get { return this.Component.SelectedNode; }
		}

		protected ActionModelConfigurationComponent Component
		{
			get { return base.Context.Component; }
		}

		protected virtual void OnSelectedNodeChanged() {}
	}

	partial class ActionModelConfigurationComponent
	{
		protected class ActionModelConfigurationComponentToolContext : IActionModelConfigurationComponentToolContext
		{
			private readonly ActionModelConfigurationComponent _component;

			public ActionModelConfigurationComponentToolContext(ActionModelConfigurationComponent component)
			{
				_component = component;
			}

			public ActionModelConfigurationComponent Component
			{
				get { return _component; }
			}

			public IDesktopWindow DesktopWindow
			{
				get { return _component.Host.DesktopWindow; }
			}
		}
	}
}