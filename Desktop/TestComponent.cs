using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Extension point for views onto <see cref="TestComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class TestComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// TestComponent class
    /// </summary>
    [AssociateView(typeof(TestComponentViewExtensionPoint))]
    public class TestComponent : ApplicationComponent
    {
        private string _name;
        private string _text;

        /// <summary>
        /// Constructor
        /// </summary>
        public TestComponent(string name)
        {
            _name = name;
        }


        public string Name
        {
            get
            {
                return _name;
            }
        }

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }


        public void ShowMessageBox()
        {
            this.Host.ShowMessageBox("Message from " + _name, MessageBoxActions.Ok);
        }

        public void ShowDialogBox()
        {
            ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, new TestComponent("Dialog from " + _name), "Dialog from " + _name);
        }

        public void Accept()
        {
            this.Host.Exit();
        }

        public void SetTitle()
        {
            this.Host.Title = _text;
        }

        public void Modify()
        {
            this.Modified = true;
        }
    }
}
