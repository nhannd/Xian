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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop.Configuration.ActionModel
{
	[ExtensionOf(typeof (NodePropertiesComponentProviderExtensionPoint))]
	public sealed class ClickActionKeystrokePropertyProvider : INodePropertiesComponentProvider
	{
		public IEnumerable<NodePropertiesComponent> CreateComponents(AbstractActionModelTreeNode selectedNode)
		{
			AbstractActionModelTreeLeafAction actionNode = selectedNode as AbstractActionModelTreeLeafAction;
			if (actionNode != null && actionNode.Action is IClickAction)
				yield return new ClickActionKeystrokePropertyComponent(actionNode);
		}
	}

	[ExtensionPoint]
	public sealed class ClickActionKeystrokePropertyComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	[AssociateView(typeof (ClickActionKeystrokePropertyComponentViewExtensionPoint))]
	public class ClickActionKeystrokePropertyComponent : NodePropertiesComponent
	{
		private XKeys _keyStroke = XKeys.None;

		public ClickActionKeystrokePropertyComponent(AbstractActionModelTreeLeafAction selectedActionNode)
			: base(selectedActionNode)
		{
			_keyStroke = ((IClickAction) selectedActionNode.Action).KeyStroke;
		}

		protected new AbstractActionModelTreeLeafAction SelectedNode
		{
			get { return (AbstractActionModelTreeLeafAction) base.SelectedNode; }
		}

		public XKeys KeyStroke
		{
			get { return _keyStroke; }
			set
			{
				if (_keyStroke != value)
				{
					_keyStroke = value;
					this.NotifyPropertyChanged("KeyStroke");
					this.OnKeyStrokeChanged();
				}
			}
		}

		protected virtual void OnKeyStrokeChanged()
		{
			((IClickAction) this.SelectedNode.Action).KeyStroke = this.KeyStroke;
		}
	}
}