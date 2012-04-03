using System.Windows.Forms;
using System.ComponentModel;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
    public partial class StudyBrowserBannerControl : UserControl, INotifyPropertyChanged
    {
        public StudyBrowserBannerControl()
        {
            InitializeComponent();
            _notificationText.Visible = !string.IsNullOrEmpty(NotificationText);
        }

        public string Text
        {
            get { return _resultsTitleBar.Text; }
            set
            {
                _resultsTitleBar.Text = value;
                EventsHelper.Fire(PropertyChanged, this, new PropertyChangedEventArgs("Text"));
            }
        }

        public string NotificationText
        {
            get { return _notificationText.Text; }
            set
            {
                _notificationText.Text = value;
                EventsHelper.Fire(PropertyChanged, this, new PropertyChangedEventArgs("NotificationText"));
                _notificationText.Visible = !string.IsNullOrEmpty(NotificationText);
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
