using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.StudyManagement
{
    public class ActivityMonitorWorkspace
    {
        private static Workspace _workspace;

        public static void Show(IDesktopWindow desktopWindow)
        {
            if (_workspace != null)
            {
                _workspace.Activate();
                return;
            }

            var component = new ActivityMonitorComponent();
            _workspace = ApplicationComponent.LaunchAsWorkspace(desktopWindow, component, SR.TitleActivityMonitor);
            _workspace.Closed += ((sender, args) =>
            {
                _workspace = null;
            });
        }
    }
}
