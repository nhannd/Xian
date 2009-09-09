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
using System.Diagnostics;
using System.Security;
using System.Text;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Externals.General
{
	[ExtensionOf(typeof (ExternalFactoryExtensionPoint))]
	public class CommandLineExternalDefinitionFactory : ExternalFactoryBase<CommandLineExternal>
	{
		public CommandLineExternalDefinitionFactory() : base("Launch via Command Line") {}
	}

	[ExtensionPoint]
	public sealed class CommandLineExternalConfigurationViewExtensionPoint : ExtensionPoint<IExternalPropertiesView> {}

	[AssociateView(typeof (CommandLineExternalConfigurationViewExtensionPoint))]
	public class CommandLineExternal : ExternalBase
	{
		private string _argumentString = string.Empty;
		private string _command = string.Empty;
		private string _workingDirectory = string.Empty;

		private string _username = string.Empty;
		private string _domain = string.Empty;
		private SecureString _password = null;

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

		public string ArgumentString
		{
			get { return _argumentString; }
			set
			{
				if (_argumentString != value)
				{
					_argumentString = value;
					this.NotifyPropertyChanged("ArgumentString");
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
				if (this._password != null)
					this._password.Dispose();
				this._password = new SecureString();
				foreach (char c in value)
					this._password.AppendChar(c);
				base.NotifyPropertyChanged("SecurePassword");
				base.NotifyPropertyChanged("Password");
			}
		}

		public IList<string> Arguments
		{
			get { return new List<string>(_argumentString.Split(new string[] {Environment.NewLine}, StringSplitOptions.None)); }
		}

		protected override bool CanLaunch(IArgumentHintResolver hintResolver)
		{
			foreach (string argument in this.Arguments)
			{
				string resolvedArgument = hintResolver.Resolve(argument);
				if (resolvedArgument == null)
					return false;
			}
			return true;
		}

		protected override bool PerformLaunch(IArgumentHintResolver hintResolver)
		{
			StringBuilder argumentsBuilder = new StringBuilder();
			foreach (string argument in this.Arguments)
			{
				string resolvedArgument = hintResolver.Resolve(argument);
				if (string.IsNullOrEmpty(resolvedArgument) || (resolvedArgument.IndexOf(' ') >= 0 && resolvedArgument.IndexOf("\"") < 0))
					resolvedArgument = string.Format("\"{0}\"", resolvedArgument);
				argumentsBuilder.Append(resolvedArgument);
				argumentsBuilder.Append(' ');
			}

			string command = hintResolver.Resolve(this._command);
			string workingDirectory = hintResolver.Resolve(this._workingDirectory);
			string arguments = argumentsBuilder.ToString().TrimEnd(' ');

			ProcessStartInfo nfo;
			if (string.IsNullOrEmpty(arguments))
				nfo = new ProcessStartInfo(command);
			else
				nfo = new ProcessStartInfo(command, arguments);
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

			return process.Start();
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder(this.Command);
			foreach (string argument in Arguments)
				sb.AppendFormat(" {0}", argument);
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
			foreach (string argument in this.Arguments)
			{
				root.AppendChild(WriteProperty(doc, "Argument", argument));
			}
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

				StringBuilder arguments = new StringBuilder();

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
							case "Argument":
								arguments.AppendLine(data);
								break;
						}
					}
				}
				this.ArgumentString = arguments.ToString();
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