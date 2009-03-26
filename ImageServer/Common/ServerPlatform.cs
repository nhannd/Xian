using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Common
{
    public delegate void ErrorDelegate();


    static public class ServerPlatform
    {
        #region Private Fields
        private static string _dbVersion;
        private static string _tempDir;
        private static object _mutex = new object();
        #endregion

        /// <summary>
        /// Generates an alert message.
        /// </summary>
        /// <param name="category">An alert category</param>
        /// <param name="level">Alert level</param>
        /// <param name="source">Name of the source where the alert is raised</param>
        /// <param name="code">Alert code</param>
        /// <param name="message">The alert message or formatted message.</param>
        /// <param name="args">Paramaters used in the alert message, when specified.</param>
        public static void Alert(AlertCategory category, AlertLevel level, String source, int code, String message, params object[] args)
        {
            Alert(category, level, source, code, TimeSpan.Zero, message, args);
        }

        /// <summary>
        /// Generates an alert message with an expiration time.
        /// </summary>
        /// <param name="category">An alert category</param>
        /// <param name="level">Alert level</param>
        /// <param name="source">Name of the source where the alert is raised</param>
        /// <param name="code">Alert type code</param>
        /// <param name="expirationTime">Expiration time for the alert</param>
        /// <param name="message">The alert message or formatted message.</param>
        /// <param name="args">Paramaters used in the alert message, when specified.</param>
        public static void Alert(AlertCategory category, AlertLevel level, String source, int code, TimeSpan expirationTime, String message, params object[] args)
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
                alert.Code = code;
                alert.ExpirationTime = Platform.Time.Add(expirationTime);
                alert.Source = src;
                alert.Data = String.Format(message, args);
                service.GenerateAlert(alert);
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
                    lock(_mutex)
                    {
                        // if specified in the config, use it
                        if (String.IsNullOrEmpty(ImageServerCommonConfiguration.TemporaryPath))
                        {
                            _tempDir =  Path.Combine(Platform.InstallDirectory, "Temp");
                        }
                        else
                        {
                            _tempDir = ImageServerCommonConfiguration.TemporaryPath;
                        }

                        // make sure it exists
                        if (!Directory.Exists(_tempDir))
                        {
                            try
                            {
                                Directory.CreateDirectory(_tempDir);
                            }
                            catch (Exception ex)
                            {
                                // cannot create directory here... ask windows for help.
                                Platform.Log(LogLevel.Error, ex);
                                _tempDir = Path.GetPathRoot(Path.GetTempPath());
                            }
                        }
                    }
                }

                return _tempDir;
            }
        }

        [Conditional("DEBUG_SIM_ERRORS")]
        public static void SimulateError(string description, ErrorDelegate del)
        {
            Random ran = new Random();
            bool simulate = ran.Next() % (ran.Next(10) + 1) == 0;
            if (simulate)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("\n\n\t**********************************************************************************************************\n");
                sb.AppendFormat("\t                 SIMULATING ERROR: {0}\n", description);
                sb.AppendFormat("\t**********************************************************************************************************\n");
                Platform.Log(LogLevel.Info, sb.ToString());
                del();
            }
        }

        /// <summary>
        /// Returns the version info in the database.
        /// </summary>
        public static String VersionString
        {
            get
            {
                lock (_mutex)
                {
                    if (String.IsNullOrEmpty(_dbVersion))
                    {
                        try
                        {
                            IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();
                            using (IReadContext ctx = store.OpenReadContext())
                            {
                                IDatabaseVersionEntityBroker broker = ctx.GetBroker<IDatabaseVersionEntityBroker>();
                                IList<DatabaseVersion> versions = broker.Find(new DatabaseVersionSelectCriteria());
                                if (versions != null && versions.Count > 0)
                                    _dbVersion = versions[0].GetVersionString();
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

    }
}
