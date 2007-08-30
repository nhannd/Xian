using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.DynamicTe.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="DynamicTeComponent"/>
    /// </summary>
    [ExtensionOf(typeof(DynamicTeComponentViewExtensionPoint))]
    public class DynamicTeComponentView : WinFormsView, IApplicationComponentView
    {
        private DynamicTeComponent _component;
        private DynamicTeComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (DynamicTeComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new DynamicTeComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
