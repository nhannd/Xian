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