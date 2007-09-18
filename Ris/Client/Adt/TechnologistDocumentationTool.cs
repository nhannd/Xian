using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;

namespace ClearCanvas.Ris.Client.Adt
{
    [MenuAction("apply", "folderexplorer-items-contextmenu/Document", "Apply")]
    [ButtonAction("apply", "folderexplorer-items-toolbar/Document", "Apply")]
    [IconSet("apply", IconScheme.Colour, "StartToolSmall.png", "StartToolMedium.png", "StartToolLarge.png")]
    [ExtensionOf(typeof(TechnologistWorkflowItemToolExtensionPoint))]
    public class TechnologistDocumentationTool : Tool<ITechnologistWorkflowItemToolContext>
    {
        public void Apply()
        {
            try
            {
                ModalityWorklistItem item = CollectionUtils.FirstElement<ModalityWorklistItem>(this.Context.SelectedItems);

                if (item != null)
                {
                    Document doc = DocumentManager.Get(item.AccessionNumber);
                    if (doc == null)
                    {
                        doc = new TechnologistDocumentationDocument(item.AccessionNumber, item, this.Context.DesktopWindow);
                        doc.Open();
                    }
                    else
                    {
                        doc.Activate();
                    }
                }

            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Context.DesktopWindow);
            }
        }
    }
}
