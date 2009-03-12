using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy.CreateStudy;
using ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy.Discard;
using ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy.MergeStudy;
using ClearCanvas.ImageServer.Services.WorkQueue.StudyProcess;

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
                ReconcileDescriptionParser parser = new ReconcileDescriptionParser();
                ReconcileDescription desc = parser.Parse(doc);
                switch(desc.Action)
                {
                    case ReconcileAction.CreateNewStudy: return new ReconcileCreateStudyProcessor();
                    case ReconcileAction.Discard: return new DiscardImageCommandProcessor();
                    case ReconcileAction.Merge: return new MergeStudyCommandProcessor();

                    default:
                        throw new NotSupportedException(String.Format("Reconcile Action: {0}", desc.Action));
                }
                
            }

            return null;
        }
        #endregion

    }
}
