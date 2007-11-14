using System;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="MimeDocumentPreviewComponent"/>
    /// </summary>
    public partial class MimeDocumentPreviewComponentControl : ApplicationComponentUserControl
    {
        private readonly MimeDocumentPreviewComponent _component;

        public MimeDocumentPreviewComponentControl(MimeDocumentPreviewComponent component)
            :base(component)
        {
            InitializeComponent();
            _component = component;

            _splitContainer.Panel1Collapsed = _component.ShowSummary == false;
            _attachments.ShowToolbar = _component.ShowToolbar;

            _attachments.Table = _component.Attachments;
            _attachments.MenuModel = _component.AttachmentActionModel;
            _attachments.ToolbarModel = _component.AttachmentActionModel;
            _attachments.DataBindings.Add("Selection", _component, "Selection", true, DataSourceUpdateMode.OnPropertyChanged);

            RefreshPreview();

            _component.DataChanged += _component_DataChanged;
        }

        void _component_DataChanged(object sender, EventArgs e)
        {
            RefreshPreview();
        }

        void RefreshPreview()
        {
            if (String.IsNullOrEmpty(_component.TempFileName))
            {
                _browser.Url = new Uri("about:blank");
                return;
            }

            _browser.Url = new Uri(_component.TempFileName);
        }
    }
}
