using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;

namespace ClearCanvas.ImageServer.Services.Common.Alert
{
    /// <summary>
    /// Alert record service
    /// </summary>
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    public class AlertService : IApplicationServiceLayer, IAlertService
    {
        #region Private Members
        private IAlertServiceExtension[] _extensions;
        #endregion

        #region Private Methods
        
        private IAlertServiceExtension[] GetExtensions()
        {
            if (_extensions == null)
            {
                _extensions =
                    CollectionUtils.ToArray<IAlertServiceExtension>(new AlertServiceExtensionPoint().CreateExtensions());
            }

            return _extensions;
        }

        #endregion

        #region IAlertService Members

        public void GenerateAlert(ImageServer.Common.Alert alert)
        {
            IAlertServiceExtension[] extensions = GetExtensions();
            foreach(IAlertServiceExtension ext in extensions)
            {
                try
                {
                    ext.OnAlert(alert);    
                }
                catch(Exception e)
                {
                    Platform.Log(LogLevel.Error, e, "Error occurred when calling {0} OnAlert()", ext.GetType());
                }
            }

        }
       

        #endregion
    }
}