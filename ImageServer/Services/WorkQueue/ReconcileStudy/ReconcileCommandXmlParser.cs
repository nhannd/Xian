using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy.CreateStudy;
using ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy.Discard;
using ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy.MergeStudy;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy
{
    /// <summary>
    /// Parses reconciliation commands in Xml format.
    /// </summary>
    /// <remarks>
    /// Currently only "MergeStudy", "CreateStudy" or "Discard" commands are supported.
    /// 
    /// <example>
    /// The following examples shows the xml of the "Discard" and "CreateStudy" commands
    /// <code>
    /// <Discard></Discard>
    /// </code>
    /// 
    /// <code>
    /// <CreateStudy>
    ///     <Set TagPath="00100010" Value="John^Smith"/>
    /// </CreateStudy>
    /// </code>
    /// </example>
    /// </remarks>
    class ReconcileCommandXmlParser
    {

        #region Public Methods
        /// <summary>
        /// Extract a list of <see cref="IReconcileServerCommand"/> in the specified Xml.
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        /// <remarks>
        /// The reconciliation commands should be specified in <ImageCommands> node.
        /// </remarks>
        public IReconcileProcessor Parse(XmlDocument doc)
        {
            //TODO: Validate the xml
            Platform.CheckForNullReference(doc, "doc");

            if (doc.DocumentElement!=null)
            {
                //TODO: use plugin?
                if (doc.DocumentElement.Name == "MergeStudy")
                {
                    return new MergeStudyCommandProcessor();
                }
                else if (doc.DocumentElement.Name == "CreateStudy")
                {
                    return new ReconcileCreateStudyProcessor();
                }
                else if (doc.DocumentElement.Name == "Discard")
                {
                    return new DiscardImageCommandProcessor();
                }
                else
                {
                    throw new NotSupportedException(String.Format("Unsupported reconcile operator: {0}", doc.DocumentElement.Name));
                }
                
            }
            
            return null;
        }
        #endregion

    }
}
