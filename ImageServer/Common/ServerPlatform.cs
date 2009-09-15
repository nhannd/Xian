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
using System.Net;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Audit;
using ClearCanvas.Dicom.Audit;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.ServiceModel;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Common
{
	/// <summary>
	/// A collection of useful ImageServer utility functions.
	/// </summary>
    static public class ServerPlatform
    {
        #region Private Fields
        private static string _dbVersion;
        private static string _tempDir;
        private static readonly object _syncLock = new object();
    	private static DicomAuditSource _auditSource;
    	private static AuditLog _log;
    	private static string _hostId;
    	private static string _serverInstanceId;
    	private static string _processorId;
    	#endregion

        /// <summary>
        /// Generates an alert message with an expiration time.
        /// </summary>
        /// <param name="category">An alert category</param>
        /// <param name="level">Alert level</param>
        /// <param name="source">Name of the source where the alert is raised</param>
        /// <param name="alertCode">Alert type code</param>
        /// <param name="contextData">The user-defined application context data</param>
        /// <param name="expirationTime">Expiration time for the alert</param>
        /// <param name="message">The alert message or formatted message.</param>
        /// <param name="args">Paramaters used in the alert message, when specified.</param>
        public static void Alert(AlertCategory category, AlertLevel level, String source, 
                                    int alertCode, object contextData, TimeSpan expirationTime, 
                                    String message, params object[] args)
        {
            Platform.CheckForNullReference(source, "source");
            Platform.CheckForNullReference(message, "message");
            IAlertService service = Platform.GetService<IAlertService>();
            if (service != null)
            {
                AlertSource src = new AlertSource(source);
                src.Host = ServerInstanceId;
                Alert alert = new Alert();
                alert.Category = category;
                alert.Level = level;
                alert.Code = alertCode;
                alert.ExpirationTime = Platform.Time.Add(expirationTime);
                alert.Source = src;
                alert.Message = String.Format(message, args);
                alert.ContextData = contextData;

                service.GenerateAlert(alert);
            }
        }

		/// <summary>
		/// A well known AuditSource for ImageServer audit logging.
		/// </summary>
    	public static DicomAuditSource AuditSource
    	{
    		get
    		{
    			lock (_syncLock)
    			{
    				if (_auditSource == null)
    				{
    					_auditSource = new DicomAuditSource("ImageServer");
    				}
    				return _auditSource;
    			}
    		}
    	}

		/// <summary>
		/// Log an Audit message.
		/// </summary>
		/// <param name="helper"></param>
		public static void LogAuditMessage(DicomAuditHelper helper)
		{
			lock (_syncLock)
			{
				if (_log == null)
					_log = new AuditLog("ImageServer","DICOM");

				_log.WriteEntry(helper.Operation, helper.Serialize(false));
			}
		}

        /// <summary>
        /// Gets the path to the temporary folder.
        /// </summary>
        public static String TempDirectory
        {
            get
            {
                if (String.IsNullOrEmpty(_tempDir) || !Directory.Exists(_tempDir))
                {
                    lock(_syncLock)
                    {
                        // if specified in the config, use it
                        if (!String.IsNullOrEmpty(ImageServerCommonConfiguration.TemporaryPath))
                        {
                            _tempDir = ImageServerCommonConfiguration.TemporaryPath;
                        }
                        else
                        {
                            // Use the OS temp folder instead, assuming it's not too long.
                            // Avoid creating a temp folder under the installation directory because it could
                            // lead to PathTooLongException.
                            _tempDir = Path.Combine(Path.GetTempPath(), "ImageServer");
                        }

                        // make sure it exists
                        if (!Directory.Exists(_tempDir))
                        {
                            Directory.CreateDirectory(_tempDir);
                        }
                    }
                }

                return _tempDir;
            }
        }

        /// <summary>
        /// Returns the version info in the database.
        /// </summary>
        public static String VersionString
        {
            get
            {
                lock (_syncLock)
                {
                    if (String.IsNullOrEmpty(_dbVersion))
                    {
                        try
                        {
                            IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();
                            using (IReadContext ctx = store.OpenReadContext())
                            {
                                IDatabaseVersionEntityBroker broker = ctx.GetBroker<IDatabaseVersionEntityBroker>();
                                DatabaseVersion version = broker.FindOne(new DatabaseVersionSelectCriteria());
								if (version != null)
                                    _dbVersion = version.GetVersionString();
                                else
                                    _dbVersion = String.Empty;
                            }
                        }
                        catch (Exception ex)
                        {
                            Platform.Log(LogLevel.Error, ex);
                        }
                    }
                }

                return _dbVersion;
            }
        }

		/// <summary>
		/// Flag telling if instance level logging is enabled.
		/// </summary>
    	public static LogLevel InstanceLogLevel
    	{
			get { return Settings.Default.InstanceLogging ? LogLevel.Info : LogLevel.Debug; }
    	}

    	/// <summary>
    	/// Returns a string that can be used to identify the host machine where the server is running
    	/// </summary>
    	public static string HostId
    	{
    		get
    		{
    			if (String.IsNullOrEmpty(_hostId))
    			{
    				String strHostName = Dns.GetHostName();
    				if (String.IsNullOrEmpty(strHostName) == false)
    					_hostId = strHostName;
    				else
    				{
    					// Find host by name
    					IPHostEntry iphostentry = Dns.GetHostEntry(strHostName);

    					// Enumerate IP addresses, pick an IPv4 address first
    					foreach (IPAddress ipaddress in iphostentry.AddressList)
    					{
    						if (ipaddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
    						{
    							_hostId = ipaddress.ToString();
    							break;
    						}
    					}
    				}
    			}

    			return _hostId;
    		}
    	}

		/// <summary>
		/// Server Instance Id
		/// </summary>
    	public static string ServerInstanceId
    	{
    		get
    		{
    			if (String.IsNullOrEmpty(_serverInstanceId))
    			{
    				_serverInstanceId = String.Format("Host={0}/Pid={1}", HostId, Process.GetCurrentProcess().Id);
    			}

    			return _serverInstanceId;
    		}
    	}

    	/// <summary>
    	/// A string representing the ID of the work queue processor.
    	/// </summary>
    	/// <remarks>
    	/// <para>
    	/// This ID is used to reset the work queue items.
    	/// </para>
    	/// <para>
    	/// For the time being, the machine ID is tied to the IP address. Assumimg the server
    	/// will be installed on a machine with DHCP disabled or if the DNS server always assign
    	/// the same IP for the machine, this will work fine.
    	/// </para>
    	/// <para>
    	/// Because of this implemenation, all instances of WorkQueueProcessor will have the same ID.
    	/// </para>
    	/// </remarks>
    	public static string ProcessorId
    	{
    		get
    		{
    			if (_processorId == null)
    			{
    				try
    				{
    					String strHostName = Dns.GetHostName();

    					// Find host by name
    					IPHostEntry iphostentry = Dns.GetHostEntry(strHostName);

    					// Enumerate IP addresses, pick an IPv4 address first
    					foreach (IPAddress ipaddress in iphostentry.AddressList)
    					{
    						if (ipaddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
    						{
    							_processorId = ipaddress.ToString();
    							break;
    						}
    					}
    					if (_processorId == null)
    					{
    						foreach (IPAddress ipaddress in iphostentry.AddressList)
    						{
    							_processorId = ipaddress.ToString();
    							break;
    						}
    					}
    				}
    				catch (Exception e)
    				{
    					Platform.Log(LogLevel.Error, e, "Cannot resolve hostname into IP address");
    				}
    			}

    			if (_processorId == null)
    			{
    				Platform.Log(LogLevel.Warn, "Could not determine hostname or IP address of the local machine. Work Queue Processor ID is set to Unknown");
    				_processorId = "Unknown";

    			}

    			return _processorId;
    		}
    	}

		public static StudyHistory CreateStudyHistoryRecord(IUpdateContext updateContext,
			StudyStorageLocation primaryStudyLocation, StudyStorageLocation secondaryStudyLocation,
			StudyHistoryTypeEnum type, object entryInfo, object changeLog)
		{
			StudyHistoryUpdateColumns columns = new StudyHistoryUpdateColumns();
			columns.InsertTime = Platform.Time;
			columns.StudyHistoryTypeEnum = type;
			columns.StudyStorageKey = primaryStudyLocation.GetKey();
			if (secondaryStudyLocation != null)
				columns.DestStudyStorageKey = secondaryStudyLocation.GetKey();
			else
				columns.DestStudyStorageKey = primaryStudyLocation.GetKey();

			columns.StudyData = XmlUtils.SerializeAsXmlDoc(entryInfo) ?? new XmlDocument();
			columns.ChangeDescription = XmlUtils.SerializeAsXmlDoc(changeLog) ?? new XmlDocument();
			IStudyHistoryEntityBroker broker = updateContext.GetBroker<IStudyHistoryEntityBroker>();
			return broker.Insert(columns);
		}

        /// <summary>
        /// Returns a boolean indicating whether the entry is still "active".
        /// A WorkQueue entry is inactive if it is in Failed state or was updated the more than X minutes ago (X is InactiveWorkQueueMinTime in the app settings)
        /// </summary>
        /// <remarks>
        /// </remarks>
        static public bool IsActiveWorkQueue(WorkQueue item)
        {
            if (item.WorkQueueStatusEnum.Equals(WorkQueueStatusEnum.Failed) ||   
                (item.LastUpdatedTime > DateTime.MinValue && item.LastUpdatedTime < Platform.Time - Settings.Default.InactiveWorkQueueMinTime))
                return false;
            else
                return true;
        }

        
    }
}
