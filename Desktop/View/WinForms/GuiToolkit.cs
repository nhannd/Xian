using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using System.ComponentModel;
using System.Windows.Forms;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.View.WinForms
{
    [ExtensionOf(typeof(GuiToolkitExtensionPoint))]
    public class GuiToolkit : IGuiToolkit
    {
        private delegate void InvokeDelegate();

        private event EventHandler _started;

        public GuiToolkit()
        {
            if (!Platform.IsWin32Platform)
                throw new NotSupportedException();
        }

        #region IGuiToolkit Members

        public string ToolkitID
        {
            get { return GuiToolkitID.WinForms; }
        }

        public event EventHandler Started
        {
            add { _started += value; }
            remove { _started -= value; }
        }

        public void Run()
        {
            // this must be called before any GUI objects are created - otherwise we get problems with icons not showing up
            System.Windows.Forms.Application.EnableVisualStyles();

            // create a timer to raise the Started event from the message pump
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 100;
            timer.Tick += delegate(object sender, EventArgs args)
            {
                // immediately disable the timer so we don't get a second Tick
                timer.Dispose();
                EventsHelper.Fire(_started, this, EventArgs.Empty);
            };
            timer.Enabled = true;

            // start the message pump
            System.Windows.Forms.Application.Run();
        }

        public void Terminate()
        {
            System.Windows.Forms.Application.Exit();
        }

        #endregion
    }
}
