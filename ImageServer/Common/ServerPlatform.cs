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
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Common.Audit;
using ClearCanvas.Dicom.Audit;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Common
{
    static public class ServerPlatform
    {
        #region Private Fields
        private static string _dbVersion;
        private static string _tempDir;
        private static readonly object _syncLock = new object();
    	private static DicomAuditSource _auditSource;
    	private static AuditLog _log; 
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
                src.Host = ServiceTools.ServerInstanceId;
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
    }
}
