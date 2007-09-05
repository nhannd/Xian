using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
    /// <summary>
	/// Provides a Windows Forms view onto <see cref="EditPresetVoiLutComponentContainer"/>
    /// </summary>
	[ExtensionOf(typeof(EditPresetVoiLutComponentContainerViewExtensionPoint))]
	public class EditPresetVoiLutComponentContainerView : WinFormsView, IApplicationComponentView
    {
        private EditPresetVoiLutComponentContainer _component;
        private EditPresetVoiLutComponentContainerControl _control;

        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (EditPresetVoiLutComponentContainer)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new EditPresetVoiLutComponentContainerControl(_component);
                }
                return _control;
            }
        }
    }
}
