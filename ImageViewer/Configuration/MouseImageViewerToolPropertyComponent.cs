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

namespace ClearCanvas.ImageViewer.Configuration
{
	[ExtensionPoint]
	public sealed class MouseImageViewerToolPropertyComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	[AssociateView(typeof (MouseImageViewerToolPropertyComponentViewExtensionPoint))]
	public class MouseImageViewerToolPropertyComponent : NodePropertiesComponent
	{
		private XMouseButtons _activeMouseButtons = XMouseButtons.Left;
		private XMouseButtons _globalMouseButtons = XMouseButtons.None;
		private ModifierFlags _globalModifiers = ModifierFlags.None;
		private bool _initiallyActive = false;

		public MouseImageViewerToolPropertyComponent(AbstractActionModelTreeNode selectedNode, XMouseButtons activeMouseButtons, XMouseButtons globalMouseButtons, ModifierFlags globalModifiers, bool initiallyActive)
			: base(selectedNode)
		{
			_activeMouseButtons = activeMouseButtons;
			_globalMouseButtons = globalMouseButtons;
			_globalModifiers = globalModifiers;
			_initiallyActive = initiallyActive;
		}

		public bool InitiallyActive
		{
			get { return _initiallyActive; }
			set
			{
				if (!this.RequestPropertyValidation("InitiallyActive", value))
					return;

				if (_initiallyActive != value)
				{
					_initiallyActive = value;
					this.NotifyPropertyValidated("InitiallyActive", value);
					this.OnInitiallyActiveChanged();
				}
			}
		}

		public XMouseButtons ActiveMouseButtons
		{
			get { return _activeMouseButtons; }
			set
			{
				if (!this.RequestPropertyValidation("ActiveMouseButtons", value))
					return;

				if (_activeMouseButtons != value)
				{
					_activeMouseButtons = value;
					this.NotifyPropertyValidated("ActiveMouseButtons", value);
					this.OnActiveMouseButtonsChanged();
				}
			}
		}

		public XMouseButtons GlobalMouseButtons
		{
			get { return _globalMouseButtons; }
			set
			{
				if (!this.RequestPropertyValidation("GlobalMouseButtons", value))
					return;

				if (_globalMouseButtons != value)
				{
					_globalMouseButtons = value;
					this.NotifyPropertyValidated("GlobalMouseButtons", value);
					this.OnGlobalMouseButtonsChanged();
				}
			}
		}

		public ModifierFlags GlobalModifiers
		{
			get { return _globalModifiers; }
			set
			{
				if (!this.RequestPropertyValidation("GlobalModifiers", value))
					return;

				if (_globalModifiers != value)
				{
					_globalModifiers = value;
					this.NotifyPropertyValidated("GlobalModifiers", value);
					this.OnGlobalModifiersChanged();
				}
			}
		}

		protected virtual void OnInitiallyActiveChanged()
		{
			this.NotifyPropertyChanged("InitiallyActive");
		}

		protected virtual void OnActiveMouseButtonsChanged()
		{
			this.NotifyPropertyChanged("ActiveMouseButtons");
			this.InitiallyActive = false;
		}

		protected virtual void OnGlobalMouseButtonsChanged()
		{
			this.NotifyPropertyChanged("GlobalMouseButtons");
		}

		protected virtual void OnGlobalModifiersChanged()
		{
			this.NotifyPropertyChanged("GlobalModifiers");
		}
	}
}