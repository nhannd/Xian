#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.ImageViewer.Common.ServerTree;

namespace ClearCanvas.ImageViewer.Configuration.ServerTree
{
	/// <summary>
	/// Extension point for views onto <see cref="DicomServerEditComponent"/>
	/// </summary>
	[ExtensionPoint]
	public sealed class DicomServerEditComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof(DicomServerEditComponentViewExtensionPoint))]
	public class DicomServerEditComponent : ApplicationComponent
	{
		private class ConflictingServerValidationRule : IValidationRule
		{
			private readonly string _propertyName;

			public ConflictingServerValidationRule(string propertyName)
			{
				_propertyName = propertyName;
			}

			#region IValidationRule Members

			public string PropertyName
			{
				get { return _propertyName; }
			}

			public ValidationResult GetResult(IApplicationComponent component)
			{
				DicomServerEditComponent serverComponent = (DicomServerEditComponent)component;

				Common.ServerTree.ServerTree serverTree = serverComponent._serverTree;

				bool isConflicted;
				string conflictingServerPath;

				if (serverTree.CurrentNode.IsServer)
				{
					isConflicted = serverTree.CanEditCurrentServer(serverComponent.ServerName, 
					                                               serverComponent.ServerAE, 
					                                               serverComponent.ServerHost, 
					                                               serverComponent.ServerPort, out conflictingServerPath);
				}
				else
				{
					isConflicted = serverTree.CanAddServerToCurrentGroup(serverComponent.ServerName, 
					                                                     serverComponent.ServerAE, 
					                                                     serverComponent.ServerHost, 
					                                                     serverComponent.ServerPort, out conflictingServerPath);
				}

				if (isConflicted)
				{
					return new ValidationResult(false, String.Format(SR.FormatServerConflict, conflictingServerPath));
				}

				return new ValidationResult(true, "");

			}

			#endregion
		}

		public static readonly int MinimumPort = 1;
		public static readonly int MaximumPort = 65535;
		public static readonly int DefaultPort = 104;
		public static readonly int DefaultHeaderServicePort = 50221;
		public static readonly int DefaultWadoServicePort = 1000;


		#region Private Fields

		private readonly Common.ServerTree.ServerTree _serverTree;
		private string _serverName;
		private string _serverLocation;
		private string _serverAE;
		private string _serverHost;
		private int _serverPort;
		private bool _isStreaming;
		private int _headerServicePort;
		private int _wadoServicePort;

		#endregion

		public DicomServerEditComponent(Common.ServerTree.ServerTree dicomServerTree)
		{
			_serverTree = dicomServerTree;

			if (_serverTree.CurrentNode.IsServer)
			{
                IServerTreeDicomServer server = (IServerTreeDicomServer)_serverTree.CurrentNode;
				_serverName = server.Name;
				_serverLocation = server.Location;
				_serverAE = server.AETitle;
				_serverHost = server.HostName;
				_serverPort = server.Port;
				_isStreaming = server.IsStreaming;
				_headerServicePort = server.HeaderServicePort;
				_wadoServicePort = server.WadoServicePort;
			}
			else
			{
				_serverName = "";
				_serverLocation = "";
				_serverAE = "";
				_serverHost = "";
				_serverPort = DefaultPort;
				_isStreaming = false;
				_headerServicePort = DefaultHeaderServicePort;
				_wadoServicePort = DefaultWadoServicePort;
			}
		}

		public override void Start()
		{
			base.Start();

			// All of the properties contribute to conflicts in the server tree, 
			// so we'll just show the validation errors for each one.
			base.Validation.Add(new ConflictingServerValidationRule("ServerName"));
			base.Validation.Add(new ConflictingServerValidationRule("ServerAE"));
			base.Validation.Add(new ConflictingServerValidationRule("ServerHost"));
			base.Validation.Add(new ConflictingServerValidationRule("ServerPort"));
		}

		#region Presentation Model

		[ValidateNotNull(Message = "MessageServerNameCannotBeEmpty")]
		public string ServerName
		{
			get { return _serverName; }
			set
			{
				if (_serverName == value)
					return;

				_serverName = value;
				AcceptEnabled = true;
				NotifyPropertyChanged("ServerName");
			}
		}

		public string ServerLocation
		{
			get { return _serverLocation; }
			set
			{
				if (_serverLocation == value)
					return;
				
				_serverLocation = value;
				AcceptEnabled = true;
				NotifyPropertyChanged("ServerLocation");
			}
		}

		[ValidateLength(1, 16, Message = "MessageServerAEInvalidLength")]
		[ValidateRegex(@"[\r\n\e\f\\]+", SuccessOnMatch = false, Message = "MessageServerAEInvalidCharacters")]
		public string ServerAE
		{
			get { return _serverAE; }
			set
			{
				if (_serverAE == value)
					return;

				_serverAE = value;
				AcceptEnabled = true;
				NotifyPropertyChanged("ServerAE");
			}
		}

		[ValidateNotNull(Message = "MessageHostNameCannotBeEmpty")]
		public string ServerHost
		{
			get { return _serverHost; }
			set
			{
				if (_serverHost == value)
					return;
				
				_serverHost = value;
				AcceptEnabled = true;
				NotifyPropertyChanged("ServerHost");
			}
		}

		[ValidateGreaterThanAttribute(0, Inclusive = false, Message = "MessagePortInvalid")]
		[ValidateLessThanAttribute(65536, Inclusive = false, Message = "MessagePortInvalid")]
		public int ServerPort
		{
			get { return _serverPort; }
			set
			{
				if (_serverPort == value)
					return;
				
				_serverPort = value;
				AcceptEnabled = true;
				NotifyPropertyChanged("ServerPort");
			}
		}

		public bool IsStreaming
		{
			get { return _isStreaming;  }
			set
			{
				if (_isStreaming == value)
					return;

				_isStreaming = value;
				//StreamingPortsEnabled = !IsStreamin
				AcceptEnabled = true;
				NotifyPropertyChanged("IsStreaming");
			}
		}

		[ValidateGreaterThanAttribute(0, Inclusive = false, Message = "MessagePortInvalid")]
		[ValidateLessThanAttribute(65536, Inclusive = false, Message = "MessagePortInvalid")]
		public int HeaderServicePort
		{
			get { return _headerServicePort; }
			set
			{
				if (_headerServicePort == value)
					return;

				_headerServicePort = value;
				AcceptEnabled = true;
				NotifyPropertyChanged("HeaderServicePort");
			}
		}

		[ValidateGreaterThanAttribute(0, Inclusive = false, Message = "MessagePortInvalid")]
		[ValidateLessThanAttribute(65536, Inclusive = false, Message = "MessagePortInvalid")]
		public int WadoServicePort
		{
			get { return _wadoServicePort; }
			set
			{
				if (_wadoServicePort == value)
					return;

				_wadoServicePort = value;
				AcceptEnabled = true;
				NotifyPropertyChanged("WadoServicePort");
			}
		}

		public bool AcceptEnabled
		{
			get { return this.Modified; }
			set
			{
				if (value == this.Modified)
					return;

				this.Modified = value;
				NotifyPropertyChanged("AcceptEnabled");
			}
		}

		public bool FieldReadonly
		{
			get { return _serverTree.CurrentNode.IsLocalDataStore; }
		}

		public void Accept()
		{
			ServerAE = ServerAE.Trim();
			ServerName = ServerName.Trim();
			ServerLocation = ServerLocation.Trim();
			ServerHost = ServerHost.Trim();

			if (base.HasValidationErrors)
			{
				this.ShowValidation(true);
			}
			else
			{
                var newServer = new ServerTreeDicomServer(
					_serverName, 
					_serverLocation, 
					_serverHost, 
					_serverAE, 
					_serverPort, 
					_isStreaming,
					_headerServicePort,
					_wadoServicePort);

				// edit current server
				if (_serverTree.CurrentNode.IsServer)
				{
					_serverTree.ReplaceDicomServerInCurrentGroup(newServer);
				}
					// add new server
				else if (_serverTree.CurrentNode.IsServerGroup)
				{
					((IServerTreeGroup) _serverTree.CurrentNode).AddChild(newServer);
					_serverTree.CurrentNode = newServer;
				}

                _serverTree.Save();
                _serverTree.FireServerTreeUpdatedEvent();

				this.ExitCode = ApplicationComponentExitCode.Accepted;
				Host.Exit();
			}
		}

		public void Cancel()
		{
			Host.Exit();
		}

		#endregion
	}
}