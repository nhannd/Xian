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
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Externals.General;

namespace ClearCanvas.ImageViewer.Externals.View.WinForms
{
	[ExtensionPoint]
	public class AddNewExternalComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	[AssociateView(typeof (AddNewExternalComponentViewExtensionPoint))]
	public class AddNewExternalComponent : ApplicationComponent, IExternalPropertiesComponent
	{
		private IList<ExternalType> _externalTypes = new List<ExternalType>();
		private ExternalType _selectedExternalType;

		public AddNewExternalComponent()
		{
			SortedList<string, ExternalType> externalTypes = new SortedList<string, ExternalType>();
			ExternalFactoryExtensionPoint extensionPoint = new ExternalFactoryExtensionPoint();
			foreach (IExternalFactory externalFactory in extensionPoint.CreateExtensions())
			{
				ExternalType externalType = new ExternalType(externalFactory);
				externalTypes.Add(externalType.ToString(), externalType);
			}
			_externalTypes = externalTypes.Values;
		}

		public IList<ExternalType> ExternalTypes
		{
			get { return _externalTypes; }
		}

		public ExternalType SelectedExternalType
		{
			get { return _selectedExternalType; }
			set
			{
				if (_selectedExternalType != value && _externalTypes.Contains(value))
				{
					string name = string.Empty;

					if (_selectedExternalType != null)
						name = _selectedExternalType.External.Name;

					_selectedExternalType = value;

					if (_selectedExternalType != null)
						_selectedExternalType.External.Name = name;
				}
			}
		}

		public IExternal External
		{
			get
			{
				if (_selectedExternalType == null)
					return null;
				return _selectedExternalType.External;
			}
		}

		public Control ExternalGuiElement
		{
			get
			{
				if (_selectedExternalType == null)
					return null;
				return _selectedExternalType.ExternalGuiElement;
			}
		}

		public override void Start()
		{
			_selectedExternalType = CollectionUtils.SelectFirst(_externalTypes, e => e.IsDefault);
			if (_selectedExternalType == null)
				_selectedExternalType = CollectionUtils.FirstElement(_externalTypes);

			base.Start();
		}

		public override void Stop()
		{
			base.Stop();

			foreach (ExternalType externalType in _externalTypes)
			{
				externalType.DisposeGuiElement();
			}
			_externalTypes = null;
		}

		public override bool HasValidationErrors
		{
			get
			{
				IExternal external = this.External;
				return base.HasValidationErrors || (external == null || !external.IsValid);
			}
		}

		public void Cancel()
		{
			if (this.CanExit())
			{
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

		public class ExternalType
		{
			private IExternalFactory _externalFactory;
			private IExternal _external;
			private IExternalPropertiesView _externalCofigurationView;

			internal ExternalType(IExternalFactory externalFactory)
			{
				_externalFactory = externalFactory;
			}

			public bool IsDefault
			{
				get { return _externalFactory is CommandLineExternalDefinitionFactory; }
			}

			public IExternal External
			{
				get
				{
					if (_external == null)
					{
						_external = _externalFactory.CreateNew();
					}
					return _external;
				}
			}

			public Control ExternalGuiElement
			{
				get
				{
					if (_externalCofigurationView == null)
					{
						_externalCofigurationView = (IExternalPropertiesView) ViewFactory.CreateAssociatedView(this.External.GetType());
						_externalCofigurationView.SetExternalLauncher(this.External);
					}
					return (Control) _externalCofigurationView.GuiElement;
				}
			}

			internal void DisposeGuiElement()
			{
				if (_externalCofigurationView != null)
				{
					Control control = this.ExternalGuiElement;
					control.Dispose();
					_externalCofigurationView = null;
				}
			}

			public override string ToString()
			{
				return _externalFactory.Description;
			}
		}
	}
}