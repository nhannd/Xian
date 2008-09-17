using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy.Commands;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy
{
    /// <summary>
    /// Parses reconciliation commands in Xml format.
    /// </summary>
    /// <remarks>
    /// The reconciliation commands should be specified in the 'ImageCommands' xml node. Currently only
    /// "Discard" command (<see cref="ReconcileDiscardImageCommand"/>) and "UpdateImages" (<see cref="ReconcileUpdateDicomFileCommand"/>)
    /// are supported.
    /// 
    /// <example>
    /// The following examples illustrate how discard and update image commands are specified in the xml format.
    /// <code>
    /// <ImageCommands>
    ///     <Discard></Discard>
    /// </ImageCommands>
    /// </code>
    /// 
    /// <code>
    /// <ImageCommands>
    ///     <UpdateImages>
    ///         <Set TagPath="00100010" Value="John^Smith"/>
    ///     </UpdateImages>
    /// </ImageCommands>
    /// </code>
    /// </example>
    /// </remarks>
    class ReconcileCommandXmlParser
    {
        #region Private Static Methods
        private static IReconcileCommandFactory GetCommandFactory(String command)
        {
            // TODO: Consider using plugin mechanism
            if (command == "Discard")
            {
                return new ReconcileDiscardImageCommandFactory();
            }
            else if (command == "UpdateImages")
            {
                return new ReconcileUpdateImageCommandFactory();
            }
            else
            {
                throw new ApplicationException(String.Format("Unknown command: {0}", command));
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Extract a list of <see cref="IReconcileServerCommand"/> in the specified Xml.
        /// </summary>
        /// <param name="specifications"></param>
        /// <returns></returns>
        /// <remarks>
        /// The reconciliation commands should be specified in <ImageCommands> node.
        /// </remarks>
        public List<IReconcileServerCommand> Parse(XmlDocument specifications)
        {
            //TODO: Validate the xml

            List<IReconcileServerCommand> commandList = new List<IReconcileServerCommand>();
            XmlNode imageCommandsNode = specifications.SelectSingleNode("//ImageCommands");
            if (imageCommandsNode!=null)
            {
                XmlNodeList nodeList = imageCommandsNode.ChildNodes;
                foreach (XmlNode node in nodeList)
                {
                    IReconcileCommandFactory factory = GetCommandFactory(node.Name);
                    if (factory!=null)
                    {
                        List<IReconcileServerCommand> commands = factory.Parse(node);
                        CollectionUtils.ForEach(commands, delegate(IReconcileServerCommand cmd)
                                                           {
                                                               commandList.Add(cmd);
                                                           });
                    }

                }
            }
            return commandList;
        }
        #endregion
    }
}
