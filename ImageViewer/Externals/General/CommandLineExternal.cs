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
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Text;
using System.Xml;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Externals.General
{
	[ExtensionOf(typeof (ExternalFactoryExtensionPoint))]
	public sealed class CommandLineExternalDefinitionFactory : ExternalFactoryBase<CommandLineExternal>
	{
		public CommandLineExternalDefinitionFactory() : base(SR.DescriptionCommandLineExternal) { }

		public override IExternalPropertiesComponent CreatePropertiesComponent()
		{
			return new CommandLineExternalPropertiesComponent();
		}
	}

	public class CommandLineExternal : ExternalBase
	{
		private string _arguments = string.Empty;
		private string _command = string.Empty;
		private string _workingDirectory = string.Empty;

		private string _username = string.Empty;
		private string _domain = string.Empty;
		private SecureString _password = null;

		private bool _allowMultiValueFields = true;
		private string _multiValueFieldSeparator;

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
				if (this._arguments != value)
				{
					this._arguments = value;
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

		public SecureString SecurePassword
		{
			get { return this._password; }
			set
			{
				if (this._password != value)
				{
					if (this._password != null)
						this._password.Dispose();

					this._password = value;
					base.NotifyPropertyChanged("SecurePassword");
					base.NotifyPropertyChanged("Password");
				}
			}
		}

		public string Password
		{
			get
			{
				if (this._password == null)
					return null;
				StringBuilder sb = new StringBuilder(this._password.Length);
				for (int n = 0; n < this._password.Length; n++)
					sb.Append('*');
				return sb.ToString();
			}
			set
			{
				SecureString newPassword = null;
				if (value != null)
				{
					newPassword = new SecureString();
					foreach (char c in value)
						newPassword.AppendChar(c);
				}
				this.SecurePassword = newPassword;
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

		public override bool IsValid
		{
			get { return base.IsValid && !string.IsNullOrEmpty(_command); }
		}

		protected override bool CanLaunch(IArgumentHintResolver hintResolver)
		{
			string resolvedArgument = hintResolver.Resolve(this.Arguments, _allowMultiValueFields, " ");
			if (resolvedArgument == null)
				return false;
			return true;
		}

		protected override bool PerformLaunch(IArgumentHintResolver hintResolver)
		{
			string multiValueSeparator = _multiValueFieldSeparator;
			if (string.IsNullOrEmpty(multiValueSeparator))
				multiValueSeparator = " ";

			string command = hintResolver.Resolve(this._command, _allowMultiValueFields, multiValueSeparator);
			string workingDirectory = hintResolver.Resolve(this._workingDirectory, _allowMultiValueFields, multiValueSeparator);
			string arguments = hintResolver.Resolve(this._arguments, _allowMultiValueFields, multiValueSeparator);

			ProcessStartInfo nfo;
			if (string.IsNullOrEmpty(arguments))
				nfo = new ProcessStartInfo(command);
			else
				nfo = new ProcessStartInfo(command, arguments);
			if (Directory.Exists(workingDirectory))
				nfo.WorkingDirectory = workingDirectory;

			switch (base.WindowStyle)
			{
				case WindowStyle.Minimized:
					nfo.WindowStyle = ProcessWindowStyle.Minimized;
					break;
				case WindowStyle.Maximized:
					nfo.WindowStyle = ProcessWindowStyle.Maximized;
					break;
				case WindowStyle.Hidden:
					nfo.WindowStyle = ProcessWindowStyle.Hidden;
					break;
				case WindowStyle.Normal:
				default:
					nfo.WindowStyle = ProcessWindowStyle.Normal;
					break;
			}

			if (!string.IsNullOrEmpty(this._username))
			{
				nfo.UserName = this._username;
				nfo.Domain = this._domain;
				nfo.Password = this._password;
			}
			nfo.UseShellExecute = false;

			Process process = new Process();
			process.StartInfo = nfo;

			Platform.Log(LogLevel.Debug, "Command Line Execute: {2}> {0} {1}", nfo.FileName, nfo.Arguments, nfo.WorkingDirectory);

			return process.Start();
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder(this.Command);
			if (!string.IsNullOrEmpty(this.Arguments))
				sb.AppendFormat(" {0}", this.Arguments);
			return sb.ToString();
		}

		public override string GetState()
		{
			XmlDocument doc = new XmlDocument();
			XmlElement root = doc.CreateElement("CommandLineProperties");
			root.SetAttribute("name", base.Name);
			root.SetAttribute("label", base.Label);
			root.SetAttribute("enabled", base.Enabled.ToString());
			root.AppendChild(WriteProperty(doc, "WindowStyle", base.WindowStyle.ToString()));
			root.AppendChild(WriteProperty(doc, "Command", this.Command));
			root.AppendChild(WriteProperty(doc, "WorkingDirectory", this.WorkingDirectory));
			root.AppendChild(WriteProperty(doc, "AllowMultiValueFields", this.AllowMultiValueFields.ToString()));
			root.AppendChild(WriteProperty(doc, "MultiValueFieldSeparator", this.MultiValueFieldSeparator));
			root.AppendChild(WriteProperty(doc, "Username", this.Username));
			root.AppendChild(WriteProperty(doc, "Domain", this.Domain));
			root.AppendChild(WriteProperty(doc, "Password", this.Password));
			root.AppendChild(WriteProperty(doc, "Arguments", this.Arguments));
			doc.AppendChild(root);
			return doc.InnerXml;
		}

		public override void SetState(string stateData)
		{
			try
			{
				XmlDocument doc = new XmlDocument();
				doc.LoadXml(stateData);

				XmlElement root = null;
				foreach (XmlNode node in doc.ChildNodes)
				{
					if (node is XmlElement && node.Name == "CommandLineProperties")
					{
						root = (XmlElement) node;
						break;
					}
				}
				if (root == null)
					throw new Exception("Root node not found.");

				base.Name = root.GetAttribute("name");
				base.Label = root.GetAttribute("label");
				base.Enabled = bool.Parse(root.GetAttribute("enabled"));
				foreach (XmlNode node in root.ChildNodes)
				{
					if (node is XmlElement)
					{
						string data = node.FirstChild.Value;
						switch (node.Name)
						{
							case "WindowStyle":
								base.WindowStyle = (WindowStyle) Enum.Parse(typeof (WindowStyle), data);
								break;
							case "Command":
								this.Command = data;
								break;
							case "WorkingDirectory":
								this.WorkingDirectory = data;
								break;
							case "Arguments":
								this.Arguments = data;
								break;
							case "AllowMultiValueFields":
								this.AllowMultiValueFields = bool.Parse(data);
								break;
							case "MultiValueFieldSeparator":
								this.MultiValueFieldSeparator = data;
								break;
							case "Username":
								this.Username = data;
								break;
							case "Domain":
								this.Domain = data;
								break;
							case "Password":
								this.Password = data;
								break;
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new ArgumentException("Invalid state data.", "stateData", ex);
			}
		}

		private static XmlNode WriteProperty(XmlDocument parentDocument, string name, string value)
		{
			XmlElement element = parentDocument.CreateElement(name);
			XmlCDataSection cdata = parentDocument.CreateCDataSection(value);
			element.AppendChild(cdata);
			return element;
		}
	}
}