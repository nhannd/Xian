using System;
using System.Collections.Generic;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Common
{
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
            return Path.Combine(Path.Combine(Path.GetPathRoot(Path.GetTempPath()), "temp"), Path.GetRandomFileName());
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
