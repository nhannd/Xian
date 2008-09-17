using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy.Commands
{
    /// <summary>
    /// Factory of "Discard" command
    /// </summary>
    class ReconcileDiscardImageCommandFactory : IReconcileCommandFactory
    {

        #region IReconcileCommandFactory Members

        public string CommandName
        {
            get { return "Discard"; }
        }

        public List<IReconcileServerCommand> Parse(System.Xml.XmlNode node)
        {
            List<IReconcileServerCommand> list = new List<IReconcileServerCommand>();
            list.Add(new ReconcileDiscardImageCommand());
            return list;
        }

        #endregion
    }
}
