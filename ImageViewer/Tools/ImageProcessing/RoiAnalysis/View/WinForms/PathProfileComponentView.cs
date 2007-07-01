using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.RoiAnalysis.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="PathProfileComponent"/>
    /// </summary>
    [ExtensionOf(typeof(PathProfileComponentViewExtensionPoint))]
    public class PathProfileComponentView : WinFormsView, IApplicationComponentView
    {
        private PathProfileComponent _component;
        private PathProfileComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (PathProfileComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new PathProfileComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
