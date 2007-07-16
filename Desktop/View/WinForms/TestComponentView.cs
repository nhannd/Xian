using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="TestComponent"/>
    /// </summary>
    [ExtensionOf(typeof(TestComponentViewExtensionPoint))]
    public class TestComponentView : WinFormsView, IApplicationComponentView
    {
        private TestComponent _component;
        private TestComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (TestComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new TestComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
