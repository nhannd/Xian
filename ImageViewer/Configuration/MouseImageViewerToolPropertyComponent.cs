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

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Configuration.ActionModel;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Configuration
{
	[ExtensionPoint]
	public sealed class MouseImageViewerToolPropertyComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	[AssociateView(typeof (MouseImageViewerToolPropertyComponentViewExtensionPoint))]
	public class MouseImageViewerToolPropertyComponent : NodePropertiesComponent
	{
		private readonly MouseToolSettingsProfile _toolProfile;
		private readonly string _actionId;

		private XMouseButtons _activeMouseButtons = XMouseButtons.Left;
		private XMouseButtons _globalMouseButtons = XMouseButtons.None;
		private ModifierFlags _globalModifiers = ModifierFlags.None;
		private bool _initiallyActive = false;

		public MouseImageViewerToolPropertyComponent(AbstractActionModelTreeLeafAction selectedNode, MouseToolSettingsProfile toolProfile) : base(selectedNode)
		{
			Platform.CheckTrue(MouseToolSettingsProfile.Current.IsRegisteredMouseToolActivationAction(selectedNode.ActionId), "action must be registered as a mouse tool activator");
			Platform.CheckForNullReference(toolProfile, "toolProfile");

			_actionId = selectedNode.ActionId;
			_toolProfile = toolProfile;

			MouseToolSettingsProfile.Setting setting = _toolProfile.GetEntryByActivationActionId(_actionId);
			_activeMouseButtons = setting.MouseButton.GetValueOrDefault(XMouseButtons.Left);
			_globalMouseButtons = setting.DefaultMouseButton.GetValueOrDefault(XMouseButtons.None);
			_globalModifiers = setting.DefaultMouseButtonModifiers.GetValueOrDefault(ModifierFlags.None);
			_initiallyActive = setting.InitiallyActive.GetValueOrDefault(false);
		}

		public new AbstractActionModelTreeLeafClickAction SelectedNode
		{
			get { return (AbstractActionModelTreeLeafClickAction) base.SelectedNode; }
		}

		public bool InitiallyActive
		{
			get { return _initiallyActive; }
			set
			{
				if (_initiallyActive != value)
				{
					_initiallyActive = value;
					this.OnInitiallyActiveChanged();
					this.NotifyPropertyChanged("InitiallyActive");
				}
			}
		}

		public XMouseButtons ActiveMouseButtons
		{
			get { return _activeMouseButtons; }
			set
			{
				if (_activeMouseButtons != value)
				{
					_activeMouseButtons = value;
					this.OnActiveMouseButtonsChanged();
					this.NotifyPropertyChanged("ActiveMouseButtons");
				}
			}
		}

		public XMouseButtons GlobalMouseButtons
		{
			get { return _globalMouseButtons; }
			set
			{
				if (_globalMouseButtons != value)
				{
					_globalMouseButtons = value;
					this.OnGlobalMouseButtonsChanged();
					this.NotifyPropertyChanged("GlobalMouseButtons");
				}
			}
		}

		public ModifierFlags GlobalModifiers
		{
			get { return _globalModifiers; }
			set
			{
				if (_globalModifiers != value)
				{
					_globalModifiers = value;
					this.OnGlobalModifiersChanged();
					this.NotifyPropertyChanged("GlobalModifiers");
				}
			}
		}

		protected virtual void OnInitiallyActiveChanged()
		{
			_toolProfile.GetEntryByActivationActionId(_actionId).InitiallyActive = _initiallyActive;
		}

		protected virtual void OnActiveMouseButtonsChanged()
		{
			_toolProfile.GetEntryByActivationActionId(_actionId).MouseButton = _activeMouseButtons;
		}

		protected virtual void OnGlobalMouseButtonsChanged()
		{
			_toolProfile.GetEntryByActivationActionId(_actionId).DefaultMouseButton = _globalMouseButtons;
		}

		protected virtual void OnGlobalModifiersChanged()
		{
			_toolProfile.GetEntryByActivationActionId(_actionId).DefaultMouseButtonModifiers = _globalModifiers;
		}
	}
}