using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Tools.Volume.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="VolumeComponent"/>
    /// </summary>
    [ExtensionOf(typeof(VolumeComponentViewExtensionPoint))]
    public class VolumeComponentView : WinFormsView, IApplicationComponentView
    {
        private VolumeComponent _component;
        private VolumeComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (VolumeComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new VolumeComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
