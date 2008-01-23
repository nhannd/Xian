using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Ris.Client.SpeechMagic
{
    [MenuAction("launch", "global-menus/Speech Magic/Show Log", "Launch")]
    //[ActionPermission("launch", ClearCanvas.Ris.Application.Common.AuthorityTokens.Developer)]
    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class ShowLogTool : Tool<IDesktopToolContext>
    {
        private IShelf _shelf;

        public override void Initialize()
        {
            base.Initialize();
            Launch();
        }

        public void Launch()
        {
            if (_shelf == null)
            {
                try
                {
                    LogComponent component = new LogComponent();

                    _shelf = ApplicationComponent.LaunchAsShelf(
                        this.Context.DesktopWindow,
                        component,
                        "Speech Magic Log", ShelfDisplayHint.DockLeft);

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

    /// <summary>
    /// Extension point for views onto <see cref="LogComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class LogComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// LogComponent class
    /// </summary>
    [AssociateView(typeof(LogComponentViewExtensionPoint))]
    public class LogComponent : ApplicationComponent
    {
    }
}
