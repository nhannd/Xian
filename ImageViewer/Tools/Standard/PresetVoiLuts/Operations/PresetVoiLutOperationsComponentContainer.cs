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
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Operations
{
	[ExtensionPoint]
	public sealed class PresetVoiLutOperationComponentContainerViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof(PresetVoiLutOperationComponentContainerViewExtensionPoint))]
	public sealed class PresetVoiLutOperationsComponentContainer : ApplicationComponentContainer
	{
		public sealed class PresetVoiLutOperationComponentHost : ApplicationComponentHost
		{
			private readonly PresetVoiLutOperationsComponentContainer _owner;

			internal PresetVoiLutOperationComponentHost(PresetVoiLutOperationsComponentContainer owner, IPresetVoiLutOperationComponent hostedComponent)
				:base(hostedComponent)
			{
				Platform.CheckForNullReference(owner, "owner"); 
				Platform.CheckForNullReference(hostedComponent, "hostedComponent");
				_owner = owner;
				hostedComponent.SetHost(this);
			}

			public bool HasAssociatedView
			{
				get { return base.Component.GetType().IsDefined(typeof(AssociateViewAttribute), false); }
			}

			public new IPresetVoiLutOperationComponent Component
			{
				get { return (IPresetVoiLutOperationComponent)base.Component; }
			}

			public override DesktopWindow DesktopWindow
			{
				get { return _owner.Host.DesktopWindow; }
			}

			public override string Title
			{
				get { return _owner.Host.Title; }
				// individual components cannot set the title for the container
				set { throw new NotSupportedException(); }
			}
		}

		private readonly PresetVoiLutOperationComponentHost _componentHost;
		private readonly List<KeyStrokeDescriptor> _availableKeyStrokes;
		private KeyStrokeDescriptor _selectedKeyStroke;

		public PresetVoiLutOperationsComponentContainer(IEnumerable<XKeys> availableKeyStrokes, IPresetVoiLutOperationComponent component)
		{
			Platform.CheckForNullReference(availableKeyStrokes, "availableKeyStrokes");
			Platform.CheckForNullReference(component, "component");

			_availableKeyStrokes = new List<KeyStrokeDescriptor>();
			foreach (XKeys keyStroke in availableKeyStrokes)
				_availableKeyStrokes.Add(keyStroke);

			Platform.CheckPositive(_availableKeyStrokes.Count, "_availableKeyStrokes.Count");
			_selectedKeyStroke = _availableKeyStrokes[0];

			_componentHost = new PresetVoiLutOperationComponentHost(this, component);
		}

		public PresetVoiLutOperationComponentHost ComponentHost
		{
			get { return _componentHost; }
		}

		public IEnumerable<KeyStrokeDescriptor> AvailableKeyStrokes
		{
			get { return _availableKeyStrokes; }
		}

		public KeyStrokeDescriptor SelectedKeyStroke
		{
			get { return _selectedKeyStroke;  }
			set 
			{ 
				if (!_availableKeyStrokes.Contains(value))
					throw new ArgumentException(SR.ExceptionSelectedKeystrokeMustBeOneOfAvailable);

				if (_selectedKeyStroke.Equals(value))
					return;
				
				_selectedKeyStroke = value;
				Modified = true;
				NotifyPropertyChanged("SelectedKeyStroke");
			}
		}

		public bool AcceptEnabled
		{
			get
			{
				if (!ComponentHost.Component.Valid)
					return false;

				switch (ComponentHost.Component.EditContext)
				{
					case EditContext.Edit:
						return this.Modified;
					default:
						return true;
				}
			}
		}

		public override bool Modified
		{
			get
			{
				return base.Modified || ComponentHost.Component.Modified;
			}
			protected set
			{
				base.Modified = value;
				NotifyPropertyChanged("AcceptEnabled");
			}
		}

		internal PresetVoiLut GetPresetVoiLut()
		{
			PresetVoiLut preset = new PresetVoiLut(ComponentHost.Component.GetOperation());
			preset.KeyStroke = _selectedKeyStroke.KeyStroke;
			return preset;
		}
		
		public void OK()
		{
			this.Exit(ApplicationComponentExitCode.Accepted);
		}

		public void Cancel()
		{
			this.Exit(ApplicationComponentExitCode.None);
		}

		public override void Start()
		{
			base.Modified = false;
			base.Start();
			ComponentHost.Component.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(ComponentPropertyChanged);
			ComponentHost.StartComponent();

			this.ExitCode = ApplicationComponentExitCode.None;
		}

		public override void Stop()
		{
			ComponentHost.StopComponent();
			ComponentHost.Component.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(ComponentPropertyChanged);
			base.Stop();
		}

		#region ApplicationComponentContainer overrides

		public override IEnumerable<IApplicationComponent> ContainedComponents
		{
			get { yield return ComponentHost.Component; }
		}

		public override IEnumerable<IApplicationComponent> VisibleComponents
		{
			get { return this.ContainedComponents; }
		}

		public override void EnsureVisible(IApplicationComponent component)
		{
			if (!this.IsStarted)
				throw new InvalidOperationException("The container was never started.");

			// nothing to do, since the hosted components are started by default
		}

		public override void EnsureStarted(IApplicationComponent component)
		{
			if (!this.IsStarted)
				throw new InvalidOperationException("The container was never started.");

			// nothing to do, since the hosted components are visible by default
		}

		#endregion

		private void ComponentPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			this.NotifyPropertyChanged("AcceptEnabled");
		}
	}
}