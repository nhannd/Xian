using System;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common.Data;
using ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy.CreateStudy;
using ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy.Discard;
using ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy.MergeStudy;
using ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy.ProcessAsIs;

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
                StudyReconcileDescriptorParser parser = new StudyReconcileDescriptorParser();
                StudyReconcileDescriptor desc = parser.Parse(doc);
                switch(desc.Action)
                {
                    case StudyReconcileAction.CreateNewStudy: return new ReconcileCreateStudyProcessor();
                    case StudyReconcileAction.Discard: return new DiscardImageCommandProcessor();
                    case StudyReconcileAction.Merge: return new MergeStudyCommandProcessor();
                    case StudyReconcileAction.ProcessAsIs: return new ReconcileProcessAsIsProcessor();
                    default:
                        throw new NotSupportedException(String.Format("Reconcile Action: {0}", desc.Action));
                }
                
            }

            return null;
        }
        #endregion

    }
}
