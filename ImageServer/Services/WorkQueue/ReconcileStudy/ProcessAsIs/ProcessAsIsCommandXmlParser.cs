using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Data;
using ClearCanvas.ImageServer.Common.Utilities;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy.ProcessAsIs
{

    /// <summary>
    /// "MergeStudy" xml parser.
    /// </summary>
    public class ProcessAsIsCommandXmlParser
    {
        public ReconcileProcessAsIsDescriptor Parse(XmlDocument doc)
        {
            if (doc == null)
                return null;

            if (doc.DocumentElement.Name == "Reconcile")
            {
                return XmlUtils.Deserialize<ReconcileProcessAsIsDescriptor>(doc.DocumentElement);
            }
            else
            {
                throw new ApplicationException("Unexpected xml format");
            }
        }
    }
}
