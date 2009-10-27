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

using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Externals.View.WinForms
{
	[ExtensionPoint]
	public sealed class ExternalPropertiesComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	public interface IExternalPropertiesComponent : IApplicationComponent
	{
		IExternal External { get; }
		Control ExternalGuiElement { get; }
		void Cancel();
		bool Accept();
	}

	[AssociateView(typeof (ExternalPropertiesComponentViewExtensionPoint))]
	public class ExternalPropertiesComponent : ApplicationComponent, IExternalPropertiesComponent
	{
		private IExternal _external;
		private IExternalPropertiesView _externalCofigurationView;
		private string _initialState;

		public ExternalPropertiesComponent(IExternal external)
		{
			Platform.CheckForNullReference(external, "external");
			_external = external;
		}

		public IExternal External
		{
			get { return _external; }
		}

		public Control ExternalGuiElement
		{
			get
			{
				if (_externalCofigurationView == null)
				{
					_externalCofigurationView = (IExternalPropertiesView) ViewFactory.CreateAssociatedView(_external.GetType());
					_externalCofigurationView.SetExternalLauncher(_external);
				}
				return (Control) _externalCofigurationView.GuiElement;
			}
		}

		public override bool HasValidationErrors
		{
			get { return base.HasValidationErrors || !_external.IsValid; }
		}

		public override void Start()
		{
			base.Start();

			_initialState = _external.GetState();
		}

		public void Cancel()
		{
			if (this.CanExit())
			{
				_external.SetState(_initialState);
				this.Exit(ApplicationComponentExitCode.None);
			}
		}

		public bool Accept()
		{
			if (this.CanExit() && !this.HasValidationErrors)
			{
				this.Exit(ApplicationComponentExitCode.Accepted);
				return true;
			}
			return false;
		}
	}
}