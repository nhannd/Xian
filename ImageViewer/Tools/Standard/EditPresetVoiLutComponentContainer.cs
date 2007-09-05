using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	/// <summary>
	/// Extension point for views onto <see cref="EditPresetVoiLutComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class EditPresetVoiLutComponentContainerViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// EditPresetVoiLutComponent class
	/// </summary>
	[AssociateView(typeof(EditPresetVoiLutComponentContainerViewExtensionPoint))]
	public class EditPresetVoiLutComponentContainer : ApplicationComponentContainer
	{
		public class EditPresetVoiLutComponentHost : ApplicationComponentHost
		{
			private EditPresetVoiLutComponentContainer _owner;

			internal EditPresetVoiLutComponentHost(EditPresetVoiLutComponentContainer owner, IEditPresetVoiLutApplicationComponent hostedComponent)
				:base(hostedComponent)
			{
				Platform.CheckForNullReference(hostedComponent, "hostedComponent");
				_owner = owner;
				hostedComponent.SetHost(this);
			}

			public new IEditPresetVoiLutApplicationComponent Component
			{
				get { return (IEditPresetVoiLutApplicationComponent)base.Component; }
			}

			public override DesktopWindow DesktopWindow
			{
				get { return _owner.Host.DesktopWindow; }
			}
		}

		public class KeyStrokeDescriptor : IEquatable<KeyStrokeDescriptor>, IEquatable<XKeys>
		{
			private readonly XKeys _keyStroke;

			internal KeyStrokeDescriptor(XKeys keyStroke)
			{
				_keyStroke = keyStroke;
			}

			public XKeys KeyStroke
			{
				get
				{
					return _keyStroke;
				}
			}

			public override string ToString()
			{
				if (_keyStroke == XKeys.None)
					return String.Format("({0})", _keyStroke.ToString());

				return _keyStroke.ToString();
			}

			public override int GetHashCode()
			{
				return _keyStroke.GetHashCode();
			}

			public override bool Equals(object obj)
			{
				if (obj == this)
					return true;

				if (obj is KeyStrokeDescriptor)
					return this.Equals((KeyStrokeDescriptor) obj);

				if (obj is XKeys)
					return this.Equals((XKeys) obj);

				return false;
			}

			#region IEquatable<KeyStrokeDescriptor> Members

			public bool Equals(KeyStrokeDescriptor other)
			{
				return this.KeyStroke == other.KeyStroke;
			}

			#endregion

			#region IEquatable<XKeys> Members

			public bool Equals(XKeys other)
			{
				return _keyStroke == other;	
			}

			#endregion

			public static implicit operator XKeys(KeyStrokeDescriptor descriptor)
			{
				return descriptor._keyStroke;
			}

			public static implicit operator KeyStrokeDescriptor(XKeys keyStroke)
			{
				return new KeyStrokeDescriptor(keyStroke);
			}
		}

		private readonly EditPresetVoiLutComponentHost _editComponentHost;
		private readonly List<KeyStrokeDescriptor> _availableKeyStrokes;
		private KeyStrokeDescriptor _selectedKeyStroke;

		public EditPresetVoiLutComponentContainer(IEnumerable<XKeys> availableKeyStrokes, IEditPresetVoiLutApplicationComponent editComponentHost)
		{
			Platform.CheckForNullReference(availableKeyStrokes, "availableKeyStrokes");
			Platform.CheckForNullReference(editComponentHost, "editComponentHost");

			_availableKeyStrokes = new List<KeyStrokeDescriptor>();
			foreach (XKeys keyStroke in availableKeyStrokes)
				_availableKeyStrokes.Add(keyStroke);

			Platform.CheckPositive(_availableKeyStrokes.Count, "Count");
			_selectedKeyStroke = _availableKeyStrokes[0];

			_editComponentHost = new EditPresetVoiLutComponentHost(this, editComponentHost);
		}

		public EditPresetVoiLutComponentHost EditComponentHost
		{
			get { return _editComponentHost; }
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
					throw new ArgumentException("The selected keystroke must be one of the available keystrokes.");

				_selectedKeyStroke = value;
				NotifyPropertyChanged("SelectedKeyStroke");
				base.Modified = true;
			}
		}

		internal PresetVoiLut GetPresetVoiLut()
		{
			PresetVoiLut preset = new PresetVoiLut(EditComponentHost.Component.GetApplicator());
			preset.KeyStroke = _selectedKeyStroke.KeyStroke;
			return preset;
		}

		public void OK()
		{
			this.ExitCode = ApplicationComponentExitCode.Normal;
			this.Host.Exit();
		}

		public void Cancel()
		{
			this.ExitCode = ApplicationComponentExitCode.Cancelled;
			this.Host.Exit();
		}

		public override void Start()
		{
			base.Start();
			EditComponentHost.StartComponent();
		
			this.ExitCode = ApplicationComponentExitCode.Cancelled;
		}

		public override void Stop()
		{
			EditComponentHost.StopComponent();

			base.Stop();
		}

		public override bool CanExit(UserInteraction interactive)
		{
			return (EditComponentHost.Component.CanExit(interactive) && base.CanExit(interactive));
		}

		public override bool Modified
		{
			get
			{
				return base.Modified || EditComponentHost.Component.Modified;
			}
			protected set
			{
				base.Modified = value;
			}
		}

		#region ApplicationComponentContainer overrides

		public override IEnumerable<IApplicationComponent> ContainedComponents
		{
			get { yield return EditComponentHost.Component; }
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
	}
}
