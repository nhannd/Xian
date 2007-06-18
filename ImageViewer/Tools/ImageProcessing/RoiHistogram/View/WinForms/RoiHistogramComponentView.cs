using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.RoiHistogram.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="RoiHistogramComponent"/>
    /// </summary>
    [ExtensionOf(typeof(RoiHistogramComponentViewExtensionPoint))]
    public class RoiHistogramComponentView : WinFormsView, IApplicationComponentView
    {
        private RoiHistogramComponent _component;
        private RoiHistogramComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (RoiHistogramComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new RoiHistogramComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
