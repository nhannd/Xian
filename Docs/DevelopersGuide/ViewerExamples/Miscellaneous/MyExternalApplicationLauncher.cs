#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Diagnostics;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Externals;
using ClearCanvas.ImageViewer.Externals.General;
using Path=System.IO.Path;

// ... (other using namespace statements here)

namespace MyPlugin.Miscellaneous
{
	/// <summary>
	/// Defines your custom launcher - the external class, the configuration component for it,
	/// and the descriptive label to be used in the Add External dropdown.
	/// </summary>
	[ExtensionOf(typeof (ExternalFactoryExtensionPoint))]
	public class MyExternalApplicationLauncherFactory : ExternalFactoryBase<MyExternalApplicationLauncher>
	{
		public MyExternalApplicationLauncherFactory() : base("Custom launcher") {}

		public override IExternalPropertiesComponent CreatePropertiesComponent()
		{
			return new MyExternalApplicationLauncherProperties();
		}
	}

	/// <summary>
	/// This is the actual external class that launches the application.
	/// </summary>
	public class MyExternalApplicationLauncher : ExternalBase
	{
		private string _command;

		public string Command
		{
			get { return _command; }
			set { _command = value; }
		}

		protected override bool CanLaunch(IArgumentHintResolver hintResolver)
		{
			// check if the supplied hints are enough to launch the external using the current configuration
			return true;
		}

		protected override bool PerformLaunch(IArgumentHintResolver hintResolver)
		{
			string filename = Path.GetTempFileName();

			// write the desired input to the file
			File.WriteAllText(filename, string.Format("<xml>{0}</xml>", hintResolver.Resolve("$FILENAME$")));

			// start the external and tell it the file containing input
			Process.Start(_command, string.Format("\"{0}\"", filename));

			return true;
		}

	}

	public sealed class MyExternalApplicationLauncherPropertiesViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	/// <summary>
	/// This is just an ApplicationComponent for editing the properties of your type of external.
	/// </summary>
	[AssociateView(typeof (MyExternalApplicationLauncherPropertiesViewExtensionPoint))]
	public class MyExternalApplicationLauncherProperties : ExternalPropertiesComponent<MyExternalApplicationLauncher>
	{
		private string _command;

		public string Command
		{
			get { return _command; }
			set
			{
				if (_command != value)
				{
					_command = value;
					this.NotifyPropertyChanged("Command");
				}
			}
		}

		public override void Load(MyExternalApplicationLauncher external)
		{
			base.Load(external);

			// populate this component from the external
			_command = external.Command;
		}

		public override void Update(MyExternalApplicationLauncher external)
		{
			// update the external using the values specified in this component
			external.Command = _command;

			base.Update(external);
		}
	}
}