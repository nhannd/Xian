using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using System.Threading;
using System.Security.Principal;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Defines an extension point for a general dialog box
    /// </summary>
    public class DialogBoxExtensionPoint : ExtensionPoint<IDialogBox>
    {
    }


    /// <summary>
    /// Singleton class that represents the desktop application as a whole.
    /// </summary>
    [ClearCanvas.Common.ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class Application : IApplicationRoot
    {
        private static Application _instance;

        private IDesktopWindow _window;
        private IDesktopWindowView _view;

        /// <summary>
        /// Initializes a new instance of the <see cref="Application"/> class.
        /// </summary>
        public Application()
        {
            _instance = this;

            _window = new DesktopWindow();

            // Create the view
            _view = (IDesktopWindowView)ViewFactory.CreateAssociatedView(typeof(DesktopWindow));
            _view.SetDesktopWindow(_window);
		}

        /// <summary>
        /// Runs the application by running the view's message pump.  Typically this method will
        /// block until the message pump terminates.
        /// </summary>
        public void RunApplication(string[] args)
        {
            try
            {
                _view.RunMessagePump();
            }
            finally
            {
                CleanUp();
            }
        }

        /// <summary>
        /// Clean up any disposable objects
        /// </summary>
        private void CleanUp()
        {
            _window.Dispose();
        }

        /// <summary>
        /// The name of the application
        /// </summary>
        public static string ApplicationName
        {
            get { return SR.ApplicationName; }
        }

        /// <summary>
        /// The <see cref="GuiToolkitID"/> of the GUI toolkit that is currently in use
        /// </summary>
        public static GuiToolkitID GuiToolkit
        {
            get
            {
                if (Platform.IsWin32Platform)
                    return GuiToolkitID.WinForms;
                if (Platform.IsUnixPlatform)
                    return GuiToolkitID.GTK;

                throw new Exception();  // TODO elaborate
            }
        }

        /// <summary>
        /// Factory method to create a dialog box using the GUI toolkit of the main window
        /// </summary>
        /// <returns></returns>
        public static IDialogBox CreateDialogBox()
        {
            GuiToolkitAttribute testAttr = new GuiToolkitAttribute(GuiToolkit);
            DialogBoxExtensionPoint xp = new DialogBoxExtensionPoint();
            return (IDialogBox)xp.CreateExtension(new AttributeExtensionFilter(testAttr));
        }

        /// <summary>
        /// Quits the application
        /// </summary>
        public static void Quit()
        {
            _instance._view.QuitMessagePump();
        }
    }
}
