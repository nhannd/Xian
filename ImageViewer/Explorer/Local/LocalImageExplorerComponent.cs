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
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Explorer;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.ImageViewer.Explorer.Local
{
	public interface ILocalImageExplorerToolContext : IToolContext
	{
		IEnumerable<string> SelectedPaths { get; }
		IDesktopWindow DesktopWindow { get; }
		ClickHandlerDelegate DefaultActionHandler { get; set; }
	}

	[ExtensionPoint()]
	public sealed class LocalImageExplorerToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	public sealed class LocalImageExplorerComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	public delegate IEnumerable<string> GetSelectedPathsDelegate();

	[AssociateView(typeof(LocalImageExplorerComponentViewExtensionPoint))]
	public class LocalImageExplorerComponent : ApplicationComponent
	{
		protected class LocalImageExplorerToolContext : ToolContext, ILocalImageExplorerToolContext
		{
			private LocalImageExplorerComponent _component;

			public LocalImageExplorerToolContext(LocalImageExplorerComponent component)
			{
				_component = component;
			}

			#region LocalImageExplorerToolContext Members

			public IEnumerable<string> SelectedPaths
			{
				get
				{
					if (_component._getSelectedPathsDelegate == null)
						return new List<string>(); //an empty list

					return _component._getSelectedPathsDelegate();
				}
			}

			public IDesktopWindow DesktopWindow
			{
				get
				{
					return _component.Host.DesktopWindow;
				}
			}

			public ClickHandlerDelegate DefaultActionHandler
			{
				get { return _component._defaultActionHandler; }
				set { _component._defaultActionHandler = value; }
			}

			#endregion
		}

		/// LocalImageExplorerComponent members

		private ToolSet _toolSet;
		private ClickHandlerDelegate _defaultActionHandler;
		private GetSelectedPathsDelegate _getSelectedPathsDelegate;

		public LocalImageExplorerComponent()
		{
		}

		protected ToolSet ToolSet
		{
			get { return _toolSet; }
			set { _toolSet = value; }
		}

		public ClickHandlerDelegate DefaultActionHandler
		{
			get { return _defaultActionHandler; }
			set { _defaultActionHandler = value; }
		}

		public GetSelectedPathsDelegate GetSelectedPathsDelegate
		{
			get { return _getSelectedPathsDelegate; }
			set { _getSelectedPathsDelegate = value; }
		}

		public void DefaultAction()
		{
			if (this.DefaultActionHandler != null)
				this.DefaultActionHandler();
		}

		public IDesktopWindow DesktopWindow
		{
			get { return base.Host.DesktopWindow; }
		}

		public ActionModelNode ContextMenuModel
		{
			get
			{
				return ActionModelRoot.CreateModel(this.GetType().FullName, "explorerlocal-contextmenu", ToolSet.Actions);
			}
		}

		public override void Start()
		{
			base.Start();
			ToolSet = new ToolSet(new LocalImageExplorerToolExtensionPoint(), new LocalImageExplorerToolContext(this));
		}

		public override void Stop()
		{
			base.Stop();
			ToolSet.Dispose();
			ToolSet = null;
		}
	}
}
