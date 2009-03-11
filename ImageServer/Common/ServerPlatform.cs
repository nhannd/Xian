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
        #endregion

        #region Constructors
        static ServerPlatform()
        {
            IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();
            using (IReadContext ctx = store.OpenReadContext())
            {
                IDatabaseVersionEntityBroker broker = ctx.GetBroker<IDatabaseVersionEntityBroker>();
                IList<DatabaseVersion> versions = broker.Find(new DatabaseVersionSelectCriteria());
                if (versions != null && versions.Count > 0)
                    _dbVersion = versions[0].GetVersionString();
                else
                    _dbVersion = "Unknown";
            }
        }
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

        public static String GetTempPath()
        {
            return Path.Combine(GetTempFolder(), Path.GetRandomFileName());
        }

        public static String GetTempFolder()
        {
            return Path.Combine(Path.GetPathRoot(Path.GetTempPath()), "temp");
        }

        public static String GetTempFolder(string operation, params string[] references)
        {
            Platform.CheckForEmptyString(operation, "operation");
            string path = Path.Combine(GetTempFolder(), operation);
            if (references!=null)
            {
                foreach(string subFolder in references)
                {
                    path = Path.Combine(path, subFolder);
                }
            }
            return path;
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

        public static String VersionString
        {
            get
            {
                return _dbVersion;
            }
        }

    }
}
