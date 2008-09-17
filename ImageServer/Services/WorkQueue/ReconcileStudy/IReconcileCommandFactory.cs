using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy
{
    /// <summary>
    /// Defines the inferace of <see cref="IReconcileServerCommand"/> factory
    /// </summary>
    interface IReconcileCommandFactory
    {
        /// <summary>
        /// Name of the command created by the factory
        /// </summary>
        String CommandName { get; }

        /// <summary>
        /// Parse the list of <see cref="IReconcileServerCommand"/> in the specified xml.
        /// </summary>
        /// <param name="node">The xml node containing the commands</param>
        /// <returns></returns>
        List<IReconcileServerCommand> Parse(XmlNode node);
    }
}