using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// Implementation of the <see cref="IWorkstationView"/> extension point.
    /// </summary>
    [ExtensionOf(typeof(DesktopWindowViewExtensionPoint))]
    public class DesktopView : WinFormsView, IDesktopWindowView
    {
        private IDesktopWindow _window;
        private DesktopForm _mainForm;

        public DesktopView()
        {
        }

        public void SetDesktopWindow(IDesktopWindow window)
        {
            _window = window;
        }

		internal static DesktopViewSettings Settings
		{
			get
			{
				return DesktopViewSettings.Default;
			}
		}

        public void RunMessagePump()
        {
            _mainForm = new DesktopForm(_window);
            System.Windows.Forms.Application.Run(_mainForm);
        }

        public void QuitMessagePump()
        {
            _mainForm.Close();
        }

        public override object GuiElement
        {
            get { return _mainForm; }
        }

        public void CloseActiveWorkspace()
        {
        }


    }
}
