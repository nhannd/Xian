using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Ris.Client.Admin
{
    /// <summary>
    /// Tool that allows editing of validation rules on a live application component.
    /// </summary>
    [MenuAction("launch", "applicationcomponent-metacontextmenu/Edit Validation Rules", "Launch")]
    [ExtensionOf(typeof(ApplicationComponentMetaToolExtensionPoint))]
    public class ValidationLiveEditTool : Tool<IApplicationComponentMetaToolContext>
    {
        private Shelf _shelf;

        public void Launch()
        {
            if (_shelf == null)
            {
                try
                {
                    ValidationEditorComponent editor = new ValidationEditorComponent(this.Context.Component);

                    _shelf = ApplicationComponent.LaunchAsShelf(
                        this.Context.DesktopWindow,
                        editor,
                        string.Format("{0} rules editor", this.Context.Component.GetType().Name),
                        ShelfDisplayHint.DockFloat);
                    _shelf.Closed += delegate { _shelf = null; };

                }
                catch (Exception e)
                {
                    // could not launch component
                    ExceptionHandler.Report(e, this.Context.DesktopWindow);
                }
            }
            else
            {
                _shelf.Activate();
            }
        }
    }
}
