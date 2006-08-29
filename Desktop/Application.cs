using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Defines an extension point for a general dialog box
    /// </summary>
    public class DialogBoxExtensionPoint : ExtensionPoint<IDialogBox>
    {
    }



    [ClearCanvas.Common.ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class Application : IApplicationRoot
    {
        private static Application _instance;

        private IDesktopWindow _window;
        private IDesktopWindowView _view;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkstationModel"/> class.
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

        public static string ApplicationName
        {
            get { return SR.ApplicationName; }
        }

        public static string ApplicationVersion
        {
            get { return SR.ApplicationVersion; }
        }

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

        public static void Quit()
        {
            _instance._view.QuitMessagePump();
        }
    }
}
