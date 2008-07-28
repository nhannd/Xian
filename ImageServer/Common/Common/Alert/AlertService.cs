using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Common.Alert;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Services.Common.Alert
{
    [ExtensionOf(typeof(CoreServiceExtensionPoint))]
    public class AlertService : ICoreServiceLayer, IAlertService
    {
        private static readonly IAlertServiceExtension[] _extensions;
        private readonly IPersistentStore _store = PersistentStoreRegistry.GetDefaultStore();
        
        static AlertService()
        {
            _extensions = Array.ConvertAll<object, IAlertServiceExtension>(new AlertServiceExtensionPoint().CreateExtensions(),
                                                                           delegate(object ext)
                                                                               { return (IAlertServiceExtension) ext; });

        }

        #region IAlertService Members

        public void GenerateAlert(AlertCategory category, AlertLevel level, String source, String message)
        {
            GenerateAlert(category, level, source, message, null);
            OnAlertGenerated(category, level, source, message, null);
        }

        public void GenerateAlert(AlertCategory category, AlertLevel level, String source, String message, DateTime expirationTime)
        {
            GenerateAlert(category, level, source, message, (DateTime?) expirationTime);
            OnAlertGenerated(category, level, source, message, expirationTime);
        }

        public void GenerateAlert(AlertCategory category, AlertLevel level, string source, object data)
        {
            GenerateAlert(category, level, source, data, null);
        }

        private void GenerateAlert(AlertCategory category, AlertLevel level, String source, object data, DateTime? expirationTime)
        {
            using (IUpdateContext ctx = _store.OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                IAlertEntityBroker broker = ctx.GetBroker<IAlertEntityBroker>();
                AlertUpdateColumns columns = new AlertUpdateColumns();
                columns.InsertTime = Platform.Time;

                switch (level)
                {
                    case AlertLevel.Informational: columns.AlertLevelEnum = AlertLevelEnum.Informational; break;
                    case AlertLevel.Warning: columns.AlertLevelEnum = AlertLevelEnum.Warning; break;
                    case AlertLevel.Error: columns.AlertLevelEnum = AlertLevelEnum.Error; break;
                    case AlertLevel.Critical: columns.AlertLevelEnum = AlertLevelEnum.Critical; break;
                    default:
                        columns.AlertLevelEnum = AlertLevelEnum.Informational; break;
                }

                switch (category)
                {
                    case AlertCategory.System: columns.AlertCategoryEnum = AlertCategoryEnum.System; break;
                    case AlertCategory.Application: columns.AlertCategoryEnum = AlertCategoryEnum.Application; break;
                    case AlertCategory.Security: columns.AlertCategoryEnum = AlertCategoryEnum.Security; break;
                    case AlertCategory.User: columns.AlertCategoryEnum = AlertCategoryEnum.User; break;
                    default:
                        columns.AlertCategoryEnum = AlertCategoryEnum.User; break;
                }

                columns.Source = source;
                XmlDocument doc = new XmlDocument();
                   
                if (data is string)
                {
                    XmlNode content = doc.CreateElement("Message");
                    XmlNode msg = doc.CreateTextNode(data.ToString());
                    content.AppendChild(msg);
                    doc.AppendChild(content);  

                }
                else
                {
                    MemoryStream ms = new MemoryStream();
                    XmlSerializer serializer = new XmlSerializer(data.GetType());
                    serializer.Serialize(ms, data);
                    ms.Seek(0, SeekOrigin.Begin);
                    doc.Load(ms);
                }

                columns.Content = doc;
                   
                columns.Content = doc;
                if (expirationTime != null)
                    columns.ExpirationTime = expirationTime.Value;

                broker.Insert(columns);

                ctx.Commit();
            }
        }



        virtual protected void OnAlertGenerated(AlertCategory category, AlertLevel level, String source, String message, DateTime? expirationTime)
        {
            foreach(IAlertServiceExtension extension in _extensions)
            {
                if (expirationTime != null)
                    extension.OnAlert(category, level, source, message, expirationTime.Value);
                else
                    extension.OnAlert(category, level, source, message);
            }
        }



        #endregion



        #region IAlertService Members


       

        #endregion
    }
}