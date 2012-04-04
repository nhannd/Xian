using System;
using System.ComponentModel;
using System.Windows.Forms;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.View.WinForms
{
    public partial class TimedDialogControl : UserControl, INotifyPropertyChanged
    {
        private readonly TimedDialog _dialog;

        public TimedDialogControl(TimedDialog dialog)
        {
            _dialog = dialog;
            InitializeComponent();

            DataBindings.Add("Message", dialog, "Message", true,
                                     DataSourceUpdateMode.OnPropertyChanged);

            DataBindings.Add("LinkText", dialog, "LinkText", true,
                                         DataSourceUpdateMode.OnPropertyChanged);
        }

        public string Message
        {
            get { return _message.Text; }
            set
            {
                _message.Text = value;
                NotifyPropertyChanged("Message");
            }
        }

        public string LinkText
        {
            get { return _link.Text; }
            set
            {
                _link.Text = value;
                NotifyPropertyChanged("LinkText");
            }
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            EventsHelper.Fire(PropertyChanged, this, new PropertyChangedEventArgs(propertyName));
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private void OnLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            _dialog.FollowLink();   
        }
    }
}
