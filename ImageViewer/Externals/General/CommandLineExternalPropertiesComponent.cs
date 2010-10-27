#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Security;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.ImageViewer.Externals.General
{
	[ExtensionPoint]
	public sealed class CommandLineExternalPropertiesComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	[AssociateView(typeof (CommandLineExternalPropertiesComponentViewExtensionPoint))]
	public class CommandLineExternalPropertiesComponent : ExternalPropertiesComponent<CommandLineExternal>
	{
		private string _arguments = string.Empty;
		private string _command = string.Empty;
		private string _workingDirectory = string.Empty;

		private string _username = string.Empty;
		private string _domain = string.Empty;
		private SecureString _password = null;

		private bool _allowMultiValueFields = true;
		private string _multiValueFieldSeparator;

		[ValidateLength(1, Message = "MessageValueCannotBeEmpty")]
		public string Command
		{
			get { return this._command; }
			set
			{
				if (this._command != value)
				{
					this._command = value;
					base.NotifyPropertyChanged("Command");
				}
			}
		}

		public string WorkingDirectory
		{
			get { return this._workingDirectory; }
			set
			{
				if (this._workingDirectory != value)
				{
					this._workingDirectory = value;
					base.NotifyPropertyChanged("WorkingDirectory");
				}
			}
		}

		public string Arguments
		{
			get { return _arguments; }
			set
			{
				if (_arguments != value)
				{
					_arguments = value;
					this.NotifyPropertyChanged("Arguments");
				}
			}
		}

		public string Username
		{
			get { return this._username; }
			set
			{
				if (this._username != value)
				{
					this._username = value;
					base.NotifyPropertyChanged("Username");
				}
			}
		}

		public string Domain
		{
			get { return this._domain; }
			set
			{
				if (this._domain != value)
				{
					this._domain = value;
					base.NotifyPropertyChanged("Domain");
				}
			}
		}

		public SecureString Password
		{
			get { return this._password; }
			set
			{
				if (this._password != value)
				{
					this._password = value;
					base.NotifyPropertyChanged("Password");
				}
			}
		}

		public bool AllowMultiValueFields
		{
			get { return this._allowMultiValueFields; }
			set
			{
				if (this._allowMultiValueFields != value)
				{
					this._allowMultiValueFields = value;
					base.NotifyPropertyChanged("AllowMultiValueFields");
				}
			}
		}

		public string MultiValueFieldSeparator
		{
			get { return this._multiValueFieldSeparator; }
			set
			{
				if (this._multiValueFieldSeparator != value)
				{
					this._multiValueFieldSeparator = value;
					base.NotifyPropertyChanged("MultiValueFieldSeparator");
				}
			}
		}

		public string ArgumentFieldsHelpText
		{
			get { return SR.HelpCommandLineExternalArgumentFields; }
		}

		public override void Load(CommandLineExternal external)
		{
			base.Load(external);

			this.Command = external.Command;
			this.WorkingDirectory = external.WorkingDirectory;
			this.Arguments = external.Arguments;
			this.Username = external.Username;
			this.Domain = external.Domain;
			this.Password = external.SecurePassword;
			this.AllowMultiValueFields = external.AllowMultiValueFields;
			this.MultiValueFieldSeparator = external.MultiValueFieldSeparator;

			base.Modified = false;
		}

		public override void Update(CommandLineExternal external)
		{
			base.Update(external);

			external.Command = this.Command;
			external.WorkingDirectory = this.WorkingDirectory;
			external.Arguments = this.Arguments;
			external.Username = this.Username;
			external.Domain = this.Domain;
			external.SecurePassword = this.Password;
			external.AllowMultiValueFields = this.AllowMultiValueFields;
			external.MultiValueFieldSeparator = this.MultiValueFieldSeparator;
		}
	}
}